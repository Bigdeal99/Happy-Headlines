using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SubscriberService.Services;

public class SubscriberQueueService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<SubscriberQueueService> _logger;
    private const string QueueName = "subscriber_queue";

    public SubscriberQueueService(ILogger<SubscriberQueueService> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBIT_HOST") ?? "rabbitmq",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        _logger.LogInformation("SubscriberQueueService initialized");
    }

    public void PublishSubscriber(int subscriberId, string email, string name)
    {
        try
        {
            var message = new
            {
                SubscriberId = subscriberId,
                Email = email,
                Name = name,
                Timestamp = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: "",
                routingKey: QueueName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published subscriber {SubscriberId} ({Email}) to queue", subscriberId, email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish subscriber to queue");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

