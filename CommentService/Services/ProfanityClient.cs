using System.Net.Http.Json;

namespace CommentService.Services
{
    public class ProfanityClient : IProfanityClient
    {
        private readonly HttpClient _http;
        private volatile HashSet<string> _fallbackList; // last-known words

        public ProfanityClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            // optional extra fallback from env
            var fallback = (cfg["FALLBACK_PROFANITY_WORDS"] ?? "bad,ugly,stupid")
                          .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            _fallbackList = new HashSet<string>(fallback, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<bool> ContainsProfanity(string text, CancellationToken ct = default)
        {
            // normal path (served by HttpClient + Polly policies)
            var resp = await _http.GetAsync($"/api/profanity/check?text={Uri.EscapeDataString(text)}", ct);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<ProfanityCheckDto>(cancellationToken: ct);
            // cache last-known set (if service returns it)
            if (result?.Words is { Count: >0 }) _fallbackList = result.Words.ToHashSet(StringComparer.OrdinalIgnoreCase);
            return result?.Contains ?? false;
        }

        private sealed record ProfanityCheckDto(bool Contains, List<string> Words);
    }
}
