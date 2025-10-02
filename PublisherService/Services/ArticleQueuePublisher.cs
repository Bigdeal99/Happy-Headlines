using System.Diagnostics;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OpenTelemetry.Context.Propagation;

namespace PublisherService.Services;

public interface IArticleQueuePublisher
{
    void Publish(object msg);
}

public class ArticleQueuePublisher : IArticleQueuePublisher, IDisposable
{
    private readonly IConnection _conn;
    private readonly IModel _channel;
    private static readonly ActivitySource ActivitySrc = new("PublisherService");

    // Use TraceContextPropagator
    private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

    public ArticleQueuePublisher(IConfiguration cfg)
    {
        var factory = new ConnectionFactory
        {
            HostName = cfg["RABBIT_HOST"] ?? "rabbitmq",
            Port = int.TryParse(cfg["RABBIT_PORT"], out var p) ? p : 5672,
            UserName = cfg["RABBIT_USER"] ?? "guest",
            Password = cfg["RABBIT_PASS"] ?? "guest"
        };

        _conn = factory.CreateConnection();
        _channel = _conn.CreateModel();
        _channel.QueueDeclare(queue: "articles", durable: true, exclusive: false, autoDelete: false);
    }

    public void Publish(object msg)
    {
        using var activity = ActivitySrc.StartActivity("Publish Article", ActivityKind.Producer);

        var body = JsonSerializer.SerializeToUtf8Bytes(msg);
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        // inject W3C trace context into headers (no Baggage needed)
        props.Headers ??= new Dictionary<string, object>();
        Propagator.Inject(
            new PropagationContext(Activity.Current?.Context ?? default, default),
            props.Headers,
            (hdrs, key, value) => hdrs[key] = Encoding.UTF8.GetBytes(value)
        );

        _channel.BasicPublish(exchange: "", routingKey: "articles", basicProperties: props, body: body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _conn?.Close();
    }
}
