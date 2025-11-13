using Microsoft.AspNetCore.Mvc;
using NewsletterService.Services;

namespace NewsletterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly SubscriberClient _subscriberClient;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(
        IHttpClientFactory http,
        SubscriberClient subscriberClient,
        ILogger<NewsletterController> logger)
    {
        _http = http;
        _subscriberClient = subscriberClient;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send()
    {
        // Get articles from ArticleService
        var articleClient = _http.CreateClient("article");
        var articleRes = await articleClient.GetAsync("/api/articles?top=5");
        if (!articleRes.IsSuccessStatusCode)
        {
            return StatusCode(502, new { error = "ArticleService unavailable" });
        }

        var payload = await articleRes.Content.ReadAsStringAsync();

        // Get subscribers from SubscriberService (with fault isolation)
        List<SubscriberInfo> subscribers = new();
        try
        {
            // Check if feature is enabled first
            var isEnabled = await _subscriberClient.IsFeatureEnabledAsync();
            if (!isEnabled)
            {
                _logger.LogInformation("SubscriberService is disabled via feature flag, sending newsletter to no subscribers");
                return Ok(new
                {
                    sent = true,
                    subscribersCount = 0,
                    preview = payload,
                    message = "SubscriberService is disabled"
                });
            }

            subscribers = await _subscriberClient.GetSubscribersAsync();
            _logger.LogInformation("Retrieved {Count} subscribers from SubscriberService", subscribers.Count);
        }
        catch (Exception ex)
        {
            // Fault isolation: Continue even if SubscriberService fails
            _logger.LogWarning(ex, "SubscriberService unavailable, continuing without subscribers (fault isolation)");
            // Continue with empty subscriber list
        }

        // In real life you'd email each subscriber; here we just return the preview
        return Ok(new
        {
            sent = true,
            subscribersCount = subscribers.Count,
            subscribers = subscribers.Select(s => new { s.Email, s.Name }),
            preview = payload
        });
    }
}
