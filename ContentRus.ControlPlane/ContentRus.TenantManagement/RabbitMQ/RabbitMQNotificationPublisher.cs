using RabbitMQ.Client;
using Microsoft.Extensions.Options;

namespace ContentRus.TenantManagement.RabbitMQ
{
    public class RabbitMQNotificationPublisher : RabbitMqPublisher
    {
        private const string NOTIFICATIONS_QUEUE = "notifications";

        public RabbitMQNotificationPublisher(IOptions<RabbitMqSettings> settings) : base(settings, NOTIFICATIONS_QUEUE)
        {
        }
    }
}