namespace ArticleService.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Continent { get; set; } = "Global"; // for Z-axis split
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
