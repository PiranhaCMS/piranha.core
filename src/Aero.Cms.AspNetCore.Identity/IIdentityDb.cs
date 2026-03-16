

using Microsoft.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Marten;
using Marten.Linq;



namespace Aero.Cms.AspNetCore.Identity;

public interface IIdentityDb
{
    IDocumentSession session { get; }
    /// <summary>
    /// Gets/sets the users set.
    /// </summary>
    IMartenQueryable<User> Users => session.Query<User>();

    /// <summary>
    /// Gets/sets the roles set.
    /// </summary>
    IMartenQueryable<Role> Roles => session.Query<Role>();

    /// <summary>
    /// Gets/sets the user claims set.
    /// </summary>
    IMartenQueryable<IdentityUserClaim<string>> UserClaims => session.Query<IdentityUserClaim<string>>();

    /// <summary>
    /// Gets/sets the user roles set.
    /// </summary>
    IMartenQueryable<IdentityUserRole<string>> UserRoles => session.Query<IdentityUserRole<string>>();

    /// <summary>
    /// Gets/sets the user roles set.
    /// </summary>
    IMartenQueryable<IdentityUserLogin<string>> UserLogins => session.Query<IdentityUserLogin<string>>();

    /// <summary>
    /// Gets/sets the roles claims set.
    /// </summary>
    IMartenQueryable<IdentityRoleClaim<string>> RoleClaims => session.Query<IdentityRoleClaim<string>>();

    /// <summary>
    /// Gets/sets the user tokes set.
    /// </summary>
    IMartenQueryable<IdentityUserToken<string>> UserTokens => session.Query<IdentityUserToken<string>>();

    /// <summary>
    /// Saves the changes made to the context.
    /// </summary>
    /// <returns></returns>
    int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Saves the changes made to the context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
