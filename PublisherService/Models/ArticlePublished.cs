namespace PublisherService.Models;

public class ArticlePublished
{
    public int Id { get; set; }        
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Continent { get; set; } = "Global";
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
