# Spørgsmål 10: Availability

## A. Forklar begrebet availability og hvordan det relaterer sig til skaleringsprincipper og skaleringskuben

### Availability - Definition:

**Availability** er procentdelen af tid hvor systemet er operationelt og kan håndtere requests. Måles typisk som:

```
Availability = (Total Time - Downtime) / Total Time × 100%
```

**Eksempler:**
- **99% availability** = 3.65 dages downtime/år
- **99.9% availability** = 8.76 timer downtime/år
- **99.99% availability** = 52.56 minutter downtime/år
- **99.999% availability** = 5.26 minutter downtime/år

### Relation til Skaleringsprincipper:

#### 1. **Performance Skalering → Availability**

**Princippet:**
- Højere performance = Bedre availability
- Systemet kan håndtere mere trafik uden at gå ned

**Eksempel:**
- **Lav performance**: System går ned ved 1000 req/s → 0% availability
- **Høj performance**: System håndterer 5000 req/s → 99.9% availability

#### 2. **Cost Skalering → Availability**

**Princippet:**
- Invester i redundancy = Højere availability
- Backup systems, failover mechanisms

**Eksempel:**
- **Lav cost**: 1 server → Hvis server fejler = 0% availability
- **Høj cost**: 3 servers (redundancy) → Hvis 1 fejler = 99%+ availability

#### 3. **People Skalering → Availability**

**Princippet:**
- Automatisering = Konsistent availability
- Færre menneskelige fejl = Højere availability

**Eksempel:**
- **Manuel deployment**: Fejl-risiko → 95% availability
- **Automatiseret deployment**: Konsistent → 99.9% availability

### Relation til Skaleringskuben:

#### X-akse (Horizontal Duplication) → Availability

**Princippet:**
- Flere instanser = Redundancy
- Hvis én instance fejler, kan andre fortsætte

**Availability Impact:**
```
1 instance: Availability = 99% (1% downtime)
3 instances: Availability = 99.999% (hvis 1 fejler, 2 andre fortsætter)
```

**Eksempel fra kodebasen:**
```yaml
# docker-compose.yml (linje 8-9)
deploy:
  replicas: 1  # Kun 1 replica (for Prometheus)
  # Med flere replicas: Højere availability
```

**Forbedring:**
- **1 replica**: Hvis container fejler → 0% availability
- **3 replicas**: Hvis 1 container fejler → 66% availability (2/3 fortsætter)

#### Y-akse (Functional Decomposition) → Availability

**Princippet:**
- Microservices = Isolation
- Fejl i én service påvirker ikke andre

**Availability Impact:**
```
Monolitisk: Hvis én del fejler → Hele systemet nede → 0% availability
Microservices: Hvis ArticleService fejler → Kommentarer fortsætter → 80% availability
```

**Eksempel fra kodebasen:**
```
Happy-Headlines (6 services):
- ArticleService fejler → Kommentarer, Newsletter fortsætter
- ProfanityService fejler → Fallback aktiveres → Kommentarer fortsætter
```

**Forbedring:**
- **Monolitisk**: 1 fejl = 0% availability
- **Microservices**: 1 fejl = 80-90% availability (andre services fortsætter)

#### Z-akse (Data Partitioning) → Availability

**Princippet:**
- Flere databases = Redundancy
- Hvis én database fejler, kan andre fortsætte

**Availability Impact:**
```
1 database: Hvis database fejler → 0% availability
8 databases (Z-akse): Hvis 1 database fejler → 87.5% availability (7/8 fortsætter)
```

**Eksempel fra kodebasen:**
```yaml
# docker-compose.yml (linje 20-69)
# 8 databases (global + 7 kontinenter)
article-db-global: ...
article-db-europe: ...
article-db-asia: ...
# ... osv
```

**Forbedring:**
- **1 database**: 1 fejl = 0% availability
- **8 databases**: 1 fejl = 87.5% availability (andre regions fortsætter)

### Kombineret Skalering → Availability:

**X + Y + Z akser kombineret:**
- **X-akse**: 3 replicas per service
- **Y-akse**: 6 services (isolation)
- **Z-akse**: 8 databases (geografisk)

**Availability beregning:**
```
Service availability = 99% (per service)
With 3 replicas (X-akse): 99.999% (redundancy)
With 6 services (Y-akse): Isolation → 1 service fejl = 83% availability
With 8 databases (Z-akse): 1 DB fejl = 87.5% availability

Total system availability ≈ 99.9%+
```

---

## B. Vis eksempler på mekanismer til forbedring af availability i et system der presses af stigende traffik

### Problemstilling:

**System presses af stigende trafik:**
- 100 req/s → 1000 req/s (10x stigning)
- Systemet går ned ved høj trafik
- Availability falder fra 99% → 50%

### Mekanismer til Forbedring:

#### Mekanisme 1: Circuit Breaker (Isolerer Fejlende Services)

**Kodeeksempel:**
```csharp
// CommentService/Program.cs (linje 28-30)
var breaker = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(20)
    );
```

**Availability forbedring:**
- **Før**: ProfanityService fejler → CommentService blokeret → 0% availability
- **Efter**: Circuit breaker åbner → CommentService bruger fallback → 80% availability

**Ved stigende trafik:**
- **100 req/s**: System håndterer det → 99% availability
- **1000 req/s**: ProfanityService overbelastet → Circuit breaker åbner → 80% availability (med fallback)

#### Mekanisme 2: Retry Pattern (Håndterer Transient Fejl)

**Kodeeksempel:**
```csharp
// CommentService/Program.cs (linje 23-26)
var retry = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => (int)r.StatusCode == 429)  // Rate limiting
    .WaitAndRetryAsync(new[] { 
        TimeSpan.FromMilliseconds(200), 
        TimeSpan.FromMilliseconds(500), 
        TimeSpan.FromSeconds(1) 
    });
```

**Availability forbedring:**
- **Før**: 1 transient fejl → Request fejler → 0% availability for den request
- **Efter**: 1 transient fejl → 3 retries → 95% success rate → Højere availability

**Ved stigende trafik:**
- **100 req/s**: Få transient fejl → Retry håndterer det → 99% availability
- **1000 req/s**: Flere transient fejl → Retry håndterer de fleste → 95% availability

#### Mekanisme 3: Caching (Reducerer Load)

**Kodeeksempel:**
```csharp
// ArticleService/Controllers/ArticlesController.cs (linje 32-39)
var cacheKey = $"articles:latest:{top}";
var cached = await _cache.StringGetAsync(cacheKey);
if (cached.HasValue)
{
    Response.Headers["X-Cache"] = "HIT";
    _hits.Inc();
    return Content(cached!, "application/json"); // 5ms response
}
```

**Availability forbedring:**
- **Før**: Hver request → Database query → Database overbelastet → 0% availability
- **Efter**: 80% cache hits → 80% færre database queries → Database ikke overbelastet → 99% availability

**Ved stigende trafik:**
- **100 req/s**: 20 DB queries/s → Database håndterer det → 99% availability
- **1000 req/s**: 200 DB queries/s (80% cache hits) → Database håndterer det → 99% availability
- **Uden cache**: 1000 DB queries/s → Database overbelastet → 0% availability

#### Mekanisme 4: Timeout (Forhindrer Blokering)

**Kodeeksempel:**
```csharp
// CommentService/Program.cs (linje 35)
c.Timeout = TimeSpan.FromSeconds(2);
```

**Availability forbedring:**
- **Før**: Langsom service blokerer alle requests → Thread pool udtømt → 0% availability
- **Efter**: Timeout efter 2 sek → Thread frigives → System fortsætter → 99% availability

**Ved stigende trafik:**
- **100 req/s**: Få langsomme requests → Timeout håndterer det → 99% availability
- **1000 req/s**: Mange langsomme requests → Timeout forhindrer blokering → 95% availability

#### Mekanisme 5: Fallback (Graceful Degradation)

**Kodeeksempel:**
```csharp
// CommentService/Services/ProfanityClient.cs (linje 14-16)
var fallback = (cfg["FALLBACK_PROFANITY_WORDS"] ?? "bad,ugly,stupid")
              .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
_fallbackList = new HashSet<string>(fallback, StringComparer.OrdinalIgnoreCase);
```

**Availability forbedring:**
- **Før**: ProfanityService nede → Kommentarer ikke mulige → 0% availability for feature
- **Efter**: Fallback aktiveres → Kommentarer mulige (reduceret funktionalitet) → 80% availability

**Ved stigende trafik:**
- **100 req/s**: ProfanityService håndterer det → 100% availability
- **1000 req/s**: ProfanityService overbelastet → Fallback aktiveres → 80% availability

#### Mekanisme 6: X-akse Skalering (Load Distribution)

**Kodeeksempel:**
```yaml
# docker-compose.yml - Kan skaleres til flere replicas
article-service:
  deploy:
    replicas: 3  # 3 instances for load distribution
    restart_policy:
      condition: on-failure
```

**Availability forbedring:**
- **Før**: 1 instance → Hvis instance fejler → 0% availability
- **Efter**: 3 instances → Hvis 1 fejler → 66% availability (2/3 fortsætter)

**Ved stigende trafik:**
- **100 req/s**: 1 instance håndterer det → 99% availability
- **1000 req/s**: 3 instances → Load fordelt → 99% availability
- **Uden skalering**: 1 instance overbelastet → 0% availability

#### Mekanisme 7: Asynkron Processing (Decoupling)

**Kodeeksempel:**
```csharp
// PublisherService/Controllers/PublishController.cs (linje 34-37)
var msg = new { req.Title, req.Content, req.Continent, PublishedAt = DateTime.UtcNow };
_publisher.Publish(msg);

return Accepted(new { status = "queued" }); // 202 Accepted - ikke venter på processing
```

**Availability forbedring:**
- **Før**: Synkron processing → Hvis ArticleService langsom → PublisherService blokeret → 0% availability
- **Efter**: Asynkron queue → PublisherService returnerer umiddelbart → 99% availability

**Ved stigende trafik:**
- **100 req/s**: Synkron → ArticleService kan håndtere det → 99% availability
- **1000 req/s**: Synkron → ArticleService overbelastet → PublisherService blokeret → 0% availability
- **Asynkron**: ArticleService processerer i baggrunden → PublisherService fortsætter → 99% availability

### Kombineret Approach (Optimal Availability):

**Alle mekanismer kombineret:**
```csharp
// CommentService - Kombineret approach
.AddPolicyHandler(retry)      // 1. Retry transient fejl
.AddPolicyHandler(breaker)    // 2. Circuit breaker for permanente fejl
// + Timeout (2 sek)          // 3. Forhindrer blokering
// + Fallback (local cache)   // 4. Graceful degradation
// + Caching (Redis)          // 5. Reducerer load
// + X-akse skalering (3 replicas) // 6. Load distribution
```

**Availability ved stigende trafik:**

| Trafik | Uden Mekanismer | Med Alle Mekanismer | Forbedring |
|--------|-----------------|---------------------|------------|
| **100 req/s** | 99% | 99.9% | +0.9% |
| **500 req/s** | 50% | 99% | +49% |
| **1000 req/s** | 0% (system ned) | 95% | +95% |
| **2000 req/s** | 0% | 90% | +90% |

---

## C. Diskuter hvordan visse arkitektoniske valg kan have en negativ indvirkning på systemets availability

### Arkitektoniske Valg der Reducerer Availability:

#### 1. **Synkron Service Dependencies**

**Problem:**
Tæt kobling mellem services via synkron HTTP calls.

**Eksempel fra kodebasen:**
```csharp
// PublisherService/Controllers/PublishController.cs (linje 24-27)
var client = _http.CreateClient("profanity");
var check = await client.GetAsync($"/api/profanity/check?text=...");
if (!check.IsSuccessStatusCode)
    return StatusCode(502, "ProfanityService unavailable");
```

**Negativ Impact:**
- **Hvis ProfanityService er langsom**: PublisherService blokeret → 0% availability
- **Hvis ProfanityService fejler**: PublisherService fejler → 0% availability
- **Cascading failures**: Fejl spreder sig til alle afhængige services

**Løsning:**
- Asynkron processing (RabbitMQ) → Decoupling
- Circuit breaker → Isolation
- Timeout → Forhindrer blokering

#### 2. **Single Point of Failure (SPOF)**

**Problem:**
Kritisk komponent uden redundancy.

**Eksempler fra kodebasen:**
```yaml
# docker-compose.yml
# Single Redis instance
redis:
  image: redis:7-alpine
  ports:
    - "6379:6379"
  # Ingen replication → SPOF
```

**Negativ Impact:**
- **Hvis Redis fejler**: Alle cache operations fejler → System overbelastet → 0% availability
- **Hvis RabbitMQ fejler**: Ingen message processing → 0% availability for publishing

**Løsning:**
- Redis Cluster (replication)
- RabbitMQ Cluster (high availability)
- Multiple instances (X-akse skalering)

#### 3. **Tæt Database Coupling**

**Problem:**
Alle services afhænger af samme database.

**Eksempel:**
```yaml
# Før Z-akse skalering
article-service:
  environment:
    - DB_CONNECTION=Server=article-db-global;...
```

**Negativ Impact:**
- **Hvis database fejler**: Alle services påvirkes → 0% availability
- **Hvis database er langsom**: Alle services langsomme → 0% availability
- **Database bottleneck**: Kan ikke skaleres uafhængigt

**Løsning:**
- Z-akse skalering (flere databases)
- Database replication
- Read replicas

#### 4. **Manglende Health Checks**

**Problem:**
Ingen automatisk detection af fejlende services.

**Negativ Impact:**
- **Fejlende service opdages sent**: Længere downtime → Lavere availability
- **Ingen automatisk recovery**: Manual intervention nødvendig → Længere downtime

**Løsning:**
```yaml
# Health check endpoints
health:
  path: /health
  interval: 30s
  timeout: 10s
  retries: 3
```

#### 5. **Ingen Circuit Breaker**

**Problem:**
Fejlende services blokerer alle requests.

**Eksempel (Uden Circuit Breaker):**
```csharp
// FØR: Ingen circuit breaker
var resp = await _http.GetAsync("/api/profanity/check?text=...");
// Hvis ProfanityService fejler → Blokerer alle requests
```

**Negativ Impact:**
- **Fejlende service**: Blokerer alle requests → 0% availability
- **Resource exhaustion**: Thread pool udtømt → 0% availability
- **Cascading failures**: Fejl spreder sig

**Løsning:**
```csharp
// Efter: Med circuit breaker
.AddPolicyHandler(breaker)  // Isolerer fejlende service
```

#### 6. **Manglende Timeout**

**Problem:**
Ingen timeout på eksterne calls.

**Negativ Impact:**
- **Langsom service**: Blokerer alle requests → 0% availability
- **Thread pool udtømt**: Alle threads venter → 0% availability

**Løsning:**
```csharp
c.Timeout = TimeSpan.FromSeconds(2);  // Timeout efter 2 sek
```

#### 7. **Synchronous Database Operations**

**Problem:**
Alle database operations er synkrone.

**Eksempel:**
```csharp
// ArticleService/Controllers/ArticlesController.cs
var items = await _context.Articles
    .OrderByDescending(a => a.PublishedAt)
    .Take(top)
    .ToListAsync();  // Blokerer tråd
```

**Negativ Impact:**
- **Langsom database query**: Blokerer tråd → Thread pool udtømt → 0% availability
- **Database bottleneck**: Kan ikke håndtere høj trafik

**Løsning:**
- Caching (reducerer database queries)
- Asynkron processing (queue)
- Read replicas (distribute load)

#### 8. **Ingen Fallback Mechanism**

**Problem:**
Hvis primær service fejler, fejler hele funktionaliteten.

**Eksempel (Uden Fallback):**
```csharp
// FØR: Ingen fallback
var hasProfanity = await _profanity.ContainsProfanity(comment.Text);
if (hasProfanity) return BadRequest("Comment rejected");
// Hvis ProfanityService fejler → Kommentarer ikke mulige
```

**Negativ Impact:**
- **Service fejl**: Total funktionalitetstab → 0% availability for feature
- **Ingen graceful degradation**: Alt eller intet

**Løsning:**
```csharp
// Efter: Med fallback
catch (BrokenCircuitException)
{
    // Fallback: Brug lokal cache
    return _fallbackList.Any(word => text.Contains(word));
}
```

#### 9. **Manglende Monitoring**

**Problem:**
Ingen visibility i systemets tilstand.

**Negativ Impact:**
- **Problemer opdages sent**: Længere downtime → Lavere availability
- **Ingen proactive detection**: Reaktive i stedet for proaktive

**Løsning:**
```csharp
// ArticleService/Program.cs
app.MapMetrics();  // Prometheus metrics
// OpenTelemetry tracing
// Serilog logging
```

#### 10. **Tight Coupling mellem Services**

**Problem:**
Services er tæt koblet, kan ikke fungere uafhængigt.

**Eksempel:**
```
PublisherService → ProfanityService (synkron)
CommentService → ProfanityService (synkron)
```

**Negativ Impact:**
- **Hvis ProfanityService fejler**: Både PublisherService og CommentService fejler → 0% availability
- **Ingen isolation**: Fejl spreder sig

**Løsning:**
- Asynkron processing (decoupling)
- Circuit breaker (isolation)
- Fallback (graceful degradation)

### Sammenligning: Negativ vs. Positiv Impact

| Arkitektonisk Valg | Availability Impact | Eksempel fra Kodebasen |
|---------------------|---------------------|-------------------------|
| **Synkron dependencies** | ❌ 0% (hvis dependency fejler) | PublisherService → ProfanityService (synkron) |
| **Asynkron processing** | ✅ 99% (decoupling) | PublisherService → RabbitMQ → ArticleService |
| **Single Point of Failure** | ❌ 0% (hvis SPOF fejler) | Single Redis instance |
| **Redundancy (X-akse)** | ✅ 99.9% (redundancy) | 3 replicas per service |
| **Ingen circuit breaker** | ❌ 0% (cascading failures) | Før circuit breaker implementation |
| **Circuit breaker** | ✅ 80-90% (isolation) | CommentService → ProfanityService |
| **Ingen timeout** | ❌ 0% (thread pool udtømt) | Før timeout implementation |
| **Timeout** | ✅ 99% (forhindrer blokering) | 2 sek timeout på HTTP calls |
| **Ingen fallback** | ❌ 0% (total fejl) | Før fallback implementation |
| **Fallback** | ✅ 70-80% (graceful degradation) | ProfanityClient fallback word list |

### Best Practices for Høj Availability:

**1. Design for Failure:**
- Antag at services kan fejle
- Implementer circuit breakers, timeouts, fallbacks

**2. Redundancy:**
- X-akse skalering (flere instances)
- Database replication
- Multiple infrastructure components

**3. Isolation:**
- Y-akse skalering (microservices)
- Circuit breakers
- Bulkhead pattern

**4. Decoupling:**
- Asynkron processing (queues)
- Event-driven architecture
- Løs kobling mellem services

**5. Monitoring:**
- Health checks
- Metrics, logging, tracing
- Proactive alerting

### Konklusion:

**Arkitektoniske valg der reducerer availability:**
- ❌ Synkron dependencies
- ❌ Single Point of Failure
- ❌ Tæt database coupling
- ❌ Manglende health checks
- ❌ Ingen circuit breaker
- ❌ Manglende timeout
- ❌ Synchronous operations
- ❌ Ingen fallback
- ❌ Manglende monitoring
- ❌ Tight coupling

**Arkitektoniske valg der forbedrer availability:**
- ✅ Asynkron processing
- ✅ Redundancy (X-akse)
- ✅ Isolation (Y-akse, circuit breakers)
- ✅ Health checks
- ✅ Circuit breakers
- ✅ Timeouts
- ✅ Caching
- ✅ Fallback mechanisms
- ✅ Monitoring
- ✅ Løs kobling

**Anbefaling:**
Design systemet med availability i tankerne fra starten:
- Implementer circuit breakers, timeouts, fallbacks
- Brug asynkron processing hvor muligt
- Skaler på X, Y, Z akser for redundancy og isolation
- Monitor kontinuerligt for proactive detection
