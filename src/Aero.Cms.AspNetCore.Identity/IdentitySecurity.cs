using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Aero.Cms.AspNetCore.Identity.Data;
using Aero.Cms.Manager.LocalAuth;
using Aero.Identity.Models;

namespace Aero.Cms.AspNetCore.Identity;

public class IdentitySecurity : ISecurity
{
    /// <summary>
    /// The optional identity seed.
    /// </summary>
    private readonly IIdentitySeed _seed;

    /// <summary>
    /// The sign in manager.
    /// </summary>
    private readonly SignInManager<User> _signInManager;

    /// <summary>
    /// The user manager.
    /// </summary>
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// The role manager.
    /// </summary>
    private readonly RoleManager<Role> _roleManager;

    /// <summary>
    /// The identity options.
    /// </summary>
    private readonly IdentityOptions _options;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public IdentitySecurity(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<IdentityOptions> identityOptions, IIdentitySeed seed = null)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _options = identityOptions.Value;
        _seed = seed;
    }

    /// <summary>
    /// Authenticates and signs in the user with the
    /// given credentials.
    /// </summary>
    /// <param name="context">The current application context</param>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <returns>If the user was signed in</returns>
    public async Task<LoginResult> SignIn(object context, string username, string password)
    {
        if (_seed != null)
        {
            await _seed.CreateAsync();
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user != null)
        {
            await SyncRoleClaimsToUserAsync(user);
        }

        var result = await _signInManager.PasswordSignInAsync(username, password, false,
            _options.Lockout.MaxFailedAccessAttempts > 0 ? true : false);

        if (result.Succeeded)
        {
            return LoginResult.Succeeded;
        }
        else if (result.IsLockedOut)
        {
            return LoginResult.Locked;
        }
        return LoginResult.Failed;
    }

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    /// <param name="context">The current application context</param>
    public Task SignOut(object context)
    {
        return _signInManager.SignOutAsync();
    }

    /// <summary>
    /// Syncs role claims to the user claims.
    /// </summary>
    /// <param name="user">The user to sync claims for.</param>
    private async Task SyncRoleClaimsToUserAsync(User user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var existingClaimTypes = user.Claims.Select(c => c.ClaimType).ToHashSet();
            foreach (var roleClaim in role.Claims)
            {
                if (!existingClaimTypes.Contains(roleClaim.ClaimType))
                {
                    user.Claims.Add(new RavenUserClaim
                    {
                        ClaimType = roleClaim.ClaimType,
                        ClaimValue = roleClaim.ClaimValue
                    });
                }   
            }
        }

        if (user.Claims.Any(c => !string.IsNullOrEmpty(c.ClaimType)))
        {
            await _userManager.UpdateAsync(user);
        }
    }
}
