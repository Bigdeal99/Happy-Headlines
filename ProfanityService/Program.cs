using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;
using ProfanityService.Models;

var builder = WebApplication.CreateBuilder(args);

var conn = Environment.GetEnvironmentVariable("DB_CONNECTION")
          ?? builder.Configuration.GetConnectionString("DefaultConnection")
          ?? "Server=profanity-db;Database=Profanity;User=sa;Password=Your_password123;";

builder.Services.AddDbContext<ProfanityDbContext>(opt => opt.UseSqlServer(conn));
builder.Services.AddControllers();

var app = builder.Build();

// Ensure DB + seed a few words
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfanityDbContext>();
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
}

app.MapControllers();
app.Run();
