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
using ContentRus.TenantManagement.RabbitMQ;
using Microsoft.Extensions.Options;


public class RabbitMqConsumerService : BackgroundService, IDisposable
{
    private const string tenant_status_queue_name = "tenant_status";
    private IConnection _connection;
    private IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    private readonly RabbitMqSettings _settings;

    public RabbitMqConsumerService(IServiceProvider serviceProvider, IOptions<RabbitMqSettings> settings)
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
            queue: tenant_status_queue_name,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            var tenantService = scope.ServiceProvider.GetRequiredService<TenantService>();
            var onboardingPublisher = scope.ServiceProvider.GetRequiredService<RabbitMQOnboardingPublisher>();
            var notificationPublisher = scope.ServiceProvider.GetRequiredService<RabbitMQNotificationPublisher>();

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");

            using var jsonDoc = JsonDocument.Parse(message);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("Type", out var typeProperty))
            {
                Console.WriteLine(" [!] Missing 'Type' property in message.");
                return;
            }

            var typeValue = typeProperty.GetString()?.ToLowerInvariant();

            switch (typeValue)
            {
                case "payment":
                    var subscriptionEvent = JsonSerializer.Deserialize<PaymentConfirmedEvent>(message);

                    if (!Guid.TryParse(subscriptionEvent.TenantID, out Guid tenantGuid))
                    {
                        Console.WriteLine(" [!] Invalid tenant ID");
                        return;
                    }

                    if (subscriptionEvent.Status == "subscription created")
                    {
                        await onboardingPublisher.SendProvisioningRequestAsync(tenantGuid);
                        Console.WriteLine(" [>] Sent provisioning request to onboarding queue");
                    }


                    //Console.WriteLine(subscriptionEvent.Plan);

                    //Console.WriteLine(subscriptionEvent.TenantID);


                    Guid id = Guid.Parse(subscriptionEvent.TenantID);

                    if (subscriptionEvent.Plan == "Plano Grupo")
                    {
                        tenantService.UpdateTenantTier(id, TenantTier.Pro);
                        tenantService.UpdateTenantState(id, TenantState.Active);
                    }
                    else if (subscriptionEvent.Plan == "Plano Básico")
                    {
                        tenantService.UpdateTenantTier(id, TenantTier.Basic);
                        tenantService.UpdateTenantState(id, TenantState.Active);
                    }
                    else if (subscriptionEvent.Plan == "Plano Enterprise")
                    {
                        tenantService.UpdateTenantTier(id, TenantTier.Enterprise);
                        tenantService.UpdateTenantState(id, TenantState.Active);
                    }
                    break;
                case "deployment":

                    var deploymentEvent = JsonSerializer.Deserialize<DeploymentStatusEvent>(message);
                    if (!Guid.TryParse(deploymentEvent.TenantID, out Guid tenantGuid2))
                    {
                        Console.WriteLine(" [!] Invalid tenant ID");
                        return;
                    }

                    var tenantId = Guid.Parse(deploymentEvent.TenantID);
                    var tenantUser = userService.GetUserByTenantId(tenantId);
                    if (tenantUser != null)
                    {
                        if (deploymentEvent.Status == "success")
                        {
                            tenantService.UpdateTenantState(tenantId, TenantState.DeploymentSuccess);
                            Console.WriteLine(" [A] Deployment successful for tenant: " + tenantId);
                            await notificationPublisher.PublishAsync(new EmailEvent
                            {
                                To = tenantUser.Email,
                                Subject = "Deployment Successful",
                                Body = $"Congratulations! Your deployment (tenant {tenantId}) was successful and is now available."
                            });
                            Console.WriteLine(" [B] Email Deployment successful for tenant: " + tenantId);
                        }
                        else if (deploymentEvent.Status == "failed")
                        {
                            tenantService.UpdateTenantState(tenantId, TenantState.DeploymentFailed);
                            Console.WriteLine(" [A] Deployment failed for tenant: " + tenantId);
                            await notificationPublisher.PublishAsync(new EmailEvent
                            {
                                To = tenantUser.Email,
                                Subject = "Deployment Failed",
                                Body = $"Unfortunately, your deployment (tenant {tenantId}) has failed. Please check the logs for more details."
                            });
                            Console.WriteLine(" [B] Email Deployment failed for tenant: " + tenantId);
                        }
                        else
                        {
                            Console.WriteLine(" [!] Unknown deployment status: " + deploymentEvent.Status);
                        }
                    }

                    break;
                default:
                    Console.WriteLine($" [!] Unknown event type: {typeValue}");
                    break;
            }

            await Task.CompletedTask;
        };


        await _channel.BasicConsumeAsync(
            queue: tenant_status_queue_name,
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
