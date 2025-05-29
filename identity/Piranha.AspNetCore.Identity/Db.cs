/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Manager;

namespace Piranha.AspNetCore.Identity;

public abstract class Db<T>
    : IdentityDbContext<
        User,
        Role,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >,
        IDb
    where T : Db<T>
{
    /// <summary>
    ///     Gets/sets whether the db context as been initialized. This
    ///     is only performed once in the application lifecycle.
    /// </summary>
    private static volatile bool IsInitialized;

    /// <summary>
    ///     The object mutex used for initializing the context.
    /// </summary>
    private static readonly object Mutex = new object();

    /// <summary>
    ///     Default constructor.
    /// </summary>
    /// <param name="options">Configuration options</param>
    protected Db(DbContextOptions<T> options)
        : base(options)
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
    /// Seeds the default data.
    /// </summary>
    private void Seed()
    {
        SaveChanges();

        // Make sure we have a SysAdmin role
        var sysAdminRole = Roles.FirstOrDefault(r => r.NormalizedName == "SYSADMIN");
        if (sysAdminRole == null)
        {
            sysAdminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "SysAdmin",
                NormalizedName = "SYSADMIN",
            };
            Roles.Add(sysAdminRole);
        }

        // Make sure we have an Author role
        var authorRole = Roles.FirstOrDefault(r => r.NormalizedName == "AUTHOR");
        if (authorRole == null)
        {
            authorRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Author",
                NormalizedName = "AUTHOR",
            };
            Roles.Add(authorRole);
        }

        // Make sure we have a Reviewer role
        var reviewerRole = Roles.FirstOrDefault(r => r.NormalizedName == "REVIEWER");
        if (reviewerRole == null)
        {
            reviewerRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Reviewer",
                NormalizedName = "REVIEWER",
            };
            Roles.Add(reviewerRole);
        }

        // Make sure we have a LegalTeam role
        var legalTeamRole = Roles.FirstOrDefault(r => r.NormalizedName == "LEGALTEAM");
        if (legalTeamRole == null)
        {
            legalTeamRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "LegalTeam",
                NormalizedName = "LEGALTEAM",
            };
            Roles.Add(legalTeamRole);
        }

        // Assign permissions to SysAdmin role (all permissions)
        foreach (var permission in App.Permissions.GetPermissions())
        {
            var roleClaim = RoleClaims.FirstOrDefault(c =>
                c.RoleId == sysAdminRole.Id
                && c.ClaimType == permission.Name
                && c.ClaimValue == permission.Name
            );
            if (roleClaim == null)
            {
                RoleClaims.Add(
                    new IdentityRoleClaim<Guid>
                    {
                        RoleId = sysAdminRole.Id,
                        ClaimType = permission.Name,
                        ClaimValue = permission.Name,
                    }
                );
            }
        }

        // Assign permissions to Author role
        var authorPermissions = new[]
        {
            "PiranhaContent",
            "PiranhaContentAdd",
            "PiranhaContentEdit",
            "PiranhaContentSave",
            "PiranhaPages",
            "PiranhaPagesAdd",
            "PiranhaPagesEdit",
            "PiranhaPagesSave",
            "PiranhaPosts",
            "PiranhaPostsAdd",
            "PiranhaPostsEdit",
            "PiranhaPostsSave",
            "PiranhaMedia",
            "PiranhaMediaAdd",
            "PiranhaMediaEdit",
        };

        foreach (var permissionName in authorPermissions)
        {
            var roleClaim = RoleClaims.FirstOrDefault(c =>
                c.RoleId == authorRole.Id
                && c.ClaimType == permissionName
                && c.ClaimValue == permissionName
            );
            if (roleClaim == null)
            {
                RoleClaims.Add(
                    new IdentityRoleClaim<Guid>
                    {
                        RoleId = authorRole.Id,
                        ClaimType = permissionName,
                        ClaimValue = permissionName,
                    }
                );
            }
        }

        // Assign permissions to Reviewer role
        var reviewerPermissions = new[]
        {
            "PiranhaReviewer",
            "PiranhaContentReview",
            "PiranhaContent",
            "PiranhaContentEdit",
            "PiranhaPages",
            "PiranhaPagesEdit",
            "PiranhaPagesPublish",
            "PiranhaPosts",
            "PiranhaPostsEdit",
            "PiranhaPostsPublish",
        };

        foreach (var permissionName in reviewerPermissions)
        {
            var roleClaim = RoleClaims.FirstOrDefault(c =>
                c.RoleId == reviewerRole.Id
                && c.ClaimType == permissionName
                && c.ClaimValue == permissionName
            );
            if (roleClaim == null)
            {
                RoleClaims.Add(
                    new IdentityRoleClaim<Guid>
                    {
                        RoleId = reviewerRole.Id,
                        ClaimType = permissionName,
                        ClaimValue = permissionName,
                    }
                );
            }
        }

        // Assign permissions to LegalTeam role
        var legalTeamPermissions = new[]
        {
            "PiranhaLegalTeam",
            "PiranhaLegalTeamReview",
            "PiranhaLegalTeamDeny",
            "PiranhaContentReview",
            "PiranhaContent",
            "PiranhaContentEdit",
            "PiranhaPages",
            "PiranhaPagesEdit",
            "PiranhaPagesPublish",
            "PiranhaPosts",
            "PiranhaPostsEdit",
            "PiranhaPostsPublish",
        };

        foreach (var permissionName in legalTeamPermissions)
        {
            var roleClaim = RoleClaims.FirstOrDefault(c =>
                c.RoleId == legalTeamRole.Id
                && c.ClaimType == permissionName
                && c.ClaimValue == permissionName
            );
            if (roleClaim == null)
            {
                RoleClaims.Add(
                    new IdentityRoleClaim<Guid>
                    {
                        RoleId = legalTeamRole.Id,
                        ClaimType = permissionName,
                        ClaimValue = permissionName,
                    }
                );
            }
        }

        SaveChanges();
    }
}
