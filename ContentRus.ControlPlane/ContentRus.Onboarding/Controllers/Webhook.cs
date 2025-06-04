using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentRus.Onboarding.Services;

[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor for Media Controller.
    /// </summary>
    /// <param name="configuration">The configuration settings for the application.</param>
    public WebhookController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Webhook to handle deployment status updates from ArgoCD, which communicates through RabbitMQ to the appropriate services.
    /// </summary> 
    /// <param name="deploymentMessage">The deployment message containing status updates.</param>
    [HttpPost("deployment-status")]
    public async Task<IActionResult> setDeploymentStatus([FromBody] DeploymentMessage deploymentMessage)
    {
        if (deploymentMessage == null)
            return BadRequest("Invalid deployment message");

        var status = deploymentMessage.health.ToLower() == "healthy" 
            ? "success" 
            : "failed";

        var deploymentStatus = new DeploymentStatusEvent
        {
            Type = "deployment",
            Status = status,
            TenantID = deploymentMessage.tenantNamespace.Substring(1), // Remove first character ('t' from 't1')
        };

        var serializedMessage = System.Text.Json.JsonSerializer.Serialize(deploymentStatus);
        await TenantStatusPublisher.PublishAsync(serializedMessage);

        return Ok(new { message = "Deployment status comunicated successfully." });
    }

    /// <summary>
    /// Deployment status updates from ArgoCD.
    /// </summary>
    public class DeploymentMessage
    {
        /// <summary>
        /// The name of the argoCD application.
        /// </summary>
        required public string app { get; set; }

        /// <summary>
        /// The sync status of the deployment.
        /// </summary>
        required public string status { get; set; }

        /// <summary>
        /// The health status of the deployment.
        /// </summary>
        required public string health { get; set; }

        /// <summary>
        /// The namespace of the tenant where the deployment is happening.
        /// </summary>
        required public string tenantNamespace { get; set; }

        /// <summary>
        /// The timestamp of the deployment status update.
        /// </summary>
        required public string timestamp { get; set; }
    }
    
}