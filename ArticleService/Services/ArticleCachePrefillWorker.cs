using ArticleService.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace ArticleService.Services;

public class ArticleCachePrefillWorker : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IDatabase _redis;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public ArticleCachePrefillWorker(IServiceProvider sp, IConnectionMultiplexer mux)
    {
        _sp = sp;
        _redis = mux.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

                var since = DateTime.UtcNow.AddDays(-14);
                var latest = await db.Articles
                    .Where(a => a.PublishedAt >= since)
                    .OrderByDescending(a => a.PublishedAt)
                    .ToListAsync(stoppingToken);

                var tasks = latest.Select(a =>
                    _redis.StringSetAsync($"article:{a.Id}",
                        JsonSerializer.Serialize(a, JsonOpts),
                        TimeSpan.FromDays(14)));

                await Task.WhenAll(tasks);
                Console.WriteLine($"🧊 Prefilled {latest.Count} articles into Redis (14d).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Prefill error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
