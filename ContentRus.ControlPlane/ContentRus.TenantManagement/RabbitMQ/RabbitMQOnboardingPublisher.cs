using RabbitMQ.Client;
using Microsoft.Extensions.Options;

namespace ContentRus.TenantManagement.RabbitMQ
{
    public class RabbitMQOnboardingPublisher : RabbitMqPublisher
    {
        private const string TENANT_STATUS_QUEUE = "provisioning";

        public RabbitMQOnboardingPublisher(IOptions<RabbitMqSettings> settings) : base(settings, TENANT_STATUS_QUEUE)
        {

        }

        public async Task SendProvisioningRequestAsync(Guid tenantId)
        {
            var message = new
            {
                type = "provisionRequest",
                data = new
                {
                    tenantId = tenantId
                }
            };

            await PublishAsync(message);
        }
    }
}