using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ArticleService.Data;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Services;

public class ArticleQueueConsumer : BackgroundService
{
    private readonly IServiceProvider _sp;
    private IConnection? _conn;
    private IModel? _channel;

    private static readonly ActivitySource ActivitySrc = new("ArticleService");
    private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

    public ArticleQueueConsumer(IServiceProvider sp) => _sp = sp;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBIT_HOST") ?? "rabbitmq",
            UserName = Environment.GetEnvironmentVariable("RABBIT_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBIT_PASS") ?? "guest",
            DispatchConsumersAsync = true
        };

        _conn = factory.CreateConnection();
        _channel = _conn.CreateModel();
        _channel.QueueDeclare("articles", durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (ch, ea) =>
        {
            var parentContext = Propagator.Extract(default, ea.BasicProperties.Headers,
                (hdrs, key) => hdrs != null && hdrs.TryGetValue(key, out var val)
                    ? new[] { Encoding.UTF8.GetString((byte[])val) }
                    : Array.Empty<string>());

            using var activity = ActivitySrc.StartActivity(
                "Consume Article",
                ActivityKind.Consumer,
                parentContext.ActivityContext);

            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var msg = JsonSerializer.Deserialize<ArticleMessage>(json)!;

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

            db.Articles.Add(new Article
            {
                Title = msg.Title,
                Content = msg.Content,
                Continent = msg.Continent,
                PublishedAt = msg.PublishedAt
            });
            await db.SaveChangesAsync(stoppingToken);

            _channel!.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicQos(0, 10, false);
        _channel.BasicConsume("articles", autoAck: false, consumer);
        return Task.CompletedTask;
    }

    public record ArticleMessage(string Title, string Content, string Continent, DateTime PublishedAt);

    public override void Dispose()
    {
        _channel?.Close();
        _conn?.Close();
        base.Dispose();
    }
}
