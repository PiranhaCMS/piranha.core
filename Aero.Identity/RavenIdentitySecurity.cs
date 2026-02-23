using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Piranha.Manager.LocalAuth;

namespace Aero.Identity;

/// <summary>
/// RavenDB implementation of the Piranha Manager security bridge.
/// </summary>
public class RavenIdentitySecurity : ISecurity
{
    private readonly SignInManager<RavenUser> _signInManager;
    private readonly IdentityOptions _options;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="signInManager">The sign in manager.</param>
    /// <param name="identityOptions">The identity options.</param>
    public RavenIdentitySecurity(SignInManager<RavenUser> signInManager, IOptions<IdentityOptions> identityOptions)
    {
        _signInManager = signInManager;
        _options = identityOptions.Value;
    }

    /// <inheritdoc />
    public async Task<LoginResult> SignIn(object context, string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, false, _options.Lockout.MaxFailedAccessAttempts > 0);

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

    /// <inheritdoc />
    public Task SignOut(object context)
    {
        return _signInManager.SignOutAsync();
    }
}
