namespace StripeApi.Models{
    public class PaymentConfirmedEvent
    {
        public string Plan { get; set; } = string.Empty;
        public string Status { get; set; } = "confirmed";
        public string TenantID {get; set; } = string.Empty;
    }
}
