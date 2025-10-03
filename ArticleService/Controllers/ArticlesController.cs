using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.Models;
using StackExchange.Redis;
using System.Text.Json;
using Prometheus;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleDbContext _context;
        private readonly IDatabase _redis;

        // Prometheus counters
        private static readonly Counter CacheHit  = Metrics.CreateCounter("article_cache_hit",  "Article cache hits");
        private static readonly Counter CacheMiss = Metrics.CreateCounter("article_cache_miss", "Article cache misses");

        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public ArticlesController(ArticleDbContext context, IConnectionMultiplexer mux)
        {
            _context = context;
            _redis = mux.GetDatabase();
        }

        private static string Key(int id) => $"article:{id}";

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Article article)
        {
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
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            await _redis.KeyDeleteAsync(Key(id));
            return NoContent();
        }
    }
}
