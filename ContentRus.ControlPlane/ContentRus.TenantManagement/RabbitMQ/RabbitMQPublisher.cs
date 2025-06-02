using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

public class RabbitMqProvisioningPublisher : IAsyncDisposable
{
    private readonly string _queueName;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqProvisioningPublisher(string queueName)
    {
        _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));

        var factory = new ConnectionFactory() { HostName = "localhost" };

        // Since constructors can't be async, use `.Result` for quick setup (or refactor if needed)
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
    }

    public async Task PublishAsync<T>(T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.QueueDeclareAsync(
            queue: tenant_status_queue_name,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await _channel.BasicPublishAsync<BasicProperties>(
            exchange: "",
            routingKey: tenant_status_queue_name,
            mandatory: true,
            basicProperties: properties,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();
    }
}
