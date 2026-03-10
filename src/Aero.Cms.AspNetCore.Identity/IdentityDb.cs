using Microsoft.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Aero.Cms.Manager;
using Aero.Identity.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Aero.Cms.AspNetCore.Identity;

public class RavenIdentityDb : IdentityDb<RavenIdentityDb>
{
    public RavenIdentityDb(IAsyncDocumentSession db) : base(db)
    {
    }
}

public abstract class IdentityDb<T> : IIdentityDb
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
    /// <param name="db">The RavenDB session</param>
    protected IdentityDb(IAsyncDocumentSession db)
    {
        this.db = db;
        if (IsInitialized)
            return;

        lock (Mutex)
        {
            if (IsInitialized)
                return;

            Seed().GetAwaiter().GetResult();

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
        // Denormalized: store claims directly in the role document
        var existingClaims = role.Claims.Select(c => (c.ClaimType, c.ClaimValue)).ToHashSet();
        
        foreach (var permission in App.Permissions.GetPermissions())
        {
            if (!existingClaims.Contains((permission.Name, permission.Name)))
            {
                role.Claims.Add(new RavenRoleClaim
                {
                    ClaimType = permission.Name,
                    ClaimValue = permission.Name
                });
            }
        }

        // Add the Admin claim which is required by all authorization policies
        if (!existingClaims.Contains((Permission.Admin, Permission.Admin)))
        {
            role.Claims.Add(new RavenRoleClaim
            {
                ClaimType = Permission.Admin,
                ClaimValue = Permission.Admin
            });
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
