using Microsoft.EntityFrameworkCore;
using ContentRus.TenantManagement.Models;
using System.Text.Json;

namespace ContentRus.TenantManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantPlan> TenantPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TenantPlan>()
            .Property(p => p.FeaturesJson)
            .HasColumnName("Features");
    }
}
