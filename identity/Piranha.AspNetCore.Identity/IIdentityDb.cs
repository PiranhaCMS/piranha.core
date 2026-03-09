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
using Piranha.AspNetCore.Identity.Data;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Piranha.AspNetCore.Identity;

public interface IIdentityDb
{
    IAsyncDocumentSession session { get; }
    /// <summary>
    /// Gets/sets the users set.
    /// </summary>
    IRavenQueryable<User> Users { get => session.Query<User>(); }

    /// <summary>
    /// Gets/sets the roles set.
    /// </summary>
    IRavenQueryable<Role> Roles { get => session.Query<Role>(); }

    /// <summary>
    /// Gets/sets the user claims set.
    /// </summary>
    IRavenQueryable<IdentityUserClaim<string>> UserClaims { get => session.Query<IdentityUserClaim<string>>(); }

    /// <summary>
    /// Gets/sets the user roles set.
    /// </summary>
    IRavenQueryable<IdentityUserRole<string>> UserRoles { get => session.Query<IdentityUserRole<string>>(); }

    /// <summary>
    /// Gets/sets the user roles set.
    /// </summary>
    IRavenQueryable<IdentityUserLogin<string>> UserLogins { get => session.Query<IdentityUserLogin<string>>(); }

    /// <summary>
    /// Gets/sets the roles claims set.
    /// </summary>
    IRavenQueryable<IdentityRoleClaim<string>> RoleClaims { get => session.Query<IdentityRoleClaim<string>>(); }

    /// <summary>
    /// Gets/sets the user tokes set.
    /// </summary>
    IRavenQueryable<IdentityUserToken<string>> UserTokens { get => session.Query<IdentityUserToken<string>>(); }

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
