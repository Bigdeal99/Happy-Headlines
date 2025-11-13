# Testing Guide for SubscriberService

## Quick Start Testing

### Step 1: Start All Services

```powershell
# Navigate to project directory
cd C:\Users\marce\Documents\GitHub\Happy-Headlines

# Start all services
docker compose up -d --build
```

Wait for all services to be healthy (check with `docker compose ps`).

### Step 2: Test SubscriberService Directly

#### 2.1 Subscribe a User

```powershell
$body = @{
    email = "test@example.com"
    name = "Test User"
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body
```

**Expected Response:**
```json
{
  "id": 1,
  "email": "test@example.com",
  "subscribedAt": "2025-01-06T..."
}
```

#### 2.2 Get All Subscribers

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers?activeOnly=true"
```

**Expected Response:**
```json
[
  {
    "id": 1,
    "email": "test@example.com",
    "name": "Test User",
    "subscribedAt": "2025-01-06T...",
    "isActive": true
  }
]
```

#### 2.3 Unsubscribe

```powershell
Invoke-RestMethod -Method Delete -Uri "http://localhost:8088/api/subscribers/test@example.com"
```

**Expected Response:**
```json
{
  "message": "Unsubscribed successfully"
}
```

### Step 3: Test Feature Flag (Release Toggle)

#### 3.1 Check Feature Flag Status

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers/feature-flag"
```

**Expected Response:**
```json
{
  "enabled": true
}
```

#### 3.2 Disable Feature Flag

```powershell
$body = @{
    enabled = $false
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers/feature-flag" -ContentType "application/json" -Body $body
```

#### 3.3 Try to Subscribe (Should Fail with 503)

```powershell
$body = @{
    email = "test2@example.com"
    name = "Test User 2"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body
} catch {
    Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)"
    Write-Host "Expected: 503 (Service Unavailable)"
}
```

**Expected:** HTTP 503 error

#### 3.4 Re-enable Feature Flag

```powershell
$body = @{
    enabled = $true
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers/feature-flag" -ContentType "application/json" -Body $body
```

### Step 4: Test NewsletterService Integration

#### 4.1 Subscribe Multiple Users

```powershell
# Subscribe user 1
$body1 = @{
    email = "newsletter1@example.com"
    name = "Newsletter User 1"
} | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body1

# Subscribe user 2
$body2 = @{
    email = "newsletter2@example.com"
    name = "Newsletter User 2"
} | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body2
```

#### 4.2 Send Newsletter

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:8086/api/newsletter/send"
```

**Expected Response:**
```json
{
  "sent": true,
  "subscribersCount": 2,
  "subscribers": [
    {
      "email": "newsletter1@example.com",
      "name": "Newsletter User 1"
    },
    {
      "email": "newsletter2@example.com",
      "name": "Newsletter User 2"
    }
  ],
  "preview": "[articles JSON]"
}
```

### Step 5: Test Fault Isolation

#### 5.1 Stop SubscriberService

```powershell
docker compose stop subscriber-service
```

#### 5.2 NewsletterService Should Still Work

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:8086/api/newsletter/send"
```

**Expected Response:**
```json
{
  "sent": true,
  "subscribersCount": 0,
  "preview": "[articles JSON]"
}
```

**Note:** NewsletterService continues to work even though SubscriberService is down (fault isolation).

#### 5.3 Restart SubscriberService

```powershell
docker compose start subscriber-service
```

### Step 6: Test RabbitMQ Queue

#### 6.1 Check RabbitMQ Management UI

1. Open browser: http://localhost:15673
2. Login: `guest` / `guest`
3. Go to **Queues** tab
4. Look for `subscriber_queue`
5. You should see messages when subscribers are added

#### 6.2 Subscribe and Check Queue

```powershell
# Subscribe a new user
$body = @{
    email = "queue-test@example.com"
    name = "Queue Test User"
} | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body

# Check RabbitMQ UI - you should see a new message in subscriber_queue
```

## Complete Test Script

Save this as `test-subscriber-service.ps1`:

```powershell
Write-Host "=== Testing SubscriberService ===" -ForegroundColor Green

# Test 1: Subscribe
Write-Host "`n1. Testing Subscribe..." -ForegroundColor Yellow
$body = @{
    email = "test@example.com"
    name = "Test User"
} | ConvertTo-Json

try {
    $result = Invoke-RestMethod -Method Post -Uri "http://localhost:8088/api/subscribers" -ContentType "application/json" -Body $body
    Write-Host "✓ Subscribe successful: ID=$($result.id)" -ForegroundColor Green
} catch {
    Write-Host "✗ Subscribe failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Get Subscribers
Write-Host "`n2. Testing Get Subscribers..." -ForegroundColor Yellow
try {
    $subscribers = Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers?activeOnly=true"
    Write-Host "✓ Found $($subscribers.Count) active subscribers" -ForegroundColor Green
} catch {
    Write-Host "✗ Get subscribers failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Feature Flag
Write-Host "`n3. Testing Feature Flag..." -ForegroundColor Yellow
try {
    $flag = Invoke-RestMethod -Method Get -Uri "http://localhost:8088/api/subscribers/feature-flag"
    Write-Host "✓ Feature flag status: $($flag.enabled)" -ForegroundColor Green
} catch {
    Write-Host "✗ Feature flag check failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Newsletter Integration
Write-Host "`n4. Testing NewsletterService Integration..." -ForegroundColor Yellow
try {
    $newsletter = Invoke-RestMethod -Method Post -Uri "http://localhost:8086/api/newsletter/send"
    Write-Host "✓ Newsletter sent to $($newsletter.subscribersCount) subscribers" -ForegroundColor Green
} catch {
    Write-Host "✗ Newsletter send failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Testing Complete ===" -ForegroundColor Green
```

Run it:
```powershell
.\test-subscriber-service.ps1
```

## Troubleshooting

### Service Not Responding

```powershell
# Check if services are running
docker compose ps

# Check logs
docker compose logs subscriber-service
docker compose logs newsletter-service
```

### Database Connection Issues

```powershell
# Check database is ready
docker compose logs subscriber-db

# Restart subscriber-service
docker compose restart subscriber-service
```

### Redis Connection Issues

```powershell
# Check Redis
docker compose logs redis

# Test Redis connection
docker compose exec redis redis-cli ping
```

### RabbitMQ Issues

```powershell
# Check RabbitMQ
docker compose logs rabbitmq

# Check RabbitMQ UI
# Open http://localhost:15673 (guest/guest)
```

## Expected Behavior Summary

| Test | Expected Result |
|------|----------------|
| Subscribe | Returns subscriber ID and email |
| Get Subscribers | Returns list of active subscribers |
| Unsubscribe | Returns success message |
| Feature Flag Disabled | All endpoints return 503 |
| Feature Flag Enabled | Service works normally |
| Newsletter with Subscribers | Returns newsletter with subscriber list |
| Newsletter with SubscriberService Down | Returns newsletter with 0 subscribers (fault isolation) |
| RabbitMQ Queue | Messages appear when subscribers are added |

---

**Note:** Make sure all services are running before testing:
```powershell
docker compose up -d
docker compose ps  # Verify all services are "Up"
```

