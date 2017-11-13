using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Piranha.AspNetCore.Identity.EntityFramework
{
    public class IdentityDb : IdentityDbContext<IdentityAppUser>
    {
        public IdentityDb(DbContextOptions<IdentityDb> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityAppUser>().Ignore(t => t.Claims);
            builder.Entity<IdentityAppUser>().Ignore(t => t.Password);
            base.OnModelCreating(builder);
        }

        
    }
}