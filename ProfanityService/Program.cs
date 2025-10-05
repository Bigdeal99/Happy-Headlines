using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;
using ProfanityService.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Logging: use built-in, no CreateLogger()
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
          ?? "Server=profanity-db;Database=Profanity;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";

builder.Services.AddDbContext<ProfanityDbContext>(opt =>
    opt.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

builder.Services.AddControllers();

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
            Thread.Sleep(5000);
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
