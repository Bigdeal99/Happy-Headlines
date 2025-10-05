using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommentService.Data;
using CommentService.Models;
using CommentService.Services;
using StackExchange.Redis;
using Prometheus;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentDbContext _db;
        private readonly IProfanityClient _profanity;
        private readonly IDatabase _cache;
        private readonly Counter _hits;
        private readonly Counter _misses;

        public CommentsController(CommentDbContext db, IProfanityClient profanity, IConnectionMultiplexer redis, Counter commentCacheHits, Counter commentCacheMisses)
        {
            _db = db; _profanity = profanity;
            _cache = redis.GetDatabase();
            _hits = commentCacheHits;
            _misses = commentCacheMisses;
        }

        // POST /api/comments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Comment comment, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(comment.Text)) return BadRequest("Text is required.");

            // Ask ProfanityService (protected by circuit breaker)
            var hasProfanity = await _profanity.ContainsProfanity(comment.Text, ct);
            if (hasProfanity) return BadRequest("Comment rejected due to profanity.");

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync(ct);

            // Invalidate cache for the article so next GET repopulates
            var key = $"comments:article:{comment.ArticleId}";
            await _cache.KeyDeleteAsync(key);
            return CreatedAtAction(nameof(GetForArticle), new { articleId = comment.ArticleId }, comment);
        }

        // GET /api/comments/{articleId}
        [HttpGet("{articleId:int}")]
        public async Task<IActionResult> GetForArticle(int articleId, CancellationToken ct)
        {
            var key = $"comments:article:{articleId}";
            var cached = await _cache.StringGetAsync(key);
            if (cached.HasValue)
            {
                _hits.Inc();
                return Content(cached!, "application/json");
            }

            var list = await _db.Comments
                    .Where(c => c.ArticleId == articleId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync(ct);

            _misses.Inc();
            var json = System.Text.Json.JsonSerializer.Serialize(list);
            await _cache.StringSetAsync(key, json, TimeSpan.FromMinutes(10));
            // Move key to front in LRU list and enforce size 30
            await _cache.ListRemoveAsync("comments:lru", key, 0);
            await _cache.ListLeftPushAsync("comments:lru", key);
            await _cache.ListTrimAsync("comments:lru", 0, 29);

            // Evict any keys beyond the LRU window
            var length = await _cache.ListLengthAsync("comments:lru");
            if (length > 30)
            {
                var toEvict = await _cache.ListRangeAsync("comments:lru", 30, -1);
                foreach (var evictKey in toEvict)
                {
                    await _cache.KeyDeleteAsync(evictKey.ToString());
                }
                await _cache.ListTrimAsync("comments:lru", 0, 29);
            }
            return Content(json, "application/json");
        }
    }
}
