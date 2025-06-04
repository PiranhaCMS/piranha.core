namespace StripeApi.Models{
    public abstract class BaseEvent
    {
        public string Type { get; set; } = string.Empty;
    }

    public class PaymentConfirmedEvent: BaseEvent
    {
        public string Plan { get; set; } = string.Empty;
        public string Status { get; set; } = "Cancelled";
        public string TenantID { get; set; } = string.Empty;
    }
}
