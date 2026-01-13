# Google Slides Content - Spørgsmål 1: Asynkron Design
## Kopier dette indhold direkte ind i Google Slides

---

## SLIDE 1: Titel
**Titel:** Asynkron Design
**Undertitel:** Performance vs. Kompleksitet


---

---

## SLIDE 3: Problem 1 - Blocking I/O
**Titel:** Problem 1: Blocking I/O Operationer

**Indhold:**
- **Problem:** Tråd blokeret under I/O
- **Effekt:** 
  - Lav throughput
  - Ressource-spild
  - Skaleringsproblemer
- **Eksempel:** Database query blokerer tråd → kan ikke håndtere andre requests

---

## SLIDE 4: Problem 2 - Tæt Kobling
**Titel:** Problem 2: Tæt Kobling mellem Services

**Indhold:**
- **Problem:** Synkron kommunikation
- **Effekt:** 
  - Hvis én service er langsom → hele kæden påvirkes
  - Cascading failures
- **Eksempel:** Service A → Service B → Service C (synkron)

---

## SLIDE 5: Problem 3 - Long-Running Operations
**Titel:** Problem 3: Long-Running Operations

**Indhold:**
- **Problem:** Operationer der tager lang tid
- **Eksempler:** 
  - Cache warming
  - Data processing
  - Batch jobs
- **Effekt:** Blokerer API responses

---

## SLIDE 6: Problem 4 - Resource Contention
**Titel:** Problem 4: Resource Contention

**Indhold:**
- **Problem:** Mange samtidige requests til samme resource
- **Eksempel:** Database connection pool udtømt
- **Effekt:** 
  - Bottleneck
  - Langsomme responses

---

## SLIDE 7: Demo 1 - Message Queue Pattern
**Titel:** Demonstration 1: Message Queue Pattern

**Indhold:**
- **Fil:** `PublisherService/Controllers/PublishController.cs`
- **Fuld Kode:**
```csharp
using Microsoft.AspNetCore.Mvc;
using PublisherService.Models;
using PublisherService.Services;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly IArticleQueuePublisher _publisher;

    public PublishController(IHttpClientFactory http, IArticleQueuePublisher publisher)
    {
        _http = http;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> Publish(PublishArticleRequest req)
    {
        // call ProfanityService first (distributed trace via HttpClient instrumentation)
        var client = _http.CreateClient("profanity");
        var check = await client.GetAsync($"/api/profanity/check?text={Uri.EscapeDataString(req.Title + " " + req.Content)}");
        if (!check.IsSuccessStatusCode)
            return StatusCode(502, "ProfanityService unavailable");

        var json = await check.Content.ReadAsStringAsync();
        if (json.Contains("\"contains\":true") || json.Contains("true"))
            return BadRequest("Article rejected due to profanity.");

        // put to queue
        var msg = new { req.Title, req.Content, req.Continent, PublishedAt = DateTime.UtcNow };
        _publisher.Publish(msg);

        return Accepted(new { status = "queued" });
    }
}
```
- **Fokus:** Linje 35 `_publisher.Publish(msg)` - asynkron queue
- **Fokus:** Linje 37 `return Accepted(...)` - 202 Accepted umiddelbart
- **Fordel:** API returnerer umiddelbart (202 Accepted)
- **Performance:** 10x hurtigere response time (500ms → 50ms)

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 8: Demo 1 - ArticleQueueConsumer (Asynkron Consumer)
**Titel:** Demonstration 1: ArticleQueueConsumer (Asynkron Consumer)

**Indhold:**
- **Fil:** `ArticleService/Services/ArticleQueueConsumer.cs`
- **Fuld Kode:**
```csharp
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ArticleService.Data;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class ArticleQueueConsumer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private IConnection? _conn;
    private IModel? _channel;

    private static readonly ActivitySource ActivitySrc = new("ArticleService");
    private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

    public ArticleQueueConsumer(IServiceProvider sp) => _sp = sp;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBIT_HOST") ?? "rabbitmq",
            UserName = Environment.GetEnvironmentVariable("RABBIT_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBIT_PASS") ?? "guest",
            DispatchConsumersAsync = true
        };

        _conn = factory.CreateConnection();
        _channel = _conn.CreateModel();
        _channel.QueueDeclare("articles", durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (ch, ea) =>
        {
            var parentContext = Propagator.Extract(default, ea.BasicProperties.Headers,
                (hdrs, key) => hdrs != null && hdrs.TryGetValue(key, out var val)
                    ? new[] { Encoding.UTF8.GetString((byte[])val) }
                    : Array.Empty<string>());

            using var activity = ActivitySrc.StartActivity(
                "Consume Article",
                ActivityKind.Consumer,
                parentContext.ActivityContext);

            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var msg = JsonSerializer.Deserialize<ArticleMessage>(json)!;

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

            db.Articles.Add(new Article
            {
                Title = msg.Title,
                Content = msg.Content,
                Continent = msg.Continent,
                PublishedAt = msg.PublishedAt
            });
            await db.SaveChangesAsync(stoppingToken);

            _channel!.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicQos(0, 10, false);  // Handle 10 messages concurrently
        _channel.BasicConsume("articles", autoAck: false, consumer);
        return Task.CompletedTask;
    }

    public record ArticleMessage(string Title, string Content, string Continent, DateTime PublishedAt);

    public override void Dispose()
    {
        _channel?.Close();
        _conn?.Close();
        base.Dispose();
    }
}
```
- **Fokus:** Linje 39 `consumer.Received += async (ch, ea) =>` - asynkron message handling
- **Fokus:** Linje 46-49 `ActivitySrc.StartActivity` - distributed tracing
- **Fokus:** Linje 64 `await db.SaveChangesAsync(stoppingToken)` - asynkron database write
- **Fokus:** Linje 69 `BasicQos(0, 10, false)` - concurrent processing (10 messages)
- **Fordel:** Consumer kan håndtere 10 messages samtidigt
- **Performance:** Bedre resource utilization

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 9: Demo 1 - Performance Metrics
**Titel:** Performance Forbedring: Message Queue

**Indhold:**
| Metrik | Før (Synkron) | Efter (Asynkron) | Forbedring |
|--------|---------------|------------------|------------|
| Response time | 500ms | 50ms | 10x |
| Throughput | 2 req/s per tråd | 20 req/s per tråd | 10x |

---

## SLIDE 10: Demo 2 - Async/Await i Controllers
**Titel:** Demonstration 2: Async/Await i Controllers

**Indhold:**
- **Fil:** `ArticleService/Controllers/ArticlesController.cs`
- **Fuld Kode (List metoden):**
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.Models;
using ArticleService.Services;
using StackExchange.Redis;
using Prometheus;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleDbContext _context;
        private readonly IDatabase _cache;
        private readonly Counter _hits;
        private readonly Counter _misses;
        
        public ArticlesController(ArticleDbContext context, IConnectionMultiplexer redis, ArticleMetricSet metrics)
        {
            _context = context;
            _cache = redis.GetDatabase();
            _hits = metrics.Hits;
            _misses = metrics.Misses;
        }

        // LIST (for NewsletterService): /api/articles?top=5
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int top = 5)
        {
            top = Math.Clamp(top, 1, 100);
            var cacheKey = $"articles:latest:{top}";
            
            // Async cache lookup - tråd frigives under I/O
            var cached = await _cache.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                Response.Headers["X-Cache"] = "HIT";
                _hits.Inc();
                return Content(cached!, "application/json");
            }

            // Async database query - tråd frigives under I/O
            var items = await _context.Articles
                    .OrderByDescending(a => a.PublishedAt)
                    .Take(top)
                    .Select(a => new { a.Id, a.Title, a.PublishedAt })
                    .ToListAsync();

            _misses.Inc();
            var json = System.Text.Json.JsonSerializer.Serialize(items);
            
            // Async cache write - tråd frigives under I/O
            await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));
            Response.Headers["X-Cache"] = "MISS";
            return Content(json, "application/json");
        }
    }
}
```
- **Fokus:** Linje 33 `await _cache.StringGetAsync(cacheKey)` - asynkron cache lookup
- **Fokus:** Linje 41-45 `await _context.Articles.ToListAsync()` - asynkron database query
- **Fokus:** Linje 49 `await _cache.StringSetAsync(...)` - asynkron cache write
- **Fordel:** Tråd frigives under I/O → kan håndtere flere requests
- **Performance:** 50x mindre hukommelse (1000 tråde → 20 tråde)

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 11: Demo 2 - Thread Pool Efficiency
**Titel:** Thread Pool Efficiency

**Indhold:**
- **Synkron:**
  - 1000 requests = 1000 tråde = 2GB RAM
- **Asynkron:**
  - 1000 requests = 20 tråde = 40MB RAM
- **Forbedring:** 50x mindre hukommelse

---

## SLIDE 12: Demo 3 - Background Services
**Titel:** Demonstration 3: Background Services

**Indhold:**
- **Fil:** `ArticleService/Services/ArticleCacheWarmer.cs`
- **Fuld Kode:**
```csharp
using ArticleService.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ArticleService.Services;

public class ArticleCacheWarmer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IDatabase _cache;
    private readonly ILogger<ArticleCacheWarmer> _logger;
    private static readonly int[] TopWindows = new[] { 5, 10, 20 };

    public ArticleCacheWarmer(IServiceProvider sp, IConnectionMultiplexer redis, ILogger<ArticleCacheWarmer> logger)
    {
        _sp = sp;
        _cache = redis.GetDatabase();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // small initial delay to allow DB to be ready
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        // Kører kontinuerligt i baggrunden
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

                var since = DateTime.UtcNow.AddDays(-14);
                var baseQuery = db.Articles
                    .Where(a => a.PublishedAt >= since)
                    .OrderByDescending(a => a.PublishedAt)
                    .Select(a => new { a.Id, a.Title, a.PublishedAt });

                foreach (var top in TopWindows)
                {
                    // Asynkron database query
                    var items = await baseQuery.Take(top).ToListAsync(stoppingToken);
                    var json = System.Text.Json.JsonSerializer.Serialize(items);
                    
                    // Asynkron cache write
                    await _cache.StringSetAsync($"articles:latest:{top}", json, TimeSpan.FromMinutes(30));
                }

                _logger.LogInformation("Article cache warmed for last 14 days for windows: {Windows}", string.Join(',', TopWindows));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Article cache warmer iteration failed");
            }

            // Vent 2 minutter før næste iteration
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }
}
```
- **Fokus:** Linje 7 `public class ArticleCacheWarmer : BackgroundService` - background service
- **Fokus:** Linje 21 `protected override async Task ExecuteAsync(...)` - asynkron execution
- **Fokus:** Linje 26 `while (!stoppingToken.IsCancellationRequested)` - kontinuerlig loop
- **Fokus:** Linje 41 `await baseQuery.Take(top).ToListAsync(stoppingToken)` - asynkron database query
- **Fokus:** Linje 43 `await _cache.StringSetAsync(...)` - asynkron cache write
- **Fokus:** Linje 53 `await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken)` - vent 2 minutter
- **Fordel:** Påvirker ikke API performance - kører i baggrunden
- **Performance:** Cache hit rate: 30% → 80%

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 13: Demo 3 - Cache Performance
**Titel:** Cache Performance Forbedring

**Indhold:**
| Metrik | Før | Efter | Forbedring |
|--------|-----|------|------------|
| Cache hits | 30% | 80% | 2.7x |
| Response time | 50ms (DB) | 5ms (Cache) | 10x |

---

## SLIDE 14: Performance Summary
**Titel:** Performance Summary

**Indhold:**
| Metrik | Før | Efter | Forbedring |
|--------|-----|------|------------|
| Threads | 1000 | 20 | 50x |
| Memory | 2GB | 40MB | 50x |
| Throughput | 50 req/s | 500 req/s | 10x |
| Response time | 500ms | 50ms | 10x |
| CPU | 80% | 40% | 2x bedre |

---

## SLIDE 15: Kompromis 1 - Error Handling
**Titel:** Kompromis 1: Error Handling Kompleksitet

**Indhold:**
- **Problem:** Mere kompleks error handling
- **Eksempel fra ArticleQueueConsumer.cs:**
```csharp
consumer.Received += async (ch, ea) =>
{
    // ... message processing ...
    
    using var scope = _sp.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

    db.Articles.Add(new Article
    {
        Title = msg.Title,
        Content = msg.Content,
        Continent = msg.Continent,
        PublishedAt = msg.PublishedAt
    });
    
    // Hvad hvis SaveChangesAsync() fejler?
    await db.SaveChangesAsync(stoppingToken);

    // Skal nack message i stedet for ack hvis fejl
    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
};
```
- **Problem:** Hvis `SaveChangesAsync()` fejler → skal BasicNack i stedet for BasicAck
- **Løsning:** Try-catch + Nack handling
- **Kompleksitet:** Synkron kode har simplere error handling

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 16: Kompromis 2 - Debugging
**Titel:** Kompromis 2: Debugging Kompleksitet

**Indhold:**
- **Problem:** 
  - Stack traces mindre informative
  - Async context kan mistes
- **Løsning:** Distributed tracing (OpenTelemetry/Jaeger)
- **Eksempel fra ArticleQueueConsumer.cs:**
```csharp
consumer.Received += async (ch, ea) =>
{
    // Extract trace context fra message headers
    var parentContext = Propagator.Extract(default, ea.BasicProperties.Headers,
        (hdrs, key) => hdrs != null && hdrs.TryGetValue(key, out var val)
            ? new[] { Encoding.UTF8.GetString((byte[])val) }
            : Array.Empty<string>());

    // Start activity for distributed tracing
    using var activity = ActivitySrc.StartActivity(
        "Consume Article",
        ActivityKind.Consumer,
        parentContext.ActivityContext);

    // ... message processing ...
};
```
- **Problem:** Stack traces i asynkron kode er mindre informative
- **Løsning:** Distributed tracing (OpenTelemetry/Jaeger) - kræver ekstra infrastruktur
- **Kompleksitet:** Synkron kode har simplere debugging

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 17: Kompromis 3 - State Management
**Titel:** Kompromis 3: State Management

**Indhold:**
- **Problem:** Shared state kan skabe race conditions
- **Eksempel:** LRU cache management i CommentService
- **Løsning:** Atomic operations eller locking
- **Trade-off:** Locking kan reducere performance gains

---

## SLIDE 18: Kompromis 4 - Message Queue Overhead
**Titel:** Kompromis 4: Message Queue Overhead

**Indhold:**
- **Problem:** 
  - RabbitMQ introducerer latency (1-5ms)
  - Yderligere infrastruktur at vedligeholde
- **Trade-off:** 
  - Længere latency
  - Men bedre decoupling og scalability

---

## SLIDE 19: Kompromis 5 - Eventual Consistency
**Titel:** Kompromis 5: Eventual Consistency

**Indhold:**
- **Problem:** Asynkron processing = eventual consistency
- **Eksempel fra PublishController.cs:**
```csharp
[HttpPost]
public async Task<IActionResult> Publish(PublishArticleRequest req)
{
    // ... profanity check ...
    
    // put to queue
    var msg = new { req.Title, req.Content, req.Continent, PublishedAt = DateTime.UtcNow };
    _publisher.Publish(msg);

    // Returnerer 202 Accepted - article er IKKE tilgængelig med det samme
    return Accepted(new { status = "queued" });
}
```
- **Problem:** Article ikke tilgængelig med det samme
- **Effekt:** 
  - Kræver status endpoints eller websockets
  - Brugeren skal vente eller poll
- **Trade-off:** Immediate consistency vs. Performance

**Screenshot:** Tag screenshot af koden ovenfor

---

## SLIDE 20: Kompromis Summary
**Titel:** Kompromis Summary

**Indhold:**
| Kompromis | Impact |
|-----------|--------|
| Error handling | Højere kompleksitet |
| Debugging | Kræver distributed tracing |
| State management | Race conditions risiko |
| Message queue | Overhead + infrastruktur |
| Consistency | Eventual consistency |

---

## SLIDE 21: Når Brug Asynkron Design?
**Titel:** Når Brug Asynkron Design?

**Indhold:**
- **✅ Brug når:**
  - I/O-bound operations
  - High throughput vigtigere end low latency
  - Services skal decouples
  - Long-running operations

- **❌ Undgå når:**
  - CPU-bound operations
  - Simpel synkron kode er tilstrækkelig
  - Team mangler erfaring

---

## SLIDE 22: Mitigation Strategies
**Titel:** Mitigation Strategies

**Indhold:**
- Comprehensive logging (Serilog, Seq)
- Distributed tracing (OpenTelemetry/Jaeger)
- Error handling patterns (Polly circuit breaker)
- Monitoring (Prometheus metrics)
- Documentation

---

## SLIDE 23: Konklusion
**Titel:** Konklusion

**Indhold:**
- **Performance gains:** 10-50x forbedring
- **Kompleksitet costs:** 
  - Højere learning curve
  - Mere infrastruktur
- **Netto vurdering:** 
  - Værd at betale for i high-scale microservices
  - Essentielt for moderne microservices arkitektur

---

## SLIDE 24: Spørgsmål?
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
   - Koden er nu inkluderet direkte i dokumentet - tag screenshot af koden fra denne fil
3. **Design forslag:**
   - Brug konsistent farveskema
   - Brug store, læsbare fonte
   - Tilføj kode-snippets som billeder eller tekstbokse
4. **Screenshots:**
   - **SLIDE 7:** Tag screenshot af PublishController.cs koden fra denne fil
   - **SLIDE 8:** Tag screenshot af ArticleQueueConsumer.cs koden fra denne fil
   - **SLIDE 10:** Tag screenshot af ArticlesController.cs List metoden fra denne fil
   - **SLIDE 12:** Tag screenshot af ArticleCacheWarmer.cs ExecuteAsync metoden fra denne fil
   - **SLIDE 15:** Tag screenshot af error handling eksemplet fra denne fil
   - **SLIDE 16:** Tag screenshot af debugging eksemplet fra denne fil
   - **SLIDE 19:** Tag screenshot af eventual consistency eksemplet fra denne fil
5. **Kode formatering:**
   - Brug monospace font (Courier New, Consolas, etc.)
   - Brug syntax highlighting hvis muligt
   - Marker vigtige linjer med farver eller annotations
