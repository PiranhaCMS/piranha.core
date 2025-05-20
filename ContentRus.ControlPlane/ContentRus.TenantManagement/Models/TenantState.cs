namespace ContentRus.TenantManagement.Models
{
    public enum TenantState
    {
        Created = 1,            // INITIAL STATE
        WaitingPayment = 2,     // WAITING FOR PAYMENT
        Active = 3,             // ACTIVE AFTER PAYMENT
        Suspended = 4,          // SUSPENDED FOR NON-PAYMENT
        Cancelled = 5           // CANCELLED
    }
}