

using Microsoft.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Raven.Client.Documents;

namespace Aero.Cms.AspNetCore.Identity;

/// <summary>
/// Default identity security seed.
/// </summary>
public class DefaultIdentitySeed : IIdentitySeed
{
    /// <summary>
    /// The private DbContext.
    /// </summary>
    private readonly IIdentityDb _db;

    /// <summary>
    /// The private user manager.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current DbContext</param>
    /// <param name="userManager">The current UserManager</param>
    public DefaultIdentitySeed(IIdentityDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    /// <summary>
    /// Create the seed data.
    /// </summary>
    public async Task CreateAsync()
    {
        if (!await _db.Users.AnyAsync())
        {
            var user = new User
            {
                Id = Snowflake.NewId(),
                UserName = "admin",
                Email = "admin@aero.io",
                SecurityStamp = Snowflake.NewId()
            };
            var createResult = await _userManager.CreateAsync(user, "password");

            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "SysAdmin");
                await _userManager.AddToRoleAsync(user, "Admin");
                await _db.SaveChangesAsync();
            }
        }
    }
}
