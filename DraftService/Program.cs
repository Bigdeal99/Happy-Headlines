using DraftService.Data;
using DraftService.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ---- Serilog with console + Seq (central logging) ----
var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://seq:5341";
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(serverUrl: seqUrl)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // DB connection
    var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
              ?? "Server=draft-db;Database=Drafts;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";

    builder.Services.AddDbContext<DraftDbContext>(opt =>
        opt.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

    builder.Services.AddControllers();

    var app = builder.Build();

    // Ensure DB exists (with retry wait for SQL Server to boot)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
        var retries = 0;
        while (true)
        {
            try { db.Database.EnsureCreated(); break; }
            catch (Exception ex)
            {
                Log.Warning(ex, "Draft DB not ready yet, retrying...");
                if (++retries >= 10) throw;
                await Task.Delay(5000);
            }
        }
    }

    // Tracing
    app.UseMiddleware<CorrelationIdMiddleware>();

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DraftService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
