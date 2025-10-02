using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// connection string from env var
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? "Server=article-db-global;Database=Articles;User=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True;";


builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
    db.Database.EnsureCreated();
}
app.MapControllers();
app.Run();
