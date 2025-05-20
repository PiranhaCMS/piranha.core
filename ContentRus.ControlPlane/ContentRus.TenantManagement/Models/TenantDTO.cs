using System;

namespace ContentRus.TenantManagement.Models
{
    public class TenantDTO
    // used for POST request to create a new tenant
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required TenantTier Tier { get; set; }
    }
}
