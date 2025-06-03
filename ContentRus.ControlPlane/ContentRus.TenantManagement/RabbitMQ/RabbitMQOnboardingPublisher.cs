using RabbitMQ.Client;

namespace ContentRus.TenantManagement.RabbitMQ
{
    public class RabbitMQOnboardingPublisher : RabbitMqProvisioningPublisher
    {
        private const string TENANT_STATUS_QUEUE = "provisioning";

        public RabbitMQOnboardingPublisher() : base(TENANT_STATUS_QUEUE)
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