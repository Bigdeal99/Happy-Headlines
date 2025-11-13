using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriberService.Data;
using SubscriberService.Models;
using SubscriberService.Services;

namespace SubscriberService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscribersController : ControllerBase
{
    private readonly SubscriberDbContext _context;
    private readonly SubscriberQueueService _queueService;
    private readonly FeatureFlagService _featureFlag;
    private readonly ILogger<SubscribersController> _logger;

    public SubscribersController(
        SubscriberDbContext context,
        SubscriberQueueService queueService,
        FeatureFlagService featureFlag,
        ILogger<SubscribersController> logger)
    {
        _context = context;
        _queueService = queueService;
        _featureFlag = featureFlag;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        // Check feature flag
        var isEnabled = await _featureFlag.IsEnabledAsync();
        if (!isEnabled)
        {
            _logger.LogWarning("SubscriberService is disabled via feature flag");
            return StatusCode(503, new { error = "SubscriberService is currently disabled" });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        try
        {
            // Check if already subscribed
            var existing = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.Email == request.Email);

            if (existing != null)
            {
                if (existing.IsActive)
                {
                    return Conflict(new { error = "Email already subscribed" });
                }
                // Reactivate
                existing.IsActive = true;
                existing.SubscribedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new subscriber
                existing = new Subscriber
                {
                    Email = request.Email,
                    Name = request.Name ?? string.Empty,
                    SubscribedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Subscribers.Add(existing);
            }

            await _context.SaveChangesAsync();

            // Publish to queue for welcome email
            _queueService.PublishSubscriber(existing.Id, existing.Email, existing.Name);

            _logger.LogInformation("Subscriber {Email} subscribed (ID: {Id})", existing.Email, existing.Id);
            return Ok(new { id = existing.Id, email = existing.Email, subscribedAt = existing.SubscribedAt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing {Email}", request.Email);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> Unsubscribe(string email)
    {
        // Check feature flag
        var isEnabled = await _featureFlag.IsEnabledAsync();
        if (!isEnabled)
        {
            _logger.LogWarning("SubscriberService is disabled via feature flag");
            return StatusCode(503, new { error = "SubscriberService is currently disabled" });
        }

        try
        {
            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.Email == email);

            if (subscriber == null || !subscriber.IsActive)
            {
                return NotFound(new { error = "Subscriber not found" });
            }

            subscriber.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscriber {Email} unsubscribed", email);
            return Ok(new { message = "Unsubscribed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing {Email}", email);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSubscribers([FromQuery] bool activeOnly = true)
    {
        // Check feature flag
        var isEnabled = await _featureFlag.IsEnabledAsync();
        if (!isEnabled)
        {
            _logger.LogWarning("SubscriberService is disabled via feature flag");
            return StatusCode(503, new { error = "SubscriberService is currently disabled" });
        }

        try
        {
            var query = _context.Subscribers.AsQueryable();
            if (activeOnly)
            {
                query = query.Where(s => s.IsActive);
            }

            var subscribers = await query
                .Select(s => new { s.Id, s.Email, s.Name, s.SubscribedAt, s.IsActive })
                .ToListAsync();

            return Ok(subscribers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscribers");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("feature-flag")]
    public async Task<IActionResult> GetFeatureFlag()
    {
        var isEnabled = await _featureFlag.IsEnabledAsync();
        return Ok(new { enabled = isEnabled });
    }

    [HttpPost("feature-flag")]
    public async Task<IActionResult> SetFeatureFlag([FromBody] FeatureFlagRequest request)
    {
        await _featureFlag.SetEnabledAsync(request.Enabled);
        return Ok(new { enabled = request.Enabled, message = "Feature flag updated" });
    }
}

public class SubscribeRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public class FeatureFlagRequest
{
    public bool Enabled { get; set; }
}

