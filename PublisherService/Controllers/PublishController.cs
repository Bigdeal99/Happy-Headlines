using Microsoft.AspNetCore.Mvc;
using PublisherService.Models;
using PublisherService.Services;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly IArticleQueuePublisher _publisher;

    public PublishController(IHttpClientFactory http, IArticleQueuePublisher publisher)
    {
        _http = http;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> Publish(PublishArticleRequest req)
    {
        // call ProfanityService first (distributed trace via HttpClient instrumentation)
        var client = _http.CreateClient("profanity");
        var check = await client.GetAsync($"/api/profanity/check?text={Uri.EscapeDataString(req.Title + " " + req.Content)}");
        if (!check.IsSuccessStatusCode)
            return StatusCode(502, "ProfanityService unavailable");

        var json = await check.Content.ReadAsStringAsync();
        if (json.Contains("\"contains\":true") || json.Contains("true"))
            return BadRequest("Article rejected due to profanity.");

        // put to queue
        var msg = new { req.Title, req.Content, req.Continent, PublishedAt = DateTime.UtcNow };
        _publisher.Publish(msg);

        return Accepted(new { status = "queued" });
    }
}
