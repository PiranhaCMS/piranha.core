namespace ContentRus.TenantManagement.Models
{
    public enum TenantState
    {
        Created = 1,            // INITIAL STATE - WAITING FOR PAYMENT
        Active = 2,             // ACTIVE AFTER PAYMENT
        Suspended = 3,          // SUSPENDED FOR NON-PAYMENT
        Cancelled = 4           // CANCELLED
    }
}