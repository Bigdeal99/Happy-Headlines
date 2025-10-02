namespace DraftService.Models;

public class Draft
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? Author { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
