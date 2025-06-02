using RabbitMQ.Client;

namespace ContentRus.TenantManagement.RabbitMQ
{
    public class RabbitMQNotificationPublisher : RabbitMqProvisioningPublisher
    {
        private const string NOTIFICATIONS_QUEUE = "notifications";

        public RabbitMQNotificationPublisher() : base(NOTIFICATIONS_QUEUE)
        {
        }
    }
}