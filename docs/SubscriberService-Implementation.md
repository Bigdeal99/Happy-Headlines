# SubscriberService Implementation

## Overview

The SubscriberService has been implemented with the following features:
- **Database**: SQL Server for storing subscribers
- **Queue**: RabbitMQ for publishing new subscribers (for welcome emails)
- **Feature Flag/Release Toggle**: Redis-based feature flag to enable/disable service without redeployment
- **Fault Isolation**: Circuit breaker pattern in NewsletterService to prevent cascading failures

## Architecture

### Services in Separate Swimlanes

- **NewsletterService** (Port 8086): Newsletter generation service
- **SubscriberService** (Port 8088): Subscriber management service

These services are isolated and communicate via HTTP with fault tolerance.

## Feature Flag / Release Toggle

The SubscriberService can be enabled/disabled without redeployment using Redis:

### Check Feature Flag Status
```bash
GET http://localhost:8088/api/subscribers/feature-flag
```

### Enable Feature Flag
```bash
POST http://localhost:8088/api/subscribers/feature-flag
Content-Type: application/json

{
  "enabled": true
}
```

### Disable Feature Flag
```bash
POST http://localhost:8088/api/subscribers/feature-flag
Content-Type: application/json

{
  "enabled": false
}
```

**Storage**: Feature flag is stored in Redis at key `feature:subscriber-service:enabled`
- Default: `true` (enabled) if not set
- When disabled, all endpoints return `503 Service Unavailable`

## Fault Isolation

### Circuit Breaker Pattern

NewsletterService uses Polly circuit breaker to isolate faults from SubscriberService:

- **Circuit Breaker**: Opens after 3 consecutive failures
- **Break Duration**: 30 seconds
- **Retry Policy**: 2 retries with exponential backoff
- **Timeout**: 5 seconds per request

**Behavior**:
- If SubscriberService fails, NewsletterService continues to function
- Newsletter is sent with 0 subscribers instead of failing completely
- Logs warnings but doesn't crash

## API Endpoints

### Subscribe
```http
POST /api/subscribers
Content-Type: application/json

{
  "email": "user@example.com",
  "name": "John Doe"
}
```

**Response**:
```json
{
  "id": 1,
  "email": "user@example.com",
  "subscribedAt": "2025-01-06T12:00:00Z"
}
```

### Unsubscribe
```http
DELETE /api/subscribers/{email}
```

**Response**:
```json
{
  "message": "Unsubscribed successfully"
}
```

### Get Subscribers
```http
GET /api/subscribers?activeOnly=true
```

**Response**:
```json
[
  {
    "id": 1,
    "email": "user@example.com",
    "name": "John Doe",
    "subscribedAt": "2025-01-06T12:00:00Z",
    "isActive": true
  }
]
```

## Queue Integration

When a subscriber subscribes:
1. Subscriber is saved to database
2. Message is published to RabbitMQ queue `subscriber_queue`
3. NewsletterService can consume from this queue to send welcome emails

**Queue Message Format**:
```json
{
  "SubscriberId": 1,
  "Email": "user@example.com",
  "Name": "John Doe",
  "Timestamp": "2025-01-06T12:00:00Z"
}
```

## Testing

### 1. Test SubscriberService Directly

```bash
# Subscribe
curl -X POST http://localhost:8088/api/subscribers \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","name":"Test User"}'

# Get subscribers
curl http://localhost:8088/api/subscribers

# Unsubscribe
curl -X DELETE http://localhost:8088/api/subscribers/test@example.com
```

### 2. Test Feature Flag

```bash
# Disable service
curl -X POST http://localhost:8088/api/subscribers/feature-flag \
  -H "Content-Type: application/json" \
  -d '{"enabled":false}'

# Try to subscribe (should return 503)
curl -X POST http://localhost:8088/api/subscribers \
  -H "Content-Type: application/json" \
  -d '{"email":"test2@example.com","name":"Test User 2"}'

# Re-enable service
curl -X POST http://localhost:8088/api/subscribers/feature-flag \
  -H "Content-Type: application/json" \
  -d '{"enabled":true}'
```

### 3. Test Fault Isolation

```bash
# Stop SubscriberService
docker compose stop subscriber-service

# NewsletterService should still work (with 0 subscribers)
curl -X POST http://localhost:8086/api/newsletter/send

# Response should show:
# {
#   "sent": true,
#   "subscribersCount": 0,
#   "preview": "...",
#   "message": "SubscriberService unavailable"
# }
```

### 4. Test NewsletterService Integration

```bash
# Subscribe a user
curl -X POST http://localhost:8088/api/subscribers \
  -H "Content-Type: application/json" \
  -d '{"email":"newsletter@example.com","name":"Newsletter User"}'

# Send newsletter (should include subscriber)
curl -X POST http://localhost:8086/api/newsletter/send

# Response should show:
# {
#   "sent": true,
#   "subscribersCount": 1,
#   "subscribers": [{"email":"newsletter@example.com","name":"Newsletter User"}],
#   "preview": "..."
# }
```

## Environment Variables

### SubscriberService
- `DB_CONNECTION`: SQL Server connection string
- `REDIS_CONNECTION`: Redis connection (default: `redis:6379`)
- `RABBIT_HOST`: RabbitMQ host (default: `rabbitmq`)
- `OTLP_ENDPOINT`: OpenTelemetry endpoint (default: `http://jaeger:4317`)

### NewsletterService
- `ARTICLE_URL`: ArticleService URL (default: `http://article-service:8080`)
- `SUBSCRIBER_URL`: SubscriberService URL (default: `http://subscriber-service:8080`)
- `OTLP_ENDPOINT`: OpenTelemetry endpoint (default: `http://jaeger:4317`)

## CI/CD Integration

SubscriberService is included in the CI/CD pipeline and will be built and pushed to GitHub Container Registry automatically on push to main/master branches.

**Image**: `ghcr.io/{owner}/happy-headlines/subscriber-service:latest`

---

**Last Updated**: 2025-01-06

