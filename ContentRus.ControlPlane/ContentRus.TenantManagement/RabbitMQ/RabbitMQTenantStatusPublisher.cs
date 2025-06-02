using RabbitMQ.Client;

namespace ContentRus.TenantManagement.RabbitMQ
{
    public class RabbitMQTenantStatusPublisher : RabbitMqProvisioningPublisher
    {
        private const string TENANT_STATUS_QUEUE = "tenantStatus";

        public RabbitMQTenantStatusPublisher() : base(TENANT_STATUS_QUEUE)
        {
        }
    }
}