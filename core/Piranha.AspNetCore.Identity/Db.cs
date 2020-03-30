/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.Data;

namespace Piranha.AspNetCore.Identity
{
    public abstract class Db<T> :
        IdentityDbContext<User, Role, Guid,
            IdentityUserClaim<Guid>,
            IdentityUserRole<Guid>,
            IdentityUserLogin<Guid>,
            IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>>,
        IDb
        where T : Db<T>
    {
        /// <summary>
        ///     Gets/sets whether the db context as been initialized. This
        ///     is only performed once in the application lifecycle.
        /// </summary>
        private static volatile bool IsInitialized;

        /// <summary>
        ///     The object mutext used for initializing the context.
        /// </summary>
        private static readonly object Mutex = new object();

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        protected Db(DbContextOptions<T> options) : base(options)
        {
            if (IsInitialized)
            {
                return;
            }

            lock (Mutex)
            {
                if (IsInitialized)
                {
                    return;
                }

                // Migrate database
                Database.Migrate();

                Seed();

                IsInitialized = true;
            }
        }

        /// <summary>
        ///     Creates and configures the data model.
        /// </summary>
        /// <param name="mb">The current model builder</param>
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<User>().ToTable("Piranha_Users");
            mb.Entity<Role>().ToTable("Piranha_Roles");
            mb.Entity<IdentityUserClaim<Guid>>().ToTable("Piranha_UserClaims");
            mb.Entity<IdentityUserRole<Guid>>().ToTable("Piranha_UserRoles");
            mb.Entity<IdentityUserLogin<Guid>>().ToTable("Piranha_UserLogins");
            mb.Entity<IdentityRoleClaim<Guid>>().ToTable("Piranha_RoleClaims");
            mb.Entity<IdentityUserToken<Guid>>().ToTable("Piranha_UserTokens");
        }

        /// <summary>
        ///     Seeds the default data.
        /// </summary>
        private void Seed()
        {
            SaveChanges();

            // Make sure we have a SysAdmin role
            var role = Roles.FirstOrDefault(r => r.NormalizedName == "SYSADMIN");
            if (role == null)
            {
                role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "SysAdmin",
                    NormalizedName = "SYSADMIN"
                };
                Roles.Add(role);
            }

            // Make sure our SysAdmin role has all of the available claims
            //foreach (var claim in Piranha.Security.Permission.All())
            foreach (var permission in App.Permissions.GetPermissions())
            {
                var roleClaim = RoleClaims.FirstOrDefault(c =>
                    c.RoleId == role.Id && c.ClaimType == permission.Name && c.ClaimValue == permission.Name);
                if (roleClaim == null)
                {
                    RoleClaims.Add(new IdentityRoleClaim<Guid>
                    {
                        RoleId = role.Id,
                        ClaimType = permission.Name,
                        ClaimValue = permission.Name
                    });
                }
            }

            SaveChanges();
        }
    }
}