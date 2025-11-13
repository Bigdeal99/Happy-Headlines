using System.Net.Http.Json;
using System.Text.Json;

namespace NewsletterService.Services;

public class SubscriberClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SubscriberClient> _logger;

    public SubscriberClient(HttpClient httpClient, ILogger<SubscriberClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<SubscriberInfo>> GetSubscribersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/subscribers?activeOnly=true", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogWarning("SubscriberService is disabled via feature flag");
                return new List<SubscriberInfo>(); // Return empty list when disabled
            }

            response.EnsureSuccessStatusCode();
            var subscribers = await response.Content.ReadFromJsonAsync<List<SubscriberInfo>>(cancellationToken: cancellationToken);
            return subscribers ?? new List<SubscriberInfo>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling SubscriberService");
            throw;
        }
    }

    public async Task<bool> IsFeatureEnabledAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/subscribers/feature-flag", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                return false;
            }
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FeatureFlagResponse>(cancellationToken: cancellationToken);
            return result?.Enabled ?? false;
        }
        catch
        {
            return false; // Fail closed - assume disabled if can't check
        }
    }
}

public class SubscriberInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime SubscribedAt { get; set; }
    public bool IsActive { get; set; }
}

public class FeatureFlagResponse
{
    public bool Enabled { get; set; }
}

