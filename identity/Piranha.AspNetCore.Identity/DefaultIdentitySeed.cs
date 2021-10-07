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
        /// The private user manager.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userManager">The current UserManager</param>
        public DefaultIdentitySeed(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Create the seed data.
        /// </summary>
        public async Task CreateAsync()
        {
            if (!_userManager.Users.Any())
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@piranhacms.org"
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