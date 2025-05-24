using System;

namespace ContentRus.TenantManagement.Models
{
    public class TenantInfoDTO
    // used for POST request to update tenant information
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
    }
}
