using System;
using System.Collections.Generic;
using System.Linq;
using ContentRus.TenantManagement.Models;

namespace ContentRus.TenantManagement.Services
{
    public class TenantService
    {
        private readonly List<Tenant> _tenants = new();

        public Tenant CreateTenant(string email)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = email,
                Email = email,
                Tier = TenantTier.Basic,
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

        public bool UpdateTenantTier(Guid id, TenantTier newTier)
        {
            var tenant = GetTenant(id);
            if (tenant == null) return false;

            tenant.Tier = newTier;
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
