/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.Data;
using System;
using System.Linq;

namespace Piranha.AspNetCore.Identity
{
    public interface IDb
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
        DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }
        DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
        DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }

        int SaveChanges();
    }
}