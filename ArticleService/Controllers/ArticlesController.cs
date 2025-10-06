using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.Models;
using ArticleService.Services;
using StackExchange.Redis;
using Prometheus;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleDbContext _context;
        private readonly IDatabase _cache;
        private readonly Counter _hits;
        private readonly Counter _misses;
        public ArticlesController(ArticleDbContext context, IConnectionMultiplexer redis, ArticleMetricSet metrics)
        {
            _context = context;
            _cache = redis.GetDatabase();
            _hits = metrics.Hits;
            _misses = metrics.Misses;
        }

        // LIST (for NewsletterService): /api/articles?top=5
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int top = 5)
        {
            top = Math.Clamp(top, 1, 100);
            var cacheKey = $"articles:latest:{top}";
            var cached = await _cache.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                Response.Headers["X-Cache"] = "HIT";
                _hits.Inc();
                return Content(cached!, "application/json");
            }

            var items = await _context.Articles
                    .OrderByDescending(a => a.PublishedAt)
                    .Take(top)
                    .Select(a => new { a.Id, a.Title, a.PublishedAt })
                    .ToListAsync();

            _misses.Inc();
            var json = System.Text.Json.JsonSerializer.Serialize(items);
            await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));
            Response.Headers["X-Cache"] = "MISS";
            return Content(json, "application/json");
        }

        // DEBUG: Inspect cache for a given 'top' key without affecting counters
        [HttpGet("cache-debug")]
        public async Task<IActionResult> CacheDebug([FromQuery] int top = 5)
        {
            top = Math.Clamp(top, 1, 100);
            var cacheKey = $"articles:latest:{top}";
            var cached = await _cache.StringGetAsync(cacheKey);
            return Ok(new
            {
                key = cacheKey,
                hasValue = cached.HasValue,
                length = cached.HasValue ? cached!.ToString().Length : 0
            });
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = article.Id }, article);
        }

        // READ
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            return article == null ? NotFound() : Ok(article);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Article article)
        {
            if (id != article.Id) return BadRequest();
            _context.Entry(article).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
            return NoContent();
        }
    }
}
