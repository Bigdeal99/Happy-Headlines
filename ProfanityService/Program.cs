using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;
using ProfanityService.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
          ?? "Server=profanity-db;Database=Profanity;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";


builder.Services.AddDbContext<ProfanityDbContext>(opt => opt.UseSqlServer(conn));
builder.Services.AddControllers();

// OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ProfanityService"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? "http://jaeger:4317");
        }));

var app = builder.Build();

// ✅ Retry until DB is ready
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfanityDbContext>();
    var retries = 0;
    var connected = false;

    while (!connected && retries < 10)
    {
        try
        {
            db.Database.EnsureCreated();
            if (!db.Words.Any())
            {
                db.Words.AddRange(
                    new ProfanityWord { Word = "bad" },
                    new ProfanityWord { Word = "ugly" },
                    new ProfanityWord { Word = "stupid" }
                );
                db.SaveChanges();
            }
            connected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Database not ready yet: {ex.Message}");
            Thread.Sleep(5000); // wait 5 seconds before retry
            retries++;
        }
    }

    if (!connected)
    {
        throw new Exception("❌ Could not connect to database after retries.");
    }
}

app.MapControllers();
app.Run();
