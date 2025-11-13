using StackExchange.Redis;

namespace SubscriberService.Services;

public class FeatureFlagService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<FeatureFlagService> _logger;
    private const string FeatureFlagKey = "feature:subscriber-service:enabled";

    public FeatureFlagService(IConnectionMultiplexer redis, ILogger<FeatureFlagService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> IsEnabledAsync()
    {
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(FeatureFlagKey);
            
            // Default to enabled if not set
            if (!value.HasValue)
            {
                await SetEnabledAsync(true);
                return true;
            }

            return value == "true" || value == "1";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature flag, defaulting to enabled");
            return true; // Fail open - default to enabled
        }
    }

    public async Task SetEnabledAsync(bool enabled)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(FeatureFlagKey, enabled ? "true" : "false");
            _logger.LogInformation("Feature flag set to {Enabled}", enabled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting feature flag");
            throw;
        }
    }
}

