using Prometheus;

namespace ArticleService.Services
{
    public sealed class ArticleMetricSet
    {
        public ArticleMetricSet(Counter hits, Counter misses)
        {
            Hits = hits;
            Misses = misses;
        }

        public Counter Hits { get; }
        public Counter Misses { get; }
    }
}


