/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;

namespace Piranha.AspNetCore.Security;

/// <summary>
/// The security builder is used for creating application
/// policies that can be selected from the manager
/// interface.
/// </summary>
public class SecurityBuilder
{
    /// <summary>
    /// The policy builder.
    /// </summary>
    private readonly AuthorizationOptions _authorization;
    private readonly PiranhaServiceBuilder _builder;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="authorization">The authorization options</param>
    /// <param name="builder">The service builder</param>
    public SecurityBuilder(AuthorizationOptions authorization, PiranhaServiceBuilder builder)
    {
        _authorization = authorization;
        _builder = builder;
    }

    /// <summary>
    /// Uses the specified permission in the application.
    /// </summary>
    /// <param name="name">The name</param>
    /// <param name="title">The optional title. If omitted the name will be used as title</param>
    /// <returns>The builder</returns>
    public SecurityBuilder UsePermission(string name, string title = null)
    {
        // Add a policy with the specified name
        _authorization.AddPolicy(name, policy =>
        {
            // Require a claim with the same name as the policy
            policy.RequireClaim(name, name);

            // Add the specified policy to the manager
            App.Permissions["App"].Add(new Piranha.Security.PermissionItem
            {
                Title = title != null ? title : name,
                Name = name
            });
        });

        return this;
    }
}
