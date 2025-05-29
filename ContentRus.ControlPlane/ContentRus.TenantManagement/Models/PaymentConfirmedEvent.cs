namespace ContentRus.TenantManagement.Models{
    public class PaymentConfirmedEvent
    {
        public string Plan { get; set; }
        public string Status { get; set; }
        public string TenantID {get; set; }
    }
}
