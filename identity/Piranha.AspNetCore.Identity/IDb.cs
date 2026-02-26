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

namespace Piranha.AspNetCore.Identity;

public interface IDb
{
    /// <summary>
    /// Gets/sets the users set.
    /// </summary>
    IRavenQueryable<User> Users { get; set; }

    /// <summary>
    /// Gets/sets the roles set.
    /// </summary>
    IRavenQueryable<Role> Roles { get; set; }

    /// <summary>
    /// Gets/sets the user claims set.
    /// </summary>
    IRavenQueryable<IdentityUserClaim<Guid>> UserClaims { get; set; }

    /// <summary>
    /// Gets/sets the user roles set.
    /// </summary>
    IRavenQueryable<IdentityUserRole<Guid>> UserRoles { get; set; }

    /// <summary>
    /// Gets/sets the user roles set.
    /// </summary>
    IRavenQueryable<IdentityUserLogin<Guid>> UserLogins { get; set; }

    /// <summary>
    /// Gets/sets the roles claims set.
    /// </summary>
    IRavenQueryable<IdentityRoleClaim<Guid>> RoleClaims { get; set; }

    /// <summary>
    /// Gets/sets the user tokes set.
    /// </summary>
    IRavenQueryable<IdentityUserToken<Guid>> UserTokens { get; set; }

    /// <summary>
    /// Saves the changes made to the context.
    /// </summary>
    /// <returns></returns>
    int SaveChanges();

    /// <summary>
    /// Saves the changes made to the context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
