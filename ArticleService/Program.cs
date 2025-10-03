using ArticleService.Data;
using ArticleService.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ----------------- DB -----------------
var connectionString =
    Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? "Server=article-db-global;Database=Articles;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true;";

builder.Services.AddDbContext<ArticleDbContext>(opt =>
    opt.UseSqlServer(connectionString,
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null)));

// ----------------- Redis -----------------
var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));

// ----------------- Controllers -----------------
builder.Services.AddControllers();

// ----------------- Background workers -----------------
builder.Services.AddHostedService<ArticleQueueConsumer>();   // keep RabbitMQ consumer
builder.Services.AddHostedService<ArticleCachePrefillWorker>(); // Redis prefill

// ----------------- OpenTelemetry -----------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ArticleService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? "http://jaeger:4317");
        }));

var app = builder.Build();

// Prometheus metrics
app.UseHttpMetrics();
app.MapMetrics("/metrics");

// Ensure DB exists (with retries)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
    for (var i = 0; i < 10; i++)
    {
        try { db.Database.EnsureCreated(); Console.WriteLine("✅ Article DB ready"); break; }
        catch (Exception ex) { Console.WriteLine($"⚠️ Article DB not ready: {ex.Message}"); Thread.Sleep(5000); }
    }
}

app.MapControllers();
app.Run();
