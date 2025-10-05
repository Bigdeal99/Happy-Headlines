using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.Models;
using StackExchange.Redis;
using System.Text.Json;
using Prometheus;
using ArticleService.Services;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleDbContext _context;
        private readonly IDatabase _redis;

        // Shared counters from custom registry
        private static readonly Counter CacheHit  = AppMetrics.ArticleCacheHit;
        private static readonly Counter CacheMiss = AppMetrics.ArticleCacheMiss;
        private static readonly Counter Requests  = AppMetrics.RequestsTotal;

        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(ArticleDbContext context, IConnectionMultiplexer mux, ILogger<ArticlesController> logger)
        {
            _context = context;
            _redis = mux.GetDatabase();
            _logger = logger;
        }

        private static string Key(int id) => $"article:{id}";

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Article article)
        {
            Requests.Inc(); _logger.LogInformation("[Metrics] Create -> hh_requests_total++");
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            // write-through cache
            var json = JsonSerializer.Serialize(article, JsonOpts);
            await _redis.StringSetAsync(Key(article.Id), json, TimeSpan.FromDays(14));

            return CreatedAtAction(nameof(Get), new { id = article.Id }, article);
        }

        // READ (read-through cache)
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Requests.Inc(); _logger.LogInformation("[Metrics] Get -> hh_requests_total++");
            var key = Key(id);

            var cached = await _redis.StringGetAsync(key);
            if (cached.HasValue)
            {
                CacheHit.Inc();
                var articleCached = JsonSerializer.Deserialize<Article>(cached!, JsonOpts);
                return Ok(articleCached);
            }

            CacheMiss.Inc();
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            await _redis.StringSetAsync(key, JsonSerializer.Serialize(article, JsonOpts), TimeSpan.FromDays(14));
            return Ok(article);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Article article)
        {
            Requests.Inc(); _logger.LogInformation("[Metrics] Update -> hh_requests_total++");
            if (id != article.Id) return BadRequest();
            _context.Entry(article).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // keep cache in sync
            await _redis.StringSetAsync(Key(id), JsonSerializer.Serialize(article, JsonOpts), TimeSpan.FromDays(14));
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Requests.Inc(); _logger.LogInformation("[Metrics] Delete -> hh_requests_total++");
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            await _redis.KeyDeleteAsync(Key(id));
            return NoContent();
        }
    }
}
