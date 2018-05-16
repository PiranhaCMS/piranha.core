/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly UserManager<Data.User> _userManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current DbContext</param>
        /// <param name="userManager">The current UserManager</param>
        public DefaultIdentitySeed(IDb db, UserManager<Data.User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Create the seed data.
        /// </summary>
        public async Task CreateAsync() 
        {
            if (_db.Users.Count() == 0)
            {
                var user = new Data.User 
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