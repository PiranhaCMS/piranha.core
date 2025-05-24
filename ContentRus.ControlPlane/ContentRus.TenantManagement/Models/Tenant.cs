using System;

namespace ContentRus.TenantManagement.Models
{
    public class Tenant
    {
        public required Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public required TenantTier Tier { get; set; }
        public required TenantState State { get; set; } = TenantState.Created;
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
