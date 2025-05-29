using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using ContentRus.TenantManagement.Models;
using ContentRus.TenantManagement.Services;
using ContentRus.TenantManagement.Controllers;

public class RabbitMqConsumerService : BackgroundService, IDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: "event_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantService = scope.ServiceProvider.GetRequiredService<TenantService>();
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");

            var subscriptionEvent = JsonSerializer.Deserialize<PaymentConfirmedEvent>(message);

            Console.WriteLine(subscriptionEvent.Plan);

            Console.WriteLine(subscriptionEvent.TenantID);


            Guid id = Guid.Parse(subscriptionEvent.TenantID);

            if(subscriptionEvent.Plan=="Plano Grupo"){
                tenantService.UpdateTenantTier(id,TenantTier.Pro);
                tenantService.UpdateTenantState(id,TenantState.Active);
            }
            else if(subscriptionEvent.Plan=="Plano Básico"){
                tenantService.UpdateTenantTier(id,TenantTier.Basic);
                tenantService.UpdateTenantState(id,TenantState.Active);

            }
            else if(subscriptionEvent.Plan=="Plano Enterprise"){
                tenantService.UpdateTenantTier(id,TenantTier.Enterprise);
                tenantService.UpdateTenantState(id,TenantState.Active);
            }

            await Task.CompletedTask;
        };


        await _channel.BasicConsumeAsync(
            queue: "event_queue",
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
