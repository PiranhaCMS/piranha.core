namespace ContentRus.Onboarding.Services
{
    public abstract class BaseEvent
    {
        public string Type { get; set; } = string.Empty;
    }

    public class PaymentConfirmedEvent : BaseEvent
    {
        public string Plan { get; set; }
        public string Status { get; set; }
        public string TenantID { get; set; }
    }
    
    public class DeploymentStatusEvent : BaseEvent
    {
        public string TenantID { get; set; }
        public string Status { get; set; }
    }
}
