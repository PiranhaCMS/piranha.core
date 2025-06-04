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
            var tenantService = scope.ServiceProvider.GetRequiredService<TenantService>();
            var onboardingPublisher = scope.ServiceProvider.GetRequiredService<RabbitMQOnboardingPublisher>();
            var notificationPublisher = scope.ServiceProvider.GetRequiredService<RabbitMQNotificationPublisher>();

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(message, jsonOptions);

            switch (baseEvent.Type.ToLower())
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
                    break;
                case "deployment":
                    
                    var deploymentEvent = JsonSerializer.Deserialize<DeploymentStatusEvent>(message);
                    if (!Guid.TryParse(subscriptionEvent.TenantID, out Guid tenantGuid))
                    {
                        Console.WriteLine(" [!] Invalid tenant ID");
                        return;
                    }

                    Guid id = Guid.Parse(subscriptionEvent.TenantID);
                    var tenantUser = tenantService.GetUserByTenantId(id);
                    if (tenantUser != null)
                    {
                        var email = tenant.Email;
                        if (deploymentEvent.Status == "success")
                        {
                            tenantService.UpdateTenantState(id, TenantState.DeploymentSuccess);
                            Console.WriteLine(" [A] Deployment successful for tenant: " + id);
                            await notificationPublisher.PublishAsync(new EmailEvent
                            {
                                To = tenantUser.Email,
                                Subject = "Deployment Successful",
                                Body = $"Congratulations! Your deployment (tenant {id}) was successful and is now available."
                            });
                            Console.WriteLine(" [B] Email Deployment successful for tenant: " + id);
                        }
                        else if (deploymentEvent.Status == "failed")
                        {
                            tenantService.UpdateTenantState(id, TenantState.DeploymentFailed);
                            Console.WriteLine(" [A] Deployment failed for tenant: " + id);
                            await notificationPublisher.PublishAsync(new EmailEvent
                            {
                                To = tenantUser.Email,
                                Subject = "Deployment Failed",
                                Body = $"Unfortunately, your deployment (tenant {id}) has failed. Please check the logs for more details."
                            });
                            Console.WriteLine(" [B] Email Deployment failed for tenant: " + id);
                        }
                        else
                        {
                            Console.WriteLine(" [!] Unknown deployment status: " + deploymentEvent.Status);
                        }
                    }

                    break;
                default:
                    Console.WriteLine($" [!] Unknown event type: {baseEvent.Type}");
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
