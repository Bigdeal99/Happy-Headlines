using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommentService.Data;
using CommentService.Models;
using CommentService.Services;
using StackExchange.Redis;
using System.Text.Json;
using Prometheus;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentDbContext _db;
        private readonly IProfanityClient _profanity;
        private readonly IDatabase _redis;

        private const string LruKey = "comments:lru";
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        private static readonly Counter CacheHit  = Metrics.CreateCounter("comment_cache_hit",  "Comment cache hits");
        private static readonly Counter CacheMiss = Metrics.CreateCounter("comment_cache_miss", "Comment cache misses");

        public CommentsController(CommentDbContext db, IProfanityClient profanity, IConnectionMultiplexer mux)
        {
            _db = db; _profanity = profanity; _redis = mux.GetDatabase();
        }

        // POST /api/comments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Comment comment, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(comment.Text)) return BadRequest("Text is required.");

            var hasProfanity = await _profanity.ContainsProfanity(comment.Text, ct);
            if (hasProfanity) return BadRequest("Comment rejected due to profanity.");

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync(ct);

            // invalidate cache for the article
            await _redis.KeyDeleteAsync(Key(comment.ArticleId));
            return CreatedAtAction(nameof(GetForArticle), new { articleId = comment.ArticleId }, comment);
        }

        // GET /api/comments/{articleId}
        [HttpGet("{articleId:int}")]
        public async Task<IActionResult> GetForArticle(int articleId, CancellationToken ct)
        {
            var key = Key(articleId);

            var cached = await _redis.StringGetAsync(key);
            if (cached.HasValue)
            {
                CacheHit.Inc();
                TouchLru(articleId);
                var list = JsonSerializer.Deserialize<List<Comment>>(cached!, JsonOpts) ?? new();
                return Ok(list);
            }

            CacheMiss.Inc();

            var listDb = await _db.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);

            await _redis.StringSetAsync(key, JsonSerializer.Serialize(listDb, JsonOpts), TimeSpan.FromDays(1));
            TouchLru(articleId);
            await TrimLruAsync();

            return Ok(listDb);
        }

        private static string Key(int articleId) => $"comments:{articleId}";

        private void TouchLru(int articleId)
        {
            _redis.SortedSetAdd(LruKey, articleId.ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        // keep only 30 most recently accessed articleIds in cache
        private async Task TrimLruAsync()
        {
            var count = await _redis.SortedSetLengthAsync(LruKey);
            while (count > 30)
            {
                var popped = await _redis.SortedSetPopAsync(LruKey, Order.Ascending);
                if (popped.Element.HasValue)
                    await _redis.KeyDeleteAsync($"comments:{popped.Element}");
                count = await _redis.SortedSetLengthAsync(LruKey);
            }
        }
    }
}
