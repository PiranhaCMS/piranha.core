using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Notifications.Services;


public class RabbitMQNotificationConsumerService : BackgroundService, IDisposable
{
    private const string notifications_queue_name = "notifications";
    private IConnection _connection;
    private IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    private readonly RabbitMqSettings _settings;

    public RabbitMQNotificationConsumerService(IServiceProvider serviceProvider, IOptions<RabbitMqSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: notifications_queue_name,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (sender, ea) =>
        {

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");

            // var subscriptionEvent = JsonSerializer.Deserialize<PaymentConfirmedEvent>(message);

            // lacks sending email

            await Task.CompletedTask;
        };


        await _channel.BasicConsumeAsync(
            queue: notifications_queue_name,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
