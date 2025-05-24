namespace ContentRus.TenantManagement.Models
{
    public enum TenantState
    {
        Created = 1,            // INITIAL STATE - WAITING FOR PAYMENT
        Active = 2,             // ACTIVE AFTER PAYMENT
        Cancelled = 3           // CANCELLED
    }
}