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

namespace Piranha.AspNetCore.Identity;

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
        // Create SysAdmin user if no users exist
        if (!_db.Users.Any())
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = "admin@piranhacms.org",
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var createResult = await _userManager.CreateAsync(adminUser, "password");
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "SysAdmin");
            }

            // Create Author user (Joana)
            var authorUser = new User
            {
                UserName = "joana",
                Email = "joana@centro-investigacao.org",
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var authorResult = await _userManager.CreateAsync(authorUser, "password");
            if (authorResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(authorUser, "Author");
            }

            // Create Reviewer user (Professora Ângela)
            var reviewerUser = new User
            {
                UserName = "angela",
                Email = "angela@centro-investigacao.org",
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var reviewerResult = await _userManager.CreateAsync(reviewerUser, "password");
            if (reviewerResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(reviewerUser, "Reviewer");
            }

            // Create LegalTeam user (Lara)
            var legalUser = new User
            {
                UserName = "lara",
                Email = "lara@centro-investigacao.org",
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var legalResult = await _userManager.CreateAsync(legalUser, "password");
            if (legalResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(legalUser, "LegalTeam");
            }
        }
    }
}
