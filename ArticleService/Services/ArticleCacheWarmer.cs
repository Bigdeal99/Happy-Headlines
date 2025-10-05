using ArticleService.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ArticleService.Services;

public class ArticleCacheWarmer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IDatabase _cache;
    private readonly ILogger<ArticleCacheWarmer> _logger;
    private static readonly int[] TopWindows = new[] { 5, 10, 20 };

    public ArticleCacheWarmer(IServiceProvider sp, IConnectionMultiplexer redis, ILogger<ArticleCacheWarmer> logger)
    {
        _sp = sp;
        _cache = redis.GetDatabase();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // small initial delay to allow DB to be ready
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

                var since = DateTime.UtcNow.AddDays(-14);
                var baseQuery = db.Articles
                    .Where(a => a.PublishedAt >= since)
                    .OrderByDescending(a => a.PublishedAt)
                    .Select(a => new { a.Id, a.Title, a.PublishedAt });

                foreach (var top in TopWindows)
                {
                    var items = await baseQuery.Take(top).ToListAsync(stoppingToken);
                    var json = System.Text.Json.JsonSerializer.Serialize(items);
                    await _cache.StringSetAsync($"articles:latest:{top}", json, TimeSpan.FromMinutes(30));
                }

                _logger.LogInformation("Article cache warmed for last 14 days for windows: {Windows}", string.Join(',', TopWindows));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Article cache warmer iteration failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }
}


