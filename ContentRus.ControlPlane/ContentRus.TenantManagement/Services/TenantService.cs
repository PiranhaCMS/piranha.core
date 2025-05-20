using System;
using System.Collections.Generic;
using System.Linq;
using ContentRus.TenantManagement.Models;

namespace ContentRus.TenantManagement.Services
{
    public class TenantService
    {
        private readonly List<Tenant> _tenants = new();

        public Tenant CreateTenant(string name, string email, TenantTier tier)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                Tier = tier,
                State = TenantState.Created,
                CreatedAt = DateTime.UtcNow
            };

            _tenants.Add(tenant);
            return tenant;
        }

        public bool UpdateTenantState(Guid id, TenantState newState)
        {
            var tenant = GetTenant(id);
            if (tenant == null) return false;

            tenant.State = newState;
            return true;
        }


        public Tenant? GetTenant(Guid id)
        {
            return _tenants.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Tenant> GetAllTenants()
        {
            return _tenants;
        }
    }
}
