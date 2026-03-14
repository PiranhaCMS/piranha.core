

using Microsoft.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;

using Aero.Cms.Manager;
using Marten;

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
        System.Console.WriteLine("[DEBUG] DefaultIdentitySeed.CreateAsync started");
        var user = await _userManager.FindByNameAsync("admin");
        if (user == null)
        {
            System.Console.WriteLine("[DEBUG] Admin user not found, creating");
            user = new User
            {
                Id = Snowflake.NewId(),
                UserName = "admin",
                Email = "admin@aero.io",
                SecurityStamp = Snowflake.NewId()
            };
            var createResult = await _userManager.CreateAsync(user, "password");

            if (createResult.Succeeded)
            {
                System.Console.WriteLine("[DEBUG] Admin user created successfully");
                await _userManager.AddToRoleAsync(user, "SysAdmin");
                await _userManager.AddToRoleAsync(user, Permission.Admin);
                await _db.SaveChangesAsync();
            }
            else
            {
                System.Console.WriteLine($"[DEBUG] Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            System.Console.WriteLine("[DEBUG] Admin user already exists, resetting password");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "password");
            if (!result.Succeeded)
            {
                // If reset fails (e.g. no token provider), try removing and re-adding
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, "password");
            }
            
            if (!await _userManager.IsInRoleAsync(user, "SysAdmin"))
                await _userManager.AddToRoleAsync(user, "SysAdmin");
            if (!await _userManager.IsInRoleAsync(user, Permission.Admin))
                await _userManager.AddToRoleAsync(user, Permission.Admin);
                
            await _db.SaveChangesAsync();
        }
    }
}
