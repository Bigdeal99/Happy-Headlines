using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace PublisherService.Messaging;

public interface IArticlePublisher
{
    void Publish(object payload);
}

public class ArticlePublisher : IArticlePublisher, IDisposable
{
    private readonly IConnection _conn;
    private readonly IModel _ch;
    private readonly string _exchange = "articles.exchange";
    private readonly string _queue = "article-queue";

    public ArticlePublisher(IConfiguration cfg)
    {
        var host = cfg["RABBITMQ_HOST"] ?? "rabbitmq";
        var factory = new ConnectionFactory { HostName = host };
        _conn = factory.CreateConnection();
        _ch = _conn.CreateModel();

        _ch.ExchangeDeclare(_exchange, ExchangeType.Fanout, durable: true);
        _ch.QueueDeclare(_queue, durable: true, exclusive: false, autoDelete: false);
        _ch.QueueBind(_queue, _exchange, routingKey: "");
    }

    public void Publish(object payload)
    {
        var props = _ch.CreateBasicProperties();
        props.Persistent = true;

        // propagate W3C trace context
        var traceparent = Activity.Current?.Id;
        if (traceparent != null)
        {
            props.Headers ??= new Dictionary<string, object>();
            props.Headers["traceparent"] = Encoding.UTF8.GetBytes(traceparent);
        }

        var body = JsonSerializer.SerializeToUtf8Bytes(payload);
        _ch.BasicPublish(_exchange, routingKey: "", props, body);
    }

    public void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
    }
}
