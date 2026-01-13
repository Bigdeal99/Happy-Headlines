# Spørgsmål 8: Design to be monitored

## A. Beskriv "Design to be monitored"-princippet, herunder forskellen på metrics, logging og tracing

### "Design to be monitored"-princippet:

**Definition:**
Systemet skal designes fra starten med monitoring i tankerne, ikke tilføjes som eftertanke. Monitoring er en first-class citizen i arkitekturen.

**Principper:**
1. **Instrumentation by Design**: Alle kritiske operationer skal instrumenteres
2. **Observability**: Systemet skal være observerbart (kan forstås uden at kende intern implementation)
3. **Proactive Monitoring**: Identificer problemer før de påvirker brugere
4. **Distributed Context**: I microservices skal man kunne følge requests på tværs af services

### Forskellen på Metrics, Logging og Tracing:

#### 1. **Metrics**

**Definition:**
Numeriske værdier der måler systemets tilstand over tid. Aggregeret data.

**Karakteristika:**
- **Format**: Numeriske værdier (counters, gauges, histograms)
- **Volume**: Lav (samlet data)
- **Retention**: Lang (måneder/år)
- **Purpose**: Trend analysis, alerting, dashboards
- **Query**: Aggregeret queries (sum, avg, rate)

**Eksempler fra kodebasen:**
```csharp
// ArticleService/Program.cs (linje 36-37)
var articleCacheHits = Metrics.CreateCounter("article_cache_hits_total", "Article cache hits");
var articleCacheMisses = Metrics.CreateCounter("article_cache_misses_total", "Article cache misses");

// ArticleService/Controllers/ArticlesController.cs (linje 33-38)
var cached = await _cache.StringGetAsync(cacheKey);
if (cached.HasValue)
{
    _hits.Inc();  // Increment counter
    return Content(cached!, "application/json");
}
```

**Eksempler:**
- Cache hit rate: 80%
- Request rate: 1000 req/s
- Error rate: 0.5%
- Response time (p95): 50ms

#### 2. **Logging**

**Definition:**
Strukturerede events og beskeder der beskriver hvad der sker i systemet. Diskret data.

**Karakteristika:**
- **Format**: Strukturerede events (JSON, key-value pairs)
- **Volume**: Høj (mange log entries)
- **Retention**: Kort-mellemlang (dage/uger)
- **Purpose**: Debugging, audit trail, understanding what happened
- **Query**: Text search, filtering, correlation

**Eksempler fra kodebasen:**
```csharp
// DraftService/Program.cs (linje 10-14)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(serverUrl: seqUrl)  // Centralized logging
    .CreateLogger();

// DraftService/Program.cs (linje 52)
Log.Warning(ex, "Draft DB not ready yet, retrying...");
```

**Eksempler:**
- "ArticleService started at 2024-01-06 10:00:00"
- "Cache miss for key: articles:latest:5"
- "Database connection failed: timeout after 5 seconds"
- "User 123 created comment on article 456"

#### 3. **Tracing**

**Definition:**
Spores en enkelt request gennem hele systemet på tværs af services. Kontekstuel data.

**Karakteristika:**
- **Format**: Spans med parent-child relationships
- **Volume**: Mellem (per request)
- **Retention**: Kort (timer/dage)
- **Purpose**: Understanding request flow, identifying bottlenecks
- **Query**: Trace by trace ID, follow request path

**Eksempler fra kodebasen:**
```csharp
// ArticleService/Services/ArticleQueueConsumer.cs (linje 19, 46-49)
private static readonly ActivitySource ActivitySrc = new("ArticleService");

using var activity = ActivitySrc.StartActivity(
    "Consume Article",
    ActivityKind.Consumer,
    parentContext.ActivityContext);

// PublisherService/Services/ArticleQueuePublisher.cs (linje 40)
using var activity = ActivitySrc.StartActivity("Publish Article", ActivityKind.Producer);
```

**Eksempler:**
- Trace ID: `abc123def456`
- Span 1: PublisherService → Publish Article (50ms)
- Span 2: RabbitMQ → Queue Message (5ms)
- Span 3: ArticleService → Consume Article (100ms)
- Span 4: Database → Save Article (80ms)

### Sammenligning:

| Aspekt | Metrics | Logging | Tracing |
|--------|---------|---------|---------|
| **Data Type** | Numerisk (aggregated) | Events (discrete) | Spans (contextual) |
| **Volume** | Lav | Høj | Mellem |
| **Retention** | Lang (måneder) | Kort (dage) | Kort (timer) |
| **Purpose** | Trends, alerting | Debugging, audit | Request flow |
| **Query** | Aggregation | Search, filter | Trace by ID |
| **Example** | "1000 req/s" | "Request failed" | "Request path: A→B→C" |

### Kombineret Approach (Fra kodebasen):

```csharp
// ArticleService eksempel - Alle tre typer
// 1. Metrics (Prometheus)
_hits.Inc();  // Counter increment

// 2. Logging (Serilog/Seq)
_logger.LogInformation("Article cache warmed for last 14 days");

// 3. Tracing (OpenTelemetry/Jaeger)
using var activity = ActivitySrc.StartActivity("Consume Article", ...);
```

---

## B. Demonstrer hvorfor skalering på y-aksen skaber problemer for monitorerings-princippet

### Y-akse Skalering (Functional Decomposition):

**Definition:**
Split systemet efter funktionalitet → Microservices (ArticleService, CommentService, ProfanityService, etc.)

### Problemer med Monitoring:

#### Problem 1: **Distributed Metrics Aggregation**

**Før Y-akse (Monolitisk):**
```
Monolitisk App
    ↓
1 Prometheus scrape target
    ↓
Alle metrics samlet ét sted
    ↓
Simpel query: sum(requests_total)
```

**Efter Y-akse (Microservices):**
```
ArticleService → Prometheus scrape target 1
CommentService → Prometheus scrape target 2
ProfanityService → Prometheus scrape target 3
PublisherService → Prometheus scrape target 4
...
    ↓
6 separate scrape targets
    ↓
Kompleks query: sum(requests_total{service="article"}) + sum(requests_total{service="comment"}) + ...
```

**Kodeeksempel:**
```yaml
# docs/prometheus.yml (linje 4-18)
scrape_configs:
  - job_name: 'article-service'
    static_configs:
      - targets: ['article-service:8080', 'happy-headlines-article-service-1:8080', ...]
    metrics_path: /metrics

  - job_name: 'comment-service'
    static_configs:
      - targets: ['comment-service:8080', 'happy-headlines-comment-service-1:8080', ...]
    metrics_path: /metrics

  - job_name: 'publisher-service'
    static_configs:
      - targets: ['publisher-service:8080']
    metrics_path: /metrics
```

**Problem:**
- **Før**: 1 scrape target → Simpel konfiguration
- **Efter**: 6 scrape targets → Kompleks konfiguration
- **Query kompleksitet**: Skal aggregere fra flere services

#### Problem 2: **Inconsistent Metric Names**

**Før Y-akse (Monolitisk):**
```csharp
// Alle metrics i samme service
Metrics.CreateCounter("cache_hits_total", "Cache hits");
Metrics.CreateCounter("cache_misses_total", "Cache misses");
```

**Efter Y-akse (Microservices):**
```csharp
// ArticleService/Program.cs (linje 36-37)
var articleCacheHits = Metrics.CreateCounter("article_cache_hits_total", "Article cache hits");
var articleCacheMisses = Metrics.CreateCounter("article_cache_misses_total", "Article cache misses");

// CommentService/Program.cs (linje 54-55)
var commentCacheHits = Metrics.CreateCounter("comment_cache_hits_total", "Comment cache hits");
var commentCacheMisses = Metrics.CreateCounter("comment_cache_misses_total", "Comment cache misses");
```

**Problem:**
- **Før**: `cache_hits_total` (én metric)
- **Efter**: `article_cache_hits_total`, `comment_cache_hits_total` (forskellige navne)
- **Query kompleksitet**: Skal query flere metrics og aggregere

**Løsning (Standardisering):**
```promql
# Skal aggregere fra flere services
sum(rate(article_cache_hits_total[5m])) + sum(rate(comment_cache_hits_total[5m]))
```

#### Problem 3: **Distributed Tracing Complexity**

**Før Y-akse (Monolitisk):**
```
Request → Monolitisk App
    ↓
1 trace, 1 span
    ↓
Simpel: Alt sker i samme process
```

**Efter Y-akse (Microservices):**
```
Request → PublisherService
    ↓
PublisherService → ProfanityService (HTTP call)
    ↓
PublisherService → RabbitMQ
    ↓
ArticleService (consumer) → Database
    ↓
4 services, 6+ spans, distributed context
```

**Kodeeksempel:**
```csharp
// PublisherService/Services/ArticleQueuePublisher.cs (linje 40, 48-52)
using var activity = ActivitySrc.StartActivity("Publish Article", ActivityKind.Producer);

// Inject trace context into RabbitMQ headers
Propagator.Inject(
    new PropagationContext(Activity.Current?.Context ?? default, default),
    props.Headers,
    (hdrs, key, value) => hdrs[key] = Encoding.UTF8.GetBytes(value)
);

// ArticleService/Services/ArticleQueueConsumer.cs (linje 41-44, 46-49)
// Extract trace context from RabbitMQ headers
var parentContext = Propagator.Extract(default, ea.BasicProperties.Headers, ...);

using var activity = ActivitySrc.StartActivity(
    "Consume Article",
    ActivityKind.Consumer,
    parentContext.ActivityContext);
```

**Problem:**
- **Før**: Ingen trace context propagation nødvendig
- **Efter**: Trace context skal propagere gennem HTTP, RabbitMQ, etc.
- **Kompleksitet**: W3C Trace Context propagation, correlation IDs

#### Problem 4: **Centralized Logging Aggregation**

**Før Y-akse (Monolitisk):**
```
Monolitisk App
    ↓
1 log stream
    ↓
Simpel: Alle logs samme sted
```

**Efter Y-akse (Microservices):**
```
ArticleService → Log stream 1
CommentService → Log stream 2
ProfanityService → Log stream 3
DraftService → Seq (centralized)
PublisherService → Log stream 4
...
    ↓
6 separate log streams
    ↓
Kompleks: Skal aggregere fra flere kilder
```

**Kodeeksempel:**
```csharp
// DraftService/Program.cs (linje 9-14)
// Centralized logging til Seq
var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://seq:5341";
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(serverUrl: seqUrl)  // Centralized
    .CreateLogger();
```

**Problem:**
- **Før**: 1 log stream → Simpel
- **Efter**: 6 log streams → Skal aggregere i Seq
- **Correlation**: Skal bruge correlation IDs for at følge requests

**Correlation ID Eksempel:**
```csharp
// DraftService/Middleware/CorrelationIdMiddleware.cs
// Correlation ID for at følge requests på tværs af services
app.UseMiddleware<CorrelationIdMiddleware>();
```

#### Problem 5: **Service Discovery for Monitoring**

**Før Y-akse (Monolitisk):**
```
Prometheus konfiguration:
  - targets: ['monolith:8080']
```

**Efter Y-akse (Microservices):**
```
Prometheus konfiguration:
  - job_name: 'article-service'
    static_configs:
      - targets: ['article-service:8080', 'article-service-2:8080', ...]
  - job_name: 'comment-service'
    static_configs:
      - targets: ['comment-service:8080', ...]
  # ... 4 flere services
```

**Problem:**
- **Før**: 1 target → Statisk konfiguration
- **Efter**: 6+ targets → Dynamisk service discovery nødvendig
- **Skalering**: Når services skaleres (X-akse), skal Prometheus opdage nye instances

**Eksempel fra prometheus.yml:**
```yaml
# docs/prometheus.yml (linje 6-7)
# Flere instances af samme service
- targets: ['article-service:8080','happy-headlines-article-service-1:8080','happy-headlines-article-service-2:8080','happy-headlines-article-service-3:8080']
```

#### Problem 6: **Cross-Service Metrics Correlation**

**Før Y-akse (Monolitisk):**
```
Request latency = Total time i monolit
    ↓
Simpel: 1 metric viser alt
```

**Efter Y-akse (Microservices):**
```
Request latency = 
  PublisherService latency +
  ProfanityService latency +
  RabbitMQ latency +
  ArticleService latency
    ↓
Kompleks: Skal aggregere fra flere services
```

**Problem:**
- **Før**: `request_duration_seconds` viser total tid
- **Efter**: Skal aggregere `publisher_request_duration + profanity_request_duration + article_request_duration`
- **Tracing nødvendig**: For at forstå hvor tid går

### Løsninger til Y-akse Monitoring Problemer:

#### 1. **Standardiserede Metric Names**
```promql
# Standardiseret naming convention
{service="article",metric="cache_hits_total"}
{service="comment",metric="cache_hits_total"}

# Aggregation
sum(rate(cache_hits_total[5m])) by (service)
```

#### 2. **Distributed Tracing (OpenTelemetry)**
```csharp
// Alle services bruger OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ArticleService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o => o.Endpoint = new Uri("http://jaeger:4317")));
```

#### 3. **Centralized Logging (Seq)**
```csharp
// Alle services sender logs til Seq
.WriteTo.Seq(serverUrl: seqUrl)
```

#### 4. **Service Discovery**
```yaml
# Prometheus service discovery
- job_name: 'kubernetes-services'
  kubernetes_sd_configs:
    - role: service
```

---

## C. Vurder hvilke data i logs og traces, der kan udgøre nyttige metrics for overvågning af et distribueret system

### Data fra Logs → Metrics:

#### 1. **Error Logs → Error Rate Metric**

**Log Data:**
```json
{
  "timestamp": "2024-01-06T10:00:00Z",
  "level": "Error",
  "message": "Database connection failed",
  "service": "ArticleService",
  "exception": "SqlException: Timeout"
}
```

**Metric:**
```promql
# Error rate fra logs
rate(error_logs_total{service="article"}[5m])
```

**Værdi:**
- ✅ Proaktiv alerting ved høj error rate
- ✅ Trend analysis over tid
- ✅ Service comparison (hvilken service fejler mest?)

#### 2. **Request Logs → Request Rate Metric**

**Log Data:**
```json
{
  "timestamp": "2024-01-06T10:00:00Z",
  "level": "Information",
  "message": "GET /api/articles?top=5",
  "service": "ArticleService",
  "statusCode": 200,
  "duration": 45
}
```

**Metric:**
```promql
# Request rate fra logs
rate(request_logs_total{service="article",status="200"}[5m])
```

**Værdi:**
- ✅ Throughput monitoring
- ✅ Traffic pattern analysis
- ✅ Capacity planning

#### 3. **Cache Logs → Cache Hit Rate Metric**

**Log Data:**
```json
{
  "timestamp": "2024-01-06T10:00:00Z",
  "level": "Information",
  "message": "Cache miss for key: articles:latest:5",
  "service": "ArticleService",
  "cacheKey": "articles:latest:5"
}
```

**Metric:**
```promql
# Cache hit rate (fra logs eller direkte metrics)
rate(cache_hits_total[5m]) / (rate(cache_hits_total[5m]) + rate(cache_misses_total[5m]))
```

**Værdi:**
- ✅ Cache performance monitoring
- ✅ Identificer cache issues
- ✅ Optimize cache strategy

**Eksempel fra kodebasen:**
```csharp
// ArticleService/Controllers/ArticlesController.cs (linje 33-38, 47-49)
// Allerede implementeret som direkte metrics
_hits.Inc();  // Counter
_misses.Inc();  // Counter
```

#### 4. **Database Query Logs → Database Performance Metric**

**Log Data:**
```json
{
  "timestamp": "2024-01-06T10:00:00Z",
  "level": "Warning",
  "message": "Slow query detected",
  "service": "ArticleService",
  "query": "SELECT * FROM Articles",
  "duration": 5000
}
```

**Metric:**
```promql
# Slow query rate
rate(slow_query_logs_total{service="article",duration>1000}[5m])
```

**Værdi:**
- ✅ Database performance monitoring
- ✅ Identificer N+1 queries
- ✅ Optimize database queries

### Data fra Traces → Metrics:

#### 1. **Span Duration → Response Time Metric**

**Trace Data:**
```json
{
  "traceId": "abc123def456",
  "spanId": "span1",
  "service": "ArticleService",
  "operation": "GET /api/articles",
  "duration": 45,
  "status": "OK"
}
```

**Metric:**
```promql
# Response time (p95) fra traces
histogram_quantile(0.95, rate(span_duration_seconds_bucket[5m]))
```

**Værdi:**
- ✅ SLA monitoring (p95, p99 response times)
- ✅ Performance degradation detection
- ✅ Service comparison

#### 2. **Span Count → Request Rate Metric**

**Trace Data:**
```json
{
  "traceId": "abc123def456",
  "spans": [
    {"service": "PublisherService", "operation": "Publish"},
    {"service": "ProfanityService", "operation": "Check"},
    {"service": "ArticleService", "operation": "Save"}
  ]
}
```

**Metric:**
```promql
# Request rate fra traces
rate(spans_total{service="article",operation="GET /api/articles"}[5m])
```

**Værdi:**
- ✅ Throughput monitoring
- ✅ Request pattern analysis
- ✅ Load balancing validation

#### 3. **Error Spans → Error Rate Metric**

**Trace Data:**
```json
{
  "traceId": "abc123def456",
  "spanId": "span1",
  "service": "ArticleService",
  "operation": "GET /api/articles",
  "status": "ERROR",
  "error": "Database timeout"
}
```

**Metric:**
```promql
# Error rate fra traces
rate(error_spans_total{service="article"}[5m]) / rate(spans_total{service="article"}[5m])
```

**Værdi:**
- ✅ Error rate monitoring
- ✅ Service health indicators
- ✅ Alerting thresholds

#### 4. **Cross-Service Latency → Service Dependency Metric**

**Trace Data:**
```json
{
  "traceId": "abc123def456",
  "spans": [
    {"service": "PublisherService", "duration": 50, "parent": null},
    {"service": "ProfanityService", "duration": 20, "parent": "span1"},
    {"service": "ArticleService", "duration": 100, "parent": "span1"}
  ]
}
```

**Metric:**
```promql
# Cross-service latency
sum(span_duration_seconds{service="publisher",operation="Publish"}) by (traceId)
```

**Værdi:**
- ✅ Identificer bottlenecks i service chain
- ✅ Optimize slow services
- ✅ Understand service dependencies

**Eksempel fra kodebasen:**
```csharp
// PublisherService → ProfanityService → ArticleService
// Trace viser hele flowet
using var activity = ActivitySrc.StartActivity("Publish Article", ActivityKind.Producer);
```

### Kombinerede Metrics fra Logs og Traces:

#### 1. **Request Success Rate**

**Fra Logs:**
```promql
# Success rate fra logs
sum(rate(request_logs_total{status="200"}[5m])) / sum(rate(request_logs_total[5m]))
```

**Fra Traces:**
```promql
# Success rate fra traces
sum(rate(spans_total{status="OK"}[5m])) / sum(rate(spans_total[5m]))
```

**Værdi:**
- ✅ System health indicator
- ✅ SLA compliance
- ✅ Alerting threshold

#### 2. **Service Dependency Graph**

**Fra Traces:**
```promql
# Service calls (fra traces)
sum(rate(spans_total{parent!=""}[5m])) by (service, parent_service)
```

**Værdi:**
- ✅ Visualize service dependencies
- ✅ Identify critical paths
- ✅ Understand system architecture

#### 3. **Cache Performance**

**Fra Logs:**
```promql
# Cache hit rate (fra logs)
sum(rate(cache_hit_logs_total[5m])) / (sum(rate(cache_hit_logs_total[5m])) + sum(rate(cache_miss_logs_total[5m])))
```

**Fra Metrics (direkte):**
```csharp
// ArticleService - Allerede implementeret
var articleCacheHits = Metrics.CreateCounter("article_cache_hits_total", "Article cache hits");
var articleCacheMisses = Metrics.CreateCounter("article_cache_misses_total", "Article cache misses");
```

**Værdi:**
- ✅ Cache effectiveness
- ✅ Performance optimization
- ✅ Cost optimization (færre database queries)

### Anbefalede Metrics fra Logs/Traces:

| Metric | Kilde | Værdi |
|--------|-------|-------|
| **Error Rate** | Logs (error level) | System health, alerting |
| **Request Rate** | Logs (request entries) | Throughput, capacity planning |
| **Response Time (p95, p99)** | Traces (span duration) | SLA monitoring, performance |
| **Cache Hit Rate** | Logs (cache events) | Performance optimization |
| **Database Query Time** | Logs (slow queries) | Database optimization |
| **Service Dependency Calls** | Traces (span relationships) | Architecture understanding |
| **Cross-Service Latency** | Traces (parent-child spans) | Bottleneck identification |

### Konklusion:

**Logs og Traces kan generere nyttige metrics:**
- ✅ **Error Rate**: Fra error logs → Alerting
- ✅ **Request Rate**: Fra request logs → Capacity planning
- ✅ **Response Time**: Fra trace spans → SLA monitoring
- ✅ **Cache Performance**: Fra cache logs → Optimization
- ✅ **Service Dependencies**: Fra trace relationships → Architecture insights

**Best Practice:**
- Brug direkte metrics når muligt (hurtigere, mindre overhead)
- Brug logs/traces → metrics for ad-hoc analysis
- Kombiner alle tre (metrics, logs, traces) for fuld observability
