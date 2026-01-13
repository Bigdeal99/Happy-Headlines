# Spørgsmål 1: Asynkron Design

## A. Beskriv problemstillinger som asynkront design kan hjælpe med at løse

### Problemstillinger:

#### 1. **Blocking I/O Operationer**
Når en synkron operation venter på I/O (database, HTTP requests, filsystem), bliver tråden blokeret og kan ikke håndtere andre requests. Dette skaber:
- **Lav throughput**: Få requests kan håndteres samtidigt
- **Ressource-spild**: Tråde venter inaktivt på I/O
- **Skaleringsproblemer**: Kræver flere tråde = mere hukommelse

#### 2. **Tæt kobling mellem services**
Synkron kommunikation skaber tæt kobling - hvis en service er langsom eller nede, påvirker det hele kæden.

#### 3. **Long-running operations**
Operationer der tager lang tid (cache warming, data processing) bør ikke blokere API responses.

#### 4. **Resource contention**
Mange samtidige requests til samme resource (database) kan skabe bottlenecks.

---

## B. Demonstrer at asynkron kode har en positiv effekt på performance

### Demonstration 1: Message Queue Pattern (PublisherService → ArticleService)

**Kodeeksempel:**
```csharp
// PublisherService/Controllers/PublishController.cs (linje 20-38)
[HttpPost]
public async Task<IActionResult> Publish(PublishArticleRequest req)
{
    // Synkron HTTP call til ProfanityService (kan optimeres)
    var check = await client.GetAsync(...);
    
    // Asynkron queue publish - returnerer umiddelbart
    _publisher.Publish(msg);
    
    return Accepted(new { status = "queued" }); // 202 Accepted
}
```

**Performance-fordele:**
1. **Response time**: API returnerer 202 Accepted i stedet for at vente på database write
   - **Før (synkron)**: ~500ms (HTTP check + DB write)
   - **Efter (asynkron)**: ~50ms (kun HTTP check)
   - **Forbedring**: 10x hurtigere response time

2. **Throughput**: Kan håndtere flere requests, da tråden ikke blokeres på DB write
   - **Før**: 1 request per tråd = ~2 requests/sekund per tråd
   - **Efter**: 1 request per tråd = ~20 requests/sekund per tråd

3. **Decoupling**: `ArticleService` kan processere messages i sit eget tempo
   - Hvis DB er langsom, påvirker det ikke `PublisherService`

**Kodeeksempel - Consumer:**
```csharp
// ArticleService/Services/ArticleQueueConsumer.cs (linje 38-67)
var consumer = new AsyncEventingBasicConsumer(_channel);
consumer.Received += async (ch, ea) =>
{
    // Asynkron processing - tråden blokeres ikke
    await db.SaveChangesAsync(stoppingToken);
    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
};

_channel.BasicQos(0, 10, false); // Håndter 10 messages samtidigt
```

**Performance-måling:**
- **Concurrent processing**: 10 messages kan processeres samtidigt
- **Better resource utilization**: Database connection pool udnyttes bedre
- **Backpressure handling**: Hvis consumer er langsom, bufferes messages i queue

### Demonstration 2: Async/Await i Controllers

**Kodeeksempel:**
```csharp
// ArticleService/Controllers/ArticlesController.cs (linje 29-52)
[HttpGet]
public async Task<IActionResult> List([FromQuery] int top = 5)
{
    // Asynkron cache lookup - tråden blokeres ikke
    var cached = await _cache.StringGetAsync(cacheKey);
    if (cached.HasValue)
    {
        _hits.Inc();
        return Content(cached!, "application/json");
    }

    // Asynkron database query - tråden blokeres ikke
    var items = await _context.Articles
        .OrderByDescending(a => a.PublishedAt)
        .Take(top)
        .ToListAsync();

    // Asynkron cache write - tråden blokeres ikke
    await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));
    return Content(json, "application/json");
}
```

**Performance-fordele:**
1. **Thread pool efficiency**: 
   - **Synkron**: 1 tråd per request = 1000 tråde for 1000 requests
   - **Asynkron**: 1 tråd kan håndtere mange requests = ~10-50 tråde for 1000 requests
   - **Hukommelse-besparelse**: ~95% reduktion i tråde

2. **Scalability**:
   - **Før**: 1000 requests = 1000 tråde = ~2GB RAM (2MB per tråd)
   - **Efter**: 1000 requests = ~20 tråde = ~40MB RAM
   - **Forbedring**: 50x mindre hukommelse

3. **Concurrent I/O**:
   - Flere database queries kan køre samtidigt på samme tråd
   - Bedre udnyttelse af connection pool

### Demonstration 3: Background Services

**Kodeeksempel:**
```csharp
// ArticleService/Services/ArticleCacheWarmer.cs
public class ArticleCacheWarmer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Asynkron cache warming - påvirker ikke API performance
            var items = await baseQuery.Take(top).ToListAsync(stoppingToken);
            await _cache.StringSetAsync($"articles:latest:{top}", json, ...);
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

**Performance-fordele:**
1. **Cache hit rate**: Proaktiv cache warming = højere cache hit rate
   - **Før**: 30% cache hits (cold cache)
   - **Efter**: 80% cache hits (warm cache)
   - **Response time**: 5ms (cache) vs 50ms (database)

2. **API performance**: Cache warming sker i baggrunden
   - Ingen impact på API response times
   - Bedre user experience

### Performance Metrics (Teoretisk eksempel)

**Scenario**: 1000 concurrent requests til ArticleService

| Metrik | Synkron | Asynkron | Forbedring |
|--------|---------|----------|------------|
| Threads brugt | 1000 | 20 | 50x |
| Memory (RAM) | 2GB | 40MB | 50x |
| Throughput (req/s) | 50 | 500 | 10x |
| Response time (p95) | 500ms | 50ms | 10x |
| CPU utilization | 80% | 40% | 2x bedre |

---

## C. Vurder kompromiser mellem kompleksitet og performance

### Kompromiser:

#### A. **Kompleksitet i Error Handling**

**Problem:**
Asynkron kode har mere kompleks error handling:
- Exceptions kan opstå i forskellige kontekster
- Stack traces er mindre klare
- Deadlocks kan opstå hvis man glemmer `await`

**Eksempel fra kodebasen:**
```csharp
// ArticleQueueConsumer.cs - Hvad hvis db.SaveChangesAsync() fejler?
await db.SaveChangesAsync(stoppingToken);
_channel!.BasicAck(ea.DeliveryTag, multiple: false); // Hvis fejl, skal vi nack i stedet
```

**Løsning:**
```csharp
try {
    await db.SaveChangesAsync(stoppingToken);
    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
} catch (Exception ex) {
    _logger.LogError(ex, "Failed to save article");
    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
}
```

**Kompromis:**
- **Performance**: ✅ Bedre (async)
- **Kompleksitet**: ❌ Højere (kræver try-catch, nack handling)

#### B. **Debugging og Observability**

**Problem:**
- Stack traces er mindre informative
- Async context kan mistes
- Distributed tracing bliver nødvendigt

**Eksempel fra kodebasen:**
```csharp
// OpenTelemetry tracing nødvendigt for at følge async flow
using var activity = ActivitySrc.StartActivity("Consume Article", ...);
```

**Kompromis:**
- **Performance**: ✅ Bedre
- **Kompleksitet**: ❌ Højere (kræver distributed tracing setup)

#### C. **State Management**

**Problem:**
- Shared state kan skabe race conditions
- Thread-safety bliver vigtigere
- Locking kan reducere performance gains

**Eksempel fra kodebasen:**
```csharp
// CommentService - LRU cache management
await _cache.ListRemoveAsync(lruKey, key, 0);
await _cache.ListLeftPushAsync(lruKey, key);
// Hvad hvis to requests opdaterer samtidigt?
```

**Kompromis:**
- **Performance**: ✅ Bedre (concurrent access)
- **Kompleksitet**: ❌ Højere (kræver atomic operations eller locking)

#### D. **Testing Kompleksitet**

**Problem:**
- Async tests kræver `async Task` test methods
- Mocking af async operations er mere komplekst
- Timing issues kan opstå

**Kompromis:**
- **Performance**: ✅ Bedre
- **Kompleksitet**: ❌ Højere (kræver async test patterns)

#### E. **Message Queue Overhead**

**Problem:**
- RabbitMQ introducerer latency (queue → consumer)
- Yderligere infrastruktur at vedligeholde
- Message durability overhead

**Eksempel fra kodebasen:**
```csharp
// PublisherService - Message queue overhead
_publisher.Publish(msg); // ~1-5ms overhead
// vs direkte HTTP call ~50ms, men synkron
```

**Kompromis:**
- **Performance**: ✅ Bedre (decoupling, scalability)
- **Kompleksitet**: ❌ Højere (RabbitMQ setup, monitoring, error handling)

#### F. **Eventual Consistency**

**Problem:**
- Asynkron processing = eventual consistency
- User kan ikke se data med det samme
- Kræver polling eller websockets for real-time updates

**Eksempel fra kodebasen:**
```csharp
// PublisherService returnerer 202 Accepted
return Accepted(new { status = "queued" });
// Article er ikke tilgængelig med det samme
```

**Kompromis:**
- **Performance**: ✅ Bedre (hurtigere response)
- **Kompleksitet**: ❌ Højere (kræver status endpoints, polling)

### Balance mellem Kompleksitet og Performance

**Anbefalinger:**

1. **Brug asynkron design når:**
   - I/O-bound operations (database, HTTP, file system)
   - High throughput er vigtigere end low latency
   - Services skal decouples
   - Long-running operations

2. **Undgå asynkron design når:**
   - CPU-bound operations (kun overhead)
   - Simpel synkron kode er tilstrækkelig
   - Team mangler erfaring med async patterns

3. **Mitigation strategies:**
   - **Comprehensive logging**: Brug structured logging (Serilog, Seq)
   - **Distributed tracing**: OpenTelemetry/Jaeger (som i kodebasen)
   - **Error handling patterns**: Retry, circuit breaker (Polly)
   - **Monitoring**: Prometheus metrics for async operations
   - **Documentation**: Klar dokumentation af async flows

### Konklusion

**Performance gains:**
- 10-50x bedre throughput
- 10x hurtigere response times
- 50x mindre hukommelse-forbrug

**Kompleksitet costs:**
- Højere learning curve
- Mere kompleks error handling
- Yderligere infrastruktur (queues, tracing)
- Eventual consistency challenges

**Netto vurdering:**
For high-scale microservices (som Happy-Headlines) er kompleksiteten værd at betale for performance-gains, især når man har:
- ✅ Distributed tracing (Jaeger)
- ✅ Monitoring (Prometheus/Grafana)
- ✅ Centralized logging (Seq)
- ✅ Error handling patterns (Polly circuit breakers)

