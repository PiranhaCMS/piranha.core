/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Piranha.AspNetCore.Identity.Data;

namespace Piranha.AspNetCore.Identity
{
    /// <summary>
    /// Default identity security seed.
    /// </summary>
    public class DefaultIdentitySeed : IIdentitySeed
    {
        /// <summary>
        /// The private DbContext.
        /// </summary>
        private readonly IDb _db;

        /// <summary>
        /// The private user manager.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current DbContext</param>
        /// <param name="userManager">The current UserManager</param>
        public DefaultIdentitySeed(IDb db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Create the seed data.
        /// </summary>
        public async Task CreateAsync()
        {
            if (!_db.Users.Any())
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@piranhacms.org",
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var createResult = await _userManager.CreateAsync(user, "password");

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SysAdmin");
                }
            }
        }
    }
}