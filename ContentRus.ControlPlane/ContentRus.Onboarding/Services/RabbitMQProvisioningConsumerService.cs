using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

/// <summary>
/// Background worker that listens to provisioning messages from RabbitMQ.
/// </summary>
public class RabbitMQProvisioningConsumer : BackgroundService, IDisposable
{
    private const string provisioning_queue_name = "provisioning";
    private readonly ILogger<RabbitMQProvisioningConsumer> _logger;
    private IConnection _connection;
    private IChannel _channel;
    private IKubernetes _kclient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQProvisioningConsumer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    public RabbitMQProvisioningConsumer(ILogger<RabbitMQProvisioningConsumer> logger)
    {
        _logger = logger;
        // in cluster config
        // var config = KubernetesClientConfiguration.InClusterConfig();
        // default kubeconfig in machine
        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
        _kclient = new Kubernetes(config);
    }

    /// <summary>
    /// Executes the worker asynchronously, connecting to RabbitMQ and consuming messages.
    /// </summary>
    /// <param name="stoppingToken">Token to observe for cancellation requests.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            // UserName = "guest",
            // Password = "guest"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: provisioning_queue_name,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message: {Message}", message);

            try
            {
                var messageObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(message);
                if (messageObj == null || !messageObj.ContainsKey("type") || !messageObj.ContainsKey("data"))
                {
                    _logger.LogError("Invalid message format");
                    return;
                }

                if (messageObj["type"].ToString() != "provisionRequest" || 
                    !((JsonElement)messageObj["data"]).TryGetProperty("tenantId", out var _))
                {
                    _logger.LogError("Invalid message type or missing tenantId");
                    return;
                }

                var tenantId = ((JsonElement)messageObj["data"]).GetProperty("tenantId").GetInt32();

                await TriggerArgoWorkflowAsync(tenantId, stoppingToken);
                
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse message");
                return;
            }

        };

        await _channel.BasicConsumeAsync(
            queue: provisioning_queue_name,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

    }

    /// <summary>
    /// Triggers the Argo workflow based on the WorkflowTemplate and tenantId.
    /// </summary>
    private async Task TriggerArgoWorkflowAsync(int tenantId, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Processing provision request for tenant {TenantId}", tenantId);

            var workflow = new
            {
                apiVersion = "argoproj.io/v1alpha1",
                kind = "Workflow",
                metadata = new
                {
                    generateName = $"provision-tenant-{tenantId}-",
                    @namespace = "argowf",
                },
                spec = new
                {
                    workflowTemplateRef = new { name = "tenant-provisioning-with-credentials-template" },
                    arguments = new
                    {
                        parameters = new[]
                        {
                            new { name = "tenantId", value = tenantId.ToString() }
                        }
                    }
                }
            };

            await _kclient.CustomObjects.CreateNamespacedCustomObjectAsync(
                workflow,
                group: "argoproj.io",
                version: "v1alpha1",
                namespaceParameter: "argowf",
                plural: "workflows",
                cancellationToken: ct);

            _logger.LogInformation("Triggered Argo WorkflowTemplate for tenant {TenantId}", tenantId);
        }
        catch (k8s.Autorest.HttpOperationException httpEx)
        {
            _logger.LogError(httpEx, "Kubernetes API error while triggering Argo Workflow for tenant {TenantId}", tenantId);
        }
        catch (HttpRequestException reqEx)
        {
            _logger.LogError(reqEx, "HTTP error while communicating with Kubernetes for tenant {TenantId}", tenantId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Operation was cancelled while triggering workflow for tenant {TenantId}", tenantId);
        }
    }

    /// <summary>
    /// Disposes the RabbitMQ connection and channel.
    /// </summary>
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
