using System.Data;
using DraftService.Data;                  
using DraftService.Middleware;            
using Microsoft.Data.SqlClient;           
using Microsoft.EntityFrameworkCore;
using Serilog;

static async Task EnsureSqlServerDatabaseAsync(string connectionString, ILogger logger)
{
    var csb = new SqlConnectionStringBuilder(connectionString);
    if (string.IsNullOrWhiteSpace(csb.InitialCatalog))
        throw new InvalidOperationException("DB_CONNECTION must include Database/Initial Catalog.");

    string dbName = csb.InitialCatalog;
    var masterCsb = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };

    using var conn = new SqlConnection(masterCsb.ConnectionString);
    await conn.OpenAsync();

    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
IF DB_ID(@db) IS NULL
BEGIN
    EXEC('CREATE DATABASE [' + @db + ']');
    IF SERVERPROPERTY('EngineEdition') <> 5
    BEGIN
        EXEC('ALTER DATABASE [' + @db + '] SET READ_COMMITTED_SNAPSHOT ON;');
    END
END";
    cmd.Parameters.Add(new SqlParameter("@db", SqlDbType.NVarChar, 128) { Value = dbName });
    await cmd.ExecuteNonQueryAsync();

    logger.Information("✅ Ensured database [{Db}] exists.", dbName);
}

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

    // DB connection (env is required in containers; default only for local dev)
    var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
              ?? "Server=draft-db;Database=Drafts;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";

    // 1) Create DB first (connects to master)
    await EnsureSqlServerDatabaseAsync(conn, Log.Logger);

    // 2) Register DbContext against the actual DB
    builder.Services.AddDbContext<DraftDbContext>(opt =>
        opt.UseSqlServer(conn, sql => sql.EnableRetryOnFailure()));

    builder.Services.AddControllers();

    var app = builder.Build();

    // 3) Ensure tables exist (or use db.Database.Migrate() if you have migrations)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>(); // <-- change if your context name differs
        var retries = 0;
        while (true)
        {
            try { db.Database.EnsureCreated(); break; }
            catch (Exception ex)
            {
                Log.Warning(ex, "Draft DB not ready yet, retrying...");
                if (++retries >= 10) throw;
                await Task.Delay(3000);
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
