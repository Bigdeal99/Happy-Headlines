using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using StackExchange.Redis;
using SubscriberService.Data;
using SubscriberService.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Database
var dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? "Server=subscriber-db;Database=Subscribers;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";

builder.Services.AddDbContext<SubscriberDbContext>(options =>
    options.UseSqlServer(dbConnection, sql => sql.EnableRetryOnFailure()));

// Redis for feature flags
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddSingleton<FeatureFlagService>();

// RabbitMQ
builder.Services.AddSingleton<SubscriberQueueService>();

// Controllers
builder.Services.AddControllers();

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("SubscriberService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? "http://jaeger:4317");
        }));

var app = builder.Build();

// Ensure database exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SubscriberDbContext>();
    var retries = 0;
    var connected = false;

    while (!connected && retries < 10)
    {
        try
        {
            db.Database.EnsureCreated();
            connected = true;
            Log.Information("Subscriber database connected");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Database not ready yet, retrying... ({Retry}/{MaxRetries})", retries + 1, 10);
            await Task.Delay(5000);
            retries++;
        }
    }

    if (!connected)
    {
        throw new Exception("Could not connect to database after retries");
    }
}

app.MapControllers();
app.Run();
