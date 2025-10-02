using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommentService.Data;
using CommentService.Models;
using CommentService.Services;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentDbContext _db;
        private readonly IProfanityClient _profanity;

        public CommentsController(CommentDbContext db, IProfanityClient profanity)
        {
            _db = db; _profanity = profanity;
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
            return CreatedAtAction(nameof(GetForArticle), new { articleId = comment.ArticleId }, comment);
        }

        // GET /api/comments/{articleId}
        [HttpGet("{articleId:int}")]
        public async Task<IActionResult> GetForArticle(int articleId, CancellationToken ct)
        {
            var list = await _db.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);

            return Ok(list);
        }
    }
}
