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
using Microsoft.Extensions.Options;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Manager.LocalAuth;

namespace Piranha.AspNetCore.Identity;

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
    /// The identity options.
    /// </summary>
    private readonly IdentityOptions _options;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public IdentitySecurity(SignInManager<User> signInManager, IOptions<IdentityOptions> identityOptions, IIdentitySeed seed = null)
    {
        _signInManager = signInManager;
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
}
