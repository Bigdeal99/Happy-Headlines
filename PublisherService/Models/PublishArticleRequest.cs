namespace PublisherService.Models;

public class PublishArticleRequest
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Continent { get; set; } = "Global";
}
