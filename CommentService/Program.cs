using CommentService.Data;
using CommentService.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------
var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
          ?? builder.Configuration.GetConnectionString("DefaultConnection")
          ?? "Server=comment-db;Database=Comments;User=sa;Password=Your_password123;";

builder.Services.AddDbContext<CommentDbContext>(opt => opt.UseSqlServer(conn));

// ---------- Profanity HttpClient with Retry + Circuit Breaker ----------
string profanityBase = Environment.GetEnvironmentVariable("PROFANITY_URL") ?? "http://profanity-service:8080";

builder.Services.AddHttpClient<IProfanityClient, ProfanityClient>(c =>
{
    c.BaseAddress = new Uri(profanityBase);
    c.Timeout = TimeSpan.FromSeconds(2);
})
.AddTransientHttpErrorPolicy(p => p.RetryAsync(3))
.AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(3, TimeSpan.FromSeconds(20)));

// ---------- Web ----------
builder.Services.AddControllers();
var app = builder.Build();

// Auto-create DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    db.Database.EnsureCreated();
}

app.MapControllers();
app.Run();
