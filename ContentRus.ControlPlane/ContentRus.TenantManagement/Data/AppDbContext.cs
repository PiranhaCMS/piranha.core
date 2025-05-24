using Microsoft.EntityFrameworkCore;
using ContentRus.TenantManagement.Models;

namespace ContentRus.TenantManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
}
