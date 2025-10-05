using Prometheus;

namespace ArticleService.Services
{
    public static class AppMetrics
    {
        public static readonly CollectorRegistry Registry = Metrics.NewCustomRegistry();

        private static readonly MetricFactory Factory = Metrics.WithCustomRegistry(Registry);

        public static readonly Counter RequestsTotal = Factory.CreateCounter(
            "hh_requests_total",
            "Total API requests processed (articles)");

        public static readonly Counter ArticleCacheHit = Factory.CreateCounter(
            "article_cache_hit",
            "Article cache hits");

        public static readonly Counter ArticleCacheMiss = Factory.CreateCounter(
            "article_cache_miss",
            "Article cache misses");
    }
}


