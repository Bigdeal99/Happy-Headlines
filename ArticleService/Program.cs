using ArticleService.Data;
using ArticleService.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// -----------------
// Database
// -----------------
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? "Server=article-db-global;Database=Articles;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true;";

builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseSqlServer(connectionString));

// -----------------
// Controllers
// -----------------
builder.Services.AddControllers();

// -----------------
// RabbitMQ Consumer
// -----------------
builder.Services.AddHostedService<ArticleQueueConsumer>();
builder.Services.AddHostedService<ArticleCacheWarmer>();

// -----------------
// Redis cache + metrics
// -----------------
var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
var articleCacheHits = Metrics.CreateCounter("article_cache_hits_total", "Article cache hits");
var articleCacheMisses = Metrics.CreateCounter("article_cache_misses_total", "Article cache misses");
builder.Services.AddSingleton(articleCacheHits);
builder.Services.AddSingleton(articleCacheMisses);

// -----------------
// OpenTelemetry
// -----------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ArticleService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT")
                ?? "http://jaeger:4317");
        }));

var app = builder.Build();

// -----------------
// Ensure DB exists with retry
// -----------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
    var retries = 0;
    var connected = false;

    while (!connected && retries < 10)
    {
        try
        {
            db.Database.EnsureCreated();
            connected = true;
            Console.WriteLine("✅ Article DB ready");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Article DB not ready yet: {ex.Message}");
            Thread.Sleep(5000); // wait 5 sec
            retries++;
        }
    }

    if (!connected)
    {
        throw new Exception("❌ Could not connect to Article DB after retries.");
    }
}

// -----------------
// Map API Controllers
// -----------------
app.MapControllers();
app.MapMetrics();

app.Run();
