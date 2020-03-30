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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.Data;

namespace Piranha.AspNetCore.Identity
{
    public interface IDb
    {
        /// <summary>
        /// Gets/sets the users set.
        /// </summary>
        DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets/sets the roles set.
        /// </summary>
        DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets/sets the user claims set.
        /// </summary>
        DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }

        /// <summary>
        /// Gets/sets the user roles set.
        /// </summary>
        DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }

        /// <summary>
        /// Gets/sets the user roles set.
        /// </summary>
        DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }

        /// <summary>
        /// Gets/sets the roles claims set.
        /// </summary>
        DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }

        /// <summary>
        /// Gets/sets the user tokes set.
        /// </summary>
        DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }

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
}