/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Piranha.AspNetCore.Identity;

public abstract class Db<T> : IDb
{
    protected readonly IAsyncDocumentSession db;

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
    protected Db(IAsyncDocumentSession db)
    {
        this.db = db;
        if (IsInitialized)
            return;

        lock (Mutex)
        {
            if (IsInitialized)
                return;

            Seed();

            IsInitialized = true;
        }
    }


    /// <summary>
    /// Seeds the default data.
    /// </summary>
    private async Task Seed()
    {
        await SaveChangesAsync();

        // Make sure we have a SysAdmin role
        var role = Roles.FirstOrDefault(r => r.NormalizedName == "SYSADMIN");
        if (role == null)
        {
            role = new Role
            {
                Id = Snowflake.NewId(),
                Name = "SysAdmin",
                NormalizedName = "SYSADMIN"
            };
            await db.StoreAsync(role);
        }

        // Make sure our SysAdmin role has all of the available claims
        //foreach (var claim in Piranha.Security.Permission.All())
        foreach (var permission in App.Permissions.GetPermissions())
        {
            var roleClaim = RoleClaims.FirstOrDefault(c =>
                c.RoleId == role.Id && c.ClaimType == permission.Name && c.ClaimValue == permission.Name);
            if (roleClaim == null)
            {
                // RoleClaims.Add(new IdentityRoleClaim<string>
                // {
                //     RoleId = role.Id,
                //     ClaimType = permission.Name,
                //     ClaimValue = permission.Name
                // });
                var r = new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = permission.Name,
                    ClaimValue = permission.Name
                };
                
                await db.StoreAsync(r);
            }
        }

        await SaveChangesAsync();
    }

    public IAsyncDocumentSession session => db;
    public IRavenQueryable<User> Users => db.Query<User>();
    public IRavenQueryable<Role> Roles => db.Query<Role>();
    public IRavenQueryable<IdentityUserClaim<string>> UserClaims => db.Query<IdentityUserClaim<string>>();
    public IRavenQueryable<IdentityUserRole<string>> UserRoles => db.Query<IdentityUserRole<string>>();
    public IRavenQueryable<IdentityUserLogin<string>> UserLogins => db.Query<IdentityUserLogin<string>>();
    public IRavenQueryable<IdentityRoleClaim<string>> RoleClaims => db.Query<IdentityRoleClaim<string>>();
    public IRavenQueryable<IdentityUserToken<string>> UserTokens => db.Query<IdentityUserToken<string>>();

    public int SaveChanges()
    {
        return SaveChangesAsync()
            .GetAwaiter()
            .GetResult();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        // todo - use raven advanced session features to track changes and only save the ones that have been changed
        
        await db.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(1);
    }
}
