using Prometheus;

namespace CommentService.Services
{
    public sealed class CommentMetricSet
    {
        public CommentMetricSet(Counter hits, Counter misses)
        {
            Hits = hits; Misses = misses;
        }
        public Counter Hits { get; }
        public Counter Misses { get; }
    }
}


