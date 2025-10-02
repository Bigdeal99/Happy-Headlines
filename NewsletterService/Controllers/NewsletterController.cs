using Microsoft.AspNetCore.Mvc;

namespace NewsletterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly IHttpClientFactory _http;

    public NewsletterController(IHttpClientFactory http) => _http = http;

    // Demo: request latest articles from ArticleService (traced over HTTP)
    [HttpPost("send")]
    public async Task<IActionResult> Send()
    {
        var client = _http.CreateClient("article");
        var res = await client.GetAsync("/api/articles?top=5");
        if (!res.IsSuccessStatusCode) return StatusCode(502, "ArticleService unavailable");

        var payload = await res.Content.ReadAsStringAsync();
        // In real life you'd email; here we just return the preview
        return Ok(new { sent = true, preview = payload });
    }
}
