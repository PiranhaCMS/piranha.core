/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore.Http;
using Piranha.AspNetCore.Security;
using Piranha.Models;

/// <summary>
/// Security extensions for simplifying authorization in
/// the client application.
/// </summary>
public static class PiranhaSecurityExtensions
{
    /// <summary>
    /// Adds authorization with the given application policies to the aplication.
    /// </summary>
    /// <param name="builder">The service builder</param>
    /// <param name="builderOptions">The security builder options</param>
    /// <param name="securityOptions">The security options</param>
    /// <returns>The service builder</returns>
    public static PiranhaServiceBuilder UseSecurity(this PiranhaServiceBuilder builder,
        Action<SecurityBuilder> builderOptions, Action<SecurityOptions> securityOptions = null)
    {
        // Configure
        builder.Services.Configure<SecurityOptions>(o => securityOptions?.Invoke(o));

        // Add authentication
        builder.Services.AddAuthorization(o =>
        {
            // Invoke the builder options
            var securityBuilder = new SecurityBuilder(o, builder);
            builderOptions?.Invoke(securityBuilder);
        });
        return builder;
    }

    /// <summary>
    /// Adds the security middleware to the pipeline.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The update builder</returns>
    public static IApplicationBuilder UseSecurityMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityMiddleware>();
    }

    /// <summary>
    /// Filters the current sitemap collection to only include the items the
    /// current user has access to. Please note that this only filters the
    /// current collection, it doesn't filter the entire strucure.
    /// </summary>
    /// <param name="sitemap">The sitemap items</param>
    /// <param name="user">The current user</param>
    /// <param name="auth">The authorization service</param>
    /// <returns>The filtered collection</returns>
    public static async Task<IEnumerable<SitemapItem>> ForUserAsync(this IEnumerable<SitemapItem> sitemap, ClaimsPrincipal user, IAuthorizationService auth)
    {
        var result = new Sitemap();

        foreach (var item in sitemap)
        {
            if (item.Permissions.Count == 0)
            {
                result.Add(item);
            }
            else
            {
                var success = true;

                foreach (var permission in item.Permissions)
                {
                    if (!(await auth.AuthorizeAsync(user, permission)).Succeeded)
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    result.Add(item);
                }
            }
        }
        return result;
    }
}