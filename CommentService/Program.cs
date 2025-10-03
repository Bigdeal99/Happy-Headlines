using CommentService.Data;
using CommentService.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
          ?? "Server=comment-db;Database=Comments;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";

builder.Services.AddDbContext<CommentDbContext>(opt =>
    opt.UseSqlServer(conn, sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null)));

builder.Services.AddControllers();

// ---------- Profanity HttpClient with Circuit Breaker ----------
string profanityBase = Environment.GetEnvironmentVariable("PROFANITY_URL") ?? "http://profanity-service:8080";

var retry = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => (int)r.StatusCode == 429)
    .WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1) });

var breaker = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(20));

builder.Services.AddHttpClient<IProfanityClient, ProfanityClient>(c =>
{
    c.BaseAddress = new Uri(profanityBase);
    c.Timeout = TimeSpan.FromSeconds(2);
})
.AddPolicyHandler(retry)
.AddPolicyHandler(breaker);

// ---------- Redis ----------
var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));

var app = builder.Build();

// ✅ Retry until DB is ready
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    var retries = 0;
    var connected = false;

    while (!connected && retries < 10)
    {
        try
        {
            db.Database.EnsureCreated();
            connected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Comment DB not ready yet: {ex.Message}");
            Thread.Sleep(5000);
            retries++;
        }
    }

    if (!connected)
    {
        throw new Exception("❌ Could not connect to Comment DB after retries.");
    }
}

// Prometheus metrics endpoint
app.UseHttpMetrics();
app.MapMetrics("/metrics");

app.MapControllers();
app.Run();
