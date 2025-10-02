using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;

namespace ProfanityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfanityController : ControllerBase
    {
        private readonly ProfanityDbContext _db;
        public ProfanityController(ProfanityDbContext db) => _db = db;

        // GET /api/profanity/check?text=hello world
        [HttpGet("check")]
        public async Task<IActionResult> Check([FromQuery] string text, CancellationToken ct)
        {
            var words = await _db.Words.AsNoTracking().Select(w => w.Word).ToListAsync(ct);
            var hits = new List<string>();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var tokens = text.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\'', '"' },
                                        StringSplitOptions.RemoveEmptyEntries);
                foreach (var t in tokens)
                    if (words.Contains(t, StringComparer.OrdinalIgnoreCase)) hits.Add(t);
            }
            return Ok(new { contains = hits.Count > 0, words = hits });
        }
    }
}
