using Prometheus;

namespace CommentService.Services
{
    public static class AppMetrics
    {
        public static readonly CollectorRegistry Registry = Metrics.NewCustomRegistry();
        private static readonly MetricFactory Factory = Metrics.WithCustomRegistry(Registry);

        public static readonly Counter RequestsTotal = Factory.CreateCounter(
            "hh_requests_comment_total",
            "Total comment API requests processed");

        public static readonly Counter CommentCacheHit = Factory.CreateCounter(
            "comment_cache_hit",
            "Comment cache hits");

        public static readonly Counter CommentCacheMiss = Factory.CreateCounter(
            "comment_cache_miss",
            "Comment cache misses");
    }
}


