# EKSAMEN - Spørgsmål 8: Design to be monitored
## Slide Outline & Speaking Notes

---

## SLIDE 1: Titel
**Speaking Notes:**
- "Jeg vil i dag præsentere Design to be monitored"
- "Jeg dækker tre dele: princippet og forskellen på metrics/logging/tracing, problemer ved Y-akse skalering, og hvilke data fra logs/traces der kan bruges som metrics"
- "Jeg bruger Happy-Headlines projektet til at illustrere det"

---

## PART A: BESKRIV PRINCIPPET

---

## SLIDE 2: "Design to be monitored"-princippet
**Speaking Notes:**
- "Design to be monitored-princippet"
- "Definition: Systemet skal designes fra starten med monitoring i tankerne, ikke tilføjes som eftertanke"
- "Monitoring er en first-class citizen i arkitekturen"
- "Dette betyder at monitoring er en del af designet, ikke noget man tilføjer bagefter"

---

## SLIDE 3: Principper
**Speaking Notes:**
- "Principper:"
- "Instrumentation by Design: Alle kritiske operationer skal instrumenteres"
- "Observability: Systemet skal være observerbart - kan forstås uden at kende intern implementation"
- "Proactive Monitoring: Identificer problemer før de påvirker brugere"
- "Distributed Context: I microservices skal man kunne følge requests på tværs af services"
- "Dette er særligt vigtigt i microservices-arkitektur"

---

## SLIDE 4: Metrics - Definition
**Speaking Notes:**
- "Første type: Metrics"
- "Definition: Numeriske værdier der måler systemets tilstand over tid"
- "Aggregeret data - samlet data"
- "Format: Numeriske værdier - counters, gauges, histograms"
- "Volume: Lav - samlet data"
- "Retention: Lang - måneder eller år"

---

## SLIDE 5: Metrics - Eksempel fra kodebasen
**Speaking Notes:**
- "Eksempel fra kodebasen:"
- "I ArticleService/Program.cs ser vi at vi opretter metrics for cache hits og misses"
- "article_cache_hits_total og article_cache_misses_total"
- "Dette bruges til at måle cache performance"
- "Eksempler: Cache hit rate 80%, Request rate 1000 req/s, Error rate 0.5%, Response time p95 50ms"

**Screenshot Instructions:**
1. Åbn `ArticleService/Program.cs`
2. Marker linje 36-37 (Metrics.CreateCounter)
3. Vis at dette er instrumentation by design

---

## SLIDE 6: Metrics - Brug i Controller
**Speaking Notes:**
- "Brug i Controller:"
- "I ArticlesController ser vi at vi incrementer metrics når cache hit eller miss"
- "_hits.Inc() når cache hit, _misses.Inc() når cache miss"
- "Dette er instrumentation by design - metrics opdateres automatisk"
- "Resultat: Vi kan måle cache performance i real-time"

**Screenshot Instructions:**
1. Åbn `ArticleService/Controllers/ArticlesController.cs`
2. Marker linje 33-38 (cache hit) og 47-49 (cache miss)
3. Vis _hits.Inc() og _misses.Inc() calls

---

## SLIDE 7: Logging - Definition
**Speaking Notes:**
- "Anden type: Logging"
- "Definition: Strukturerede events og beskeder der beskriver hvad der sker i systemet"
- "Diskret data - individuelle events"
- "Format: Strukturerede events - JSON, key-value pairs"
- "Volume: Høj - mange log entries"
- "Retention: Kort-mellemlang - dage eller uger"

---

## SLIDE 8: Logging - Eksempel fra kodebasen
**Speaking Notes:**
- "Eksempel fra kodebasen:"
- "I DraftService/Program.cs ser vi Serilog konfiguration"
- "WriteTo.Seq - centralized logging til Seq server"
- "Dette er design to be monitored - logging er konfigureret fra starten"
- "Eksempler: 'ArticleService started', 'Cache miss for key', 'Database connection failed'"

**Screenshot Instructions:**
1. Åbn `DraftService/Program.cs`
2. Marker linje 9-14 (Serilog konfiguration)
3. Vis WriteTo.Seq for centralized logging

---

## SLIDE 9: Logging - Correlation ID
**Speaking Notes:**
- "Correlation ID Middleware:"
- "I DraftService/Middleware/CorrelationIdMiddleware.cs ser vi correlation ID middleware"
- "Dette tilføjer correlation ID til alle requests"
- "Resultat: Vi kan følge requests på tværs af services i logs"
- "Dette er distributed context - vigtigt for microservices"

**Screenshot Instructions:**
1. Åbn `DraftService/Middleware/CorrelationIdMiddleware.cs`
2. Marker hele filen
3. Forklar at dette tilføjer correlation ID til logs

---

## SLIDE 10: Tracing - Definition
**Speaking Notes:**
- "Tredje type: Tracing"
- "Definition: Spores en enkelt request gennem hele systemet på tværs af services"
- "Kontekstuel data - viser sammenhæng"
- "Format: Spans med parent-child relationships"
- "Volume: Mellem - per request"
- "Retention: Kort - timer eller dage"

---

## SLIDE 11: Tracing - Eksempel fra kodebasen
**Speaking Notes:**
- "Eksempel fra kodebasen:"
- "I ArticleService/Services/ArticleQueueConsumer.cs ser vi OpenTelemetry tracing"
- "ActivitySource oprettes for at starte spans"
- "Trace context propagation - extract fra RabbitMQ headers"
- "Dette viser distributed tracing - kan følge request gennem flere services"
- "Eksempel: Trace ID abc123def456, spans gennem PublisherService → RabbitMQ → ArticleService → Database"

**Screenshot Instructions:**
1. Åbn `ArticleService/Services/ArticleQueueConsumer.cs`
2. Marker linje 19 (ActivitySource), 41-49 (trace context extraction og activity start)
3. Forklar distributed tracing

---

## SLIDE 12: Tracing - Context Propagation
**Speaking Notes:**
- "Trace Context Propagation:"
- "I PublisherService/Services/ArticleQueuePublisher.cs ser vi trace context injection"
- "Propagator.Inject - injecter trace context i RabbitMQ headers"
- "Dette sikrer at trace context propagere gennem message queue"
- "Resultat: Vi kan følge request fra PublisherService → RabbitMQ → ArticleService"
- "Dette er distributed context - vigtigt for microservices"

**Screenshot Instructions:**
1. Åbn `PublisherService/Services/ArticleQueuePublisher.cs`
2. Marker linje 40 (activity start), 48-52 (context injection)
3. Forklar W3C Trace Context propagation

---

## SLIDE 13: Sammenligning - Tabel
**Speaking Notes:**
- "Sammenligning:"
- "Data Type: Metrics er numerisk - aggregated, Logging er events - discrete, Tracing er spans - contextual"
- "Volume: Metrics er lav, Logging er høj, Tracing er mellem"
- "Retention: Metrics er lang - måneder, Logging er kort - dage, Tracing er kort - timer"
- "Purpose: Metrics for trends og alerting, Logging for debugging og audit, Tracing for request flow"
- "Query: Metrics bruger aggregation, Logging bruger search/filter, Tracing bruger trace by ID"

---

## SLIDE 14: Kombineret Approach
**Speaking Notes:**
- "Kombineret Approach:"
- "Alle tre typer bruges sammen i Happy-Headlines"
- "Metrics (Prometheus): _hits.Inc() - for trends og alerting"
- "Logging (Serilog/Seq): _logger.LogInformation(...) - for debugging og audit"
- "Tracing (OpenTelemetry/Jaeger): ActivitySrc.StartActivity(...) - for request flow"
- "Dette giver fuld observability - kan forstå systemet fra alle vinkler"

---

## PART B: Y-AKSE SKALERING PROBLEMER

---

## SLIDE 15: Y-akse Skalering - Definition
**Speaking Notes:**
- "Y-akse Skalering:"
- "Definition: Split systemet efter funktionalitet → Microservices"
- "Eksempel: ArticleService, CommentService, ProfanityService, PublisherService, DraftService, NewsletterService"
- "Resultat: 6 separate services i stedet for 1 monolitisk applikation"
- "Dette skaber problemer for monitoring"

---

## SLIDE 16: Problem 1 - Distributed Metrics Aggregation
**Speaking Notes:**
- "Problem 1: Distributed Metrics Aggregation"
- "Før Y-akse: 1 Prometheus scrape target, simpel query sum(requests_total)"
- "Efter Y-akse: 6 separate scrape targets, kompleks query skal aggregere fra flere services"
- "I prometheus.yml ser vi at hver service har sit eget scrape target"
- "Dette gør queries mere komplekse - skal aggregere fra flere kilder"

**Screenshot Instructions:**
1. Åbn `docs/prometheus.yml`
2. Marker linje 4-18 (scrape_configs for alle services)
3. Vis at hver service har sit eget target

---

## SLIDE 17: Problem 2 - Inconsistent Metric Names
**Speaking Notes:**
- "Problem 2: Inconsistent Metric Names"
- "Før Y-akse: cache_hits_total - én metric"
- "Efter Y-akse: article_cache_hits_total, comment_cache_hits_total - forskellige navne"
- "Dette gør queries komplekse - skal query flere metrics og aggregere"
- "Løsning: Standardiseret naming convention med service labels"

---

## SLIDE 18: Problem 3 - Distributed Tracing Complexity
**Speaking Notes:**
- "Problem 3: Distributed Tracing Complexity"
- "Før Y-akse: 1 trace, 1 span - alt sker i samme process"
- "Efter Y-akse: 4 services, 6+ spans, distributed context"
- "Trace context skal propagere gennem HTTP, RabbitMQ, etc."
- "I PublisherService injecter vi trace context i RabbitMQ headers"
- "I ArticleService extracter vi trace context fra RabbitMQ headers"
- "Dette er komplekst - W3C Trace Context propagation, correlation IDs"

**Screenshot Instructions:**
1. Vis `PublisherService/Services/ArticleQueuePublisher.cs` linje 48-52 (context injection)
2. Vis `ArticleService/Services/ArticleQueueConsumer.cs` linje 41-49 (context extraction)
3. Forklar distributed tracing flow

---

## SLIDE 19: Problem 4 - Centralized Logging Aggregation
**Speaking Notes:**
- "Problem 4: Centralized Logging Aggregation"
- "Før Y-akse: 1 log stream - alle logs samme sted"
- "Efter Y-akse: 6 separate log streams - skal aggregere fra flere kilder"
- "I DraftService ser vi centralized logging til Seq"
- "Dette kræver correlation IDs for at følge requests på tværs af services"
- "Kompleksitet: Skal aggregere logs fra flere services"

**Screenshot Instructions:**
1. Åbn `DraftService/Program.cs`
2. Marker linje 9-14 (Seq logging)
3. Forklar centralized logging

---

## SLIDE 20: Problem 5 - Service Discovery for Monitoring
**Speaking Notes:**
- "Problem 5: Service Discovery for Monitoring"
- "Før Y-akse: 1 target - statisk konfiguration"
- "Efter Y-akse: 6+ targets - dynamisk service discovery nødvendig"
- "I prometheus.yml ser vi flere instances af samme service"
- "Når services skaleres (X-akse), skal Prometheus opdage nye instances"
- "Dette kræver service discovery - Kubernetes, Consul, etc."

**Screenshot Instructions:**
1. Åbn `docs/prometheus.yml`
2. Marker linje 6-7 (flere article-service instances)
3. Forklar service discovery behov

---

## SLIDE 21: Problem 6 - Cross-Service Metrics Correlation
**Speaking Notes:**
- "Problem 6: Cross-Service Metrics Correlation"
- "Før Y-akse: request_duration_seconds viser total tid - simpel"
- "Efter Y-akse: Skal aggregere publisher_request_duration + profanity_request_duration + article_request_duration"
- "Kompleks: Skal aggregere fra flere services"
- "Tracing nødvendig: For at forstå hvor tid går i hver service"
- "Dette viser behovet for distributed tracing"

---

## SLIDE 22: Løsninger
**Speaking Notes:**
- "Løsninger til Y-akse Monitoring Problemer:"
- "Standardiserede Metric Names: Brug service labels i stedet for forskellige navne"
- "Distributed Tracing (OpenTelemetry): Alle services bruger OpenTelemetry for tracing"
- "Centralized Logging (Seq): Alle services sender logs til Seq for aggregation"
- "Service Discovery: Kubernetes service discovery for Prometheus til at opdage nye instances"
- "Dette løser problemerne, men tilføjer kompleksitet"

---

## PART C: DATA FRA LOGS OG TRACES → METRICS

---

## SLIDE 23: Data fra Logs → Metrics
**Speaking Notes:**
- "Data fra Logs → Metrics:"
- "Error Logs → Error Rate Metric: rate(error_logs_total{service="article"}[5m])"
- "Værdi: Proaktiv alerting ved høj error rate, trend analysis, service comparison"
- "Request Logs → Request Rate Metric: rate(request_logs_total{service="article"}[5m])"
- "Værdi: Throughput monitoring, traffic pattern analysis, capacity planning"
- "Cache Logs → Cache Hit Rate Metric: rate(cache_hits_total[5m]) / (rate(cache_hits_total[5m]) + rate(cache_misses_total[5m]))"
- "Værdi: Cache performance monitoring, identificer cache issues, optimize cache strategy"
- "Database Query Logs → Database Performance Metric: rate(slow_query_logs_total[5m])"
- "Værdi: Database performance monitoring, identificer N+1 queries, optimize database queries"

---

## SLIDE 24: Data fra Traces → Metrics
**Speaking Notes:**
- "Data fra Traces → Metrics:"
- "Span Duration → Response Time Metric: histogram_quantile(0.95, rate(span_duration_seconds_bucket[5m]))"
- "Værdi: SLA monitoring (p95, p99 response times), performance degradation detection, service comparison"
- "Span Count → Request Rate Metric: rate(spans_total{service="article"}[5m])"
- "Værdi: Throughput monitoring, request pattern analysis, load balancing validation"
- "Error Spans → Error Rate Metric: rate(error_spans_total[5m]) / rate(spans_total[5m])"
- "Værdi: Error rate monitoring, service health indicators, alerting thresholds"
- "Cross-Service Latency → Service Dependency Metric: sum(span_duration_seconds{service="publisher"}) by (traceId)"
- "Værdi: Identificer bottlenecks i service chain, optimize slow services, understand service dependencies"

---

## SLIDE 25: Kombinerede Metrics
**Speaking Notes:**
- "Kombinerede Metrics:"
- "Request Success Rate: Fra logs eller traces - system health indicator, SLA compliance, alerting threshold"
- "Service Dependency Graph: Fra traces (span relationships) - visualize service dependencies, identify critical paths, understand system architecture"
- "Cache Performance: Fra logs eller direkte metrics - allerede implementeret i ArticleService med Prometheus counters"
- "Dette viser at logs og traces kan generere nyttige metrics"

---

## SLIDE 26: Anbefalede Metrics fra Logs/Traces
**Speaking Notes:**
- "Anbefalede Metrics fra Logs/Traces:"
- "Error Rate: Fra logs (error level) - system health, alerting"
- "Request Rate: Fra logs (request entries) - throughput, capacity planning"
- "Response Time (p95, p99): Fra traces (span duration) - SLA monitoring, performance"
- "Cache Hit Rate: Fra logs (cache events) - performance optimization"
- "Database Query Time: Fra logs (slow queries) - database optimization"
- "Service Dependency Calls: Fra traces (span relationships) - architecture understanding"
- "Cross-Service Latency: Fra traces (parent-child spans) - bottleneck identification"

---

## SLIDE 27: Konklusion
**Speaking Notes:**
- "Konklusion:"
- "Logs og Traces kan generere nyttige metrics for overvågning af distribueret system"
- "Error Rate: Fra error logs → Alerting"
- "Request Rate: Fra request logs → Capacity planning"
- "Response Time: Fra trace spans → SLA monitoring"
- "Cache Performance: Fra cache logs → Optimization"
- "Service Dependencies: Fra trace relationships → Architecture insights"
- "Best Practice: Brug direkte metrics når muligt - hurtigere, mindre overhead. Brug logs/traces → metrics for ad-hoc analysis. Kombiner alle tre - metrics, logs, traces - for fuld observability"

---

## SLIDE 28: Spørgsmål?
**Speaking Notes:**
- "Tak for jeres opmærksomhed"
- "Jeg er klar til spørgsmål"

---

## EKSTRA NOTER TIL EKSAMEN:

### Hvis de spørger om specifikke filer:
- **`ArticleService/Program.cs`**: Metrics oprettelse (linje 36-37)
- **`ArticleService/Controllers/ArticlesController.cs`**: Metrics brug (linje 33-38, 47-49)
- **`DraftService/Program.cs`**: Logging konfiguration (linje 9-14)
- **`DraftService/Middleware/CorrelationIdMiddleware.cs`**: Correlation ID middleware
- **`ArticleService/Services/ArticleQueueConsumer.cs`**: Distributed tracing (linje 19, 41-49)
- **`PublisherService/Services/ArticleQueuePublisher.cs`**: Trace context propagation (linje 40, 48-52)
- **`docs/prometheus.yml`**: Prometheus konfiguration med flere services

### Hvis de spørger om metrics:
- Cache hit rate: 80%
- Request rate: 1000 req/s
- Error rate: 0.5%
- Response time (p95): 50ms

### Hvis de spørger om logging:
- Serilog med Seq (centralized logging)
- Correlation ID middleware for distributed context
- Strukturerede logs (JSON format)

### Hvis de spørger om tracing:
- OpenTelemetry med ActivitySource
- W3C Trace Context propagation gennem RabbitMQ
- Distributed tracing gennem flere services

### Hvis de spørger om Y-akse problemer:
- Distributed metrics aggregation (6 scrape targets)
- Inconsistent metric names (forskellige navne per service)
- Distributed tracing complexity (trace context propagation)
- Centralized logging aggregation (6 log streams)
- Service discovery for monitoring (dynamisk discovery nødvendig)
- Cross-service metrics correlation (skal aggregere fra flere services)

### Hvis de spørger om logs/traces → metrics:
- Error logs → Error rate metric
- Request logs → Request rate metric
- Cache logs → Cache hit rate metric
- Span duration → Response time metric
- Error spans → Error rate metric
- Cross-service latency → Service dependency metric
