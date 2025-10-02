using Microsoft.AspNetCore.Mvc;
using PublisherService.Messaging;
using PublisherService.Models;
using Serilog;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishController : ControllerBase
{
    private readonly IArticlePublisher _publisher;

    public PublishController(IArticlePublisher publisher) => _publisher = publisher;

    [HttpPost]
    public IActionResult Publish(ArticlePublished dto)
    {
        Log.Information("Publishing article '{Title}'", dto.Title);
        dto.PublishedAt = DateTime.UtcNow;
        _publisher.Publish(dto);
        return Accepted(dto);
    }
}
