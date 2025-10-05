using ArticleService.Data;
using ArticleService.Services;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// ----------------- DB -----------------
var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
          ?? "Server=article-db-global;Database=Articles;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";

// Ensure DB exists before EF tries to connect
using (var tempLoggerFactory = LoggerFactory.Create(b => b.AddConsole()))
{
    var startupLogger = tempLoggerFactory.CreateLogger("ArticleServiceStartup");
    await EnsureSqlServerDatabaseAsync(conn, startupLogger);
}

builder.Services.AddDbContext<ArticleDbContext>(opt =>
    opt.UseSqlServer(conn, sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null)));

// ----------------- Redis -----------------
var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));

// ----------------- Controllers -----------------
builder.Services.AddControllers();

// ----------------- Background workers -----------------
// Disable queue consumer when RabbitMQ is unavailable (avoid crashing dev metrics)
var enableQueue = Environment.GetEnvironmentVariable("ENABLE_QUEUE")?.ToLowerInvariant() != "false";
if (enableQueue)
    builder.Services.AddHostedService<ArticleQueueConsumer>();
builder.Services.AddHostedService<ArticleCachePrefillWorker>();

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

// Prometheus metrics - expose shared registry
app.UseHttpMetrics();
app.UseMetricServer("/metrics", registry: AppMetrics.Registry);

// Ensure DB tables exist (retry loop)
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


// ------------- Helper -------------
static async Task EnsureSqlServerDatabaseAsync(string connectionString, ILogger logger)
{
    var builder = new SqlConnectionStringBuilder(connectionString);
    var databaseName = builder.InitialCatalog;

    // Connect to master DB
    builder.InitialCatalog = "master";

    using var connection = new SqlConnection(builder.ConnectionString);
    await connection.OpenAsync();

    using var cmd = connection.CreateCommand();
    cmd.CommandText = $@"
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
        BEGIN
            CREATE DATABASE [{databaseName}];
        END";
    await cmd.ExecuteNonQueryAsync();

    logger.LogInformation("✅ Ensured database {Database} exists", databaseName);
}
