# Google Slides Content - Spørgsmål 8: Design to be monitored
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Design to be monitored
**Undertitel:** Metrics, logging og tracing i distribueret system
**Footer:** Dit navn | Dato

---

## SLIDE 2: "Design to be monitored"-princippet
**Titel:** "Design to be monitored"-princippet

**Indhold:**
- **Definition:** Systemet skal designes fra starten med monitoring i tankerne
- Monitoring er en first-class citizen i arkitekturen
- Ikke tilføjes som eftertanke

---

## SLIDE 3: Principper
**Titel:** Principper

**Indhold:**
- **Instrumentation by Design:** Alle kritiske operationer skal instrumenteres
- **Observability:** Systemet skal være observerbart (kan forstås uden at kende intern implementation)
- **Proactive Monitoring:** Identificer problemer før de påvirker brugere
- **Distributed Context:** I microservices skal man kunne følge requests på tværs af services

---

## SLIDE 4: Metrics - Definition
**Titel:** Metrics - Definition

**Indhold:**
- **Definition:** Numeriske værdier der måler systemets tilstand over tid
- Aggregeret data
- **Format:** Numeriske værdier (counters, gauges, histograms)
- **Volume:** Lav (samlet data)
- **Retention:** Lang (måneder/år)

---

## SLIDE 5: Metrics - Eksempel fra kodebasen
**Titel:** Metrics - Eksempel fra kodebasen

**Indhold:**
- **Fil:** `ArticleService/Program.cs`
- **Kode:**
  ```csharp
  var articleCacheHits = Metrics.CreateCounter("article_cache_hits_total", "Article cache hits");
  var articleCacheMisses = Metrics.CreateCounter("article_cache_misses_total", "Article cache misses");
  ```
- **Eksempler:** Cache hit rate: 80%, Request rate: 1000 req/s

**Screenshot:** Vis linje 36-37 (Metrics.CreateCounter)

---

## SLIDE 6: Metrics - Brug i Controller
**Titel:** Metrics - Brug i Controller

**Indhold:**
- **Fil:** `ArticleService/Controllers/ArticlesController.cs`
- **Kode:**
  ```csharp
  if (cached.HasValue) {
      _hits.Inc();  // Increment counter
  } else {
      _misses.Inc();  // Increment counter
  }
  ```
- **Resultat:** Metrics opdateres i runtime

**Screenshot:** Vis linje 33-38 (cache hit) og 47-49 (cache miss)

---

## SLIDE 7: Logging - Definition
**Titel:** Logging - Definition

**Indhold:**
- **Definition:** Strukturerede events og beskeder der beskriver hvad der sker
- Diskret data
- **Format:** Strukturerede events (JSON, key-value pairs)
- **Volume:** Høj (mange log entries)
- **Retention:** Kort-mellemlang (dage/uger)

---

## SLIDE 8: Logging - Eksempel fra kodebasen
**Titel:** Logging - Eksempel fra kodebasen

**Indhold:**
- **Fil:** `DraftService/Program.cs`
- **Kode:**
  ```csharp
  Log.Logger = new LoggerConfiguration()
      .Enrich.FromLogContext()
      .WriteTo.Console()
      .WriteTo.Seq(serverUrl: seqUrl)  // Centralized logging
      .CreateLogger();
  ```
- **Eksempler:** "ArticleService started", "Cache miss for key", "Database connection failed"

**Screenshot:** Vis linje 9-14 (Serilog konfiguration)

---

## SLIDE 9: Logging - Correlation ID
**Titel:** Logging - Correlation ID

**Indhold:**
- **Fil:** `DraftService/Middleware/CorrelationIdMiddleware.cs`
- **Formål:** Correlation ID middleware for distributed context
- **Resultat:** Kan følge requests på tværs af services

**Screenshot:** Vis hele filen (CorrelationIdMiddleware)

---

## SLIDE 10: Tracing - Definition
**Titel:** Tracing - Definition

**Indhold:**
- **Definition:** Spores en enkelt request gennem hele systemet
- Kontekstuel data
- **Format:** Spans med parent-child relationships
- **Volume:** Mellem (per request)
- **Retention:** Kort (timer/dage)

---

## SLIDE 11: Tracing - Eksempel fra kodebasen
**Titel:** Tracing - Eksempel fra kodebasen

**Indhold:**
- **Fil:** `ArticleService/Services/ArticleQueueConsumer.cs`
- **Kode:**
  ```csharp
  private static readonly ActivitySource ActivitySrc = new("ArticleService");
  var parentContext = Propagator.Extract(...);
  using var activity = ActivitySrc.StartActivity("Consume Article", ...);
  ```
- **Eksempel:** Trace ID: abc123def456, spans gennem flere services

**Screenshot:** Vis linje 19, 41-49 (ActivitySource og trace context extraction)

---

## SLIDE 12: Tracing - Context Propagation
**Titel:** Tracing - Context Propagation

**Indhold:**
- **Fil:** `PublisherService/Services/ArticleQueuePublisher.cs`
- **Kode:**
  ```csharp
  using var activity = ActivitySrc.StartActivity("Publish Article", ActivityKind.Producer);
  Propagator.Inject(..., props.Headers, ...);
  ```
- **Resultat:** Trace context propagere gennem message queue

**Screenshot:** Vis linje 40, 48-52 (activity start og context injection)

---

## SLIDE 13: Sammenligning - Tabel
**Titel:** Sammenligning - Tabel

**Indhold:**
| Aspekt | Metrics | Logging | Tracing |
|--------|---------|---------|---------|
| Data Type | Numerisk (aggregated) | Events (discrete) | Spans (contextual) |
| Volume | Lav | Høj | Mellem |
| Retention | Lang (måneder) | Kort (dage) | Kort (timer) |
| Purpose | Trends, alerting | Debugging, audit | Request flow |
| Query | Aggregation | Search, filter | Trace by ID |

---

## SLIDE 14: Kombineret Approach
**Titel:** Kombineret Approach

**Indhold:**
- **Alle tre typer bruges sammen:**
  - Metrics (Prometheus): `_hits.Inc()`
  - Logging (Serilog/Seq): `_logger.LogInformation(...)`
  - Tracing (OpenTelemetry/Jaeger): `ActivitySrc.StartActivity(...)`

---

## SLIDE 15: Y-akse Skalering - Definition
**Titel:** Y-akse Skalering - Definition

**Indhold:**
- **Definition:** Split systemet efter funktionalitet → Microservices
- **Eksempel:** ArticleService, CommentService, ProfanityService, etc.
- **Resultat:** 6 separate services i stedet for 1 monolit

---

## SLIDE 16: Problem 1 - Distributed Metrics Aggregation
**Titel:** Problem 1 - Distributed Metrics Aggregation

**Indhold:**
- **Før:** 1 Prometheus scrape target → Simpel query: `sum(requests_total)`
- **Efter:** 6 separate scrape targets → Kompleks query: `sum(requests_total{service="article"}) + sum(requests_total{service="comment"}) + ...`
- **Fil:** `docs/prometheus.yml`

**Screenshot:** Vis linje 4-18 (scrape_configs for alle services)

---

## SLIDE 17: Problem 2 - Inconsistent Metric Names
**Titel:** Problem 2 - Inconsistent Metric Names

**Indhold:**
- **Før:** `cache_hits_total` (én metric)
- **Efter:** `article_cache_hits_total`, `comment_cache_hits_total` (forskellige navne)
- **Problem:** Skal query flere metrics og aggregere

---

## SLIDE 18: Problem 3 - Distributed Tracing Complexity
**Titel:** Problem 3 - Distributed Tracing Complexity

**Indhold:**
- **Før:** 1 trace, 1 span → Simpel: Alt sker i samme process
- **Efter:** 4 services, 6+ spans, distributed context → Trace context skal propagere gennem HTTP, RabbitMQ
- **Filer:** `PublisherService/Services/ArticleQueuePublisher.cs` og `ArticleService/Services/ArticleQueueConsumer.cs`

**Screenshot:** Vis context injection og extraction

---

## SLIDE 19: Problem 4 - Centralized Logging Aggregation
**Titel:** Problem 4 - Centralized Logging Aggregation

**Indhold:**
- **Før:** 1 log stream → Simpel: Alle logs samme sted
- **Efter:** 6 separate log streams → Kompleks: Skal aggregere fra flere kilder
- **Fil:** `DraftService/Program.cs` (Seq centralized logging)

**Screenshot:** Vis linje 9-14 (Seq logging)

---

## SLIDE 20: Problem 5 - Service Discovery for Monitoring
**Titel:** Problem 5 - Service Discovery for Monitoring

**Indhold:**
- **Før:** 1 target → Statisk konfiguration
- **Efter:** 6+ targets → Dynamisk service discovery nødvendig
- **Fil:** `docs/prometheus.yml` (flere instances)

**Screenshot:** Vis linje 6-7 (flere article-service instances)

---

## SLIDE 21: Problem 6 - Cross-Service Metrics Correlation
**Titel:** Problem 6 - Cross-Service Metrics Correlation

**Indhold:**
- **Før:** `request_duration_seconds` viser total tid
- **Efter:** Skal aggregere `publisher_request_duration + profanity_request_duration + article_request_duration`
- **Tracing nødvendig:** For at forstå hvor tid går

---

## SLIDE 22: Løsninger
**Titel:** Løsninger

**Indhold:**
- **Standardiserede Metric Names:** `{service="article",metric="cache_hits_total"}`
- **Distributed Tracing (OpenTelemetry):** Alle services bruger OpenTelemetry
- **Centralized Logging (Seq):** Alle services sender logs til Seq
- **Service Discovery:** Kubernetes service discovery for Prometheus

---

## SLIDE 23: Data fra Logs → Metrics
**Titel:** Data fra Logs → Metrics

**Indhold:**
- **Error Logs → Error Rate Metric:** `rate(error_logs_total{service="article"}[5m])`
- **Request Logs → Request Rate Metric:** `rate(request_logs_total{service="article"}[5m])`
- **Cache Logs → Cache Hit Rate Metric:** `rate(cache_hits_total[5m]) / (rate(cache_hits_total[5m]) + rate(cache_misses_total[5m]))`
- **Database Query Logs → Database Performance Metric:** `rate(slow_query_logs_total[5m])`

---

## SLIDE 24: Data fra Traces → Metrics
**Titel:** Data fra Traces → Metrics

**Indhold:**
- **Span Duration → Response Time Metric:** `histogram_quantile(0.95, rate(span_duration_seconds_bucket[5m]))`
- **Span Count → Request Rate Metric:** `rate(spans_total{service="article"}[5m])`
- **Error Spans → Error Rate Metric:** `rate(error_spans_total[5m]) / rate(spans_total[5m])`
- **Cross-Service Latency → Service Dependency Metric:** `sum(span_duration_seconds{service="publisher"}) by (traceId)`

---

## SLIDE 25: Kombinerede Metrics
**Titel:** Kombinerede Metrics

**Indhold:**
- **Request Success Rate:** Fra logs eller traces
- **Service Dependency Graph:** Fra traces (span relationships)
- **Cache Performance:** Fra logs eller direkte metrics (allerede implementeret i ArticleService)

---

## SLIDE 26: Anbefalede Metrics fra Logs/Traces
**Titel:** Anbefalede Metrics fra Logs/Traces

**Indhold:**
| Metric | Kilde | Værdi |
|--------|-------|-------|
| Error Rate | Logs (error level) | System health, alerting |
| Request Rate | Logs (request entries) | Throughput, capacity planning |
| Response Time (p95, p99) | Traces (span duration) | SLA monitoring, performance |
| Cache Hit Rate | Logs (cache events) | Performance optimization |
| Database Query Time | Logs (slow queries) | Database optimization |
| Service Dependency Calls | Traces (span relationships) | Architecture understanding |
| Cross-Service Latency | Traces (parent-child spans) | Bottleneck identification |

---

## SLIDE 27: Konklusion
**Titel:** Konklusion

**Indhold:**
- Logs og Traces kan generere nyttige metrics
- **Error Rate:** Fra error logs → Alerting
- **Request Rate:** Fra request logs → Capacity planning
- **Response Time:** Fra trace spans → SLA monitoring
- **Cache Performance:** Fra cache logs → Optimization
- **Service Dependencies:** Fra trace relationships → Architecture insights
- **Best Practice:** Brug direkte metrics når muligt, brug logs/traces → metrics for ad-hoc analysis, kombiner alle tre for fuld observability

---

## SLIDE 28: Spørgsmål?
**Titel:** Spørgsmål?

**Indhold:**
- Dit navn
- Kontaktinfo (hvis relevant)

---

## INSTRUKTIONER TIL GOOGLE SLIDES:

1. **Opret ny Google Slides præsentation**
2. **For hver slide:**
   - Kopier titlen som slide-titel
   - Kopier indholdet som slide-body
   - Tilføj screenshots hvor angivet
3. **Design forslag:**
   - Brug konsistent farveskema
   - Brug store, læsbare fonte
   - Tilføj kode-snippets som billeder eller tekstbokse
4. **Screenshots:**
   - Tag screenshots af de angivne filer
   - Indsæt som billeder på relevante slides
   - Tilføj annotations hvis nødvendigt
5. **PromQL queries:**
   - Slide 23-24: Vis PromQL queries som kode-blokke
   - Brug monospace font for queries
