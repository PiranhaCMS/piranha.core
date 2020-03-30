/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha.Extend;
using Piranha.Security;

public static class AspNetCoreExtensions
{
    /// <summary>
    /// Adds the piranha application service.
    /// </summary>
    /// <param name="services">The service collection</param>
    public static IServiceCollection AddPiranhaApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<Piranha.AspNetCore.Services.IApplicationService, Piranha.AspNetCore.Services.ApplicationService>()
            .AddScoped<Piranha.AspNetCore.Services.IModelLoader, Piranha.AspNetCore.Services.ModelLoader>()
            .AddAuthorization(o =>
            {
                o.AddPolicy(Permission.PagePreview, policy =>
                {
                    policy.RequireClaim(Permission.PagePreview, Permission.PagePreview);
                });
                o.AddPolicy(Permission.PostPreview, policy =>
                {
                    policy.RequireClaim(Permission.PostPreview, Permission.PostPreview);
                });
            });
    }

    /// <summary>
    /// Uses the integrated piranha middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseIntegratedPiranha(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.IntegratedMiddleware>();
    }

    /// <summary>
    /// Uses the piranha middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranha(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.ApplicationMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.AliasMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.PageMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.PostMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.ArchiveMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.StartPageMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.SitemapMiddleware>();
    }

    /// <summary>
    /// Uses the piranha alias middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaAliases(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.AliasMiddleware>();
    }

    /// <summary>
    /// Uses the piranha application middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaApplication(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.ApplicationMiddleware>();
    }

    /// <summary>
    /// Uses the piranha archive middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaArchives(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.ArchiveMiddleware>();
    }

    /// <summary>
    /// Uses the piranha page middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaPages(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.PageMiddleware>();
    }

    /// <summary>
    /// Uses the piranha post middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaPosts(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.PostMiddleware>();
    }

    /// <summary>
    /// Uses the piranha startpage middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaStartPage(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.StartPageMiddleware>();
    }

    /// <summary>
    /// Uses the piranha sitemap generation middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaSitemap(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<Piranha.AspNetCore.SitemapMiddleware>();
    }

    /// <summary>
    /// Converts the type name of the block into a pretty
    /// css class name.
    /// </summary>
    /// <param name="block">The current block</param>
    /// <returns>The css class name</returns>
    public static string CssName(this Block block)
    {
        return ClassNameToWebName(block.GetType().Name);
    }

    /// <summary>
    /// Converts a standard camel case class name to a lowercase
    /// string with each word separated with a dash, suitable
    /// for use in views.
    /// </summary>
    /// <param name="str">The camel case string</param>
    /// <returns>The converted string</returns>
    private static string ClassNameToWebName(string str)
    {
        return Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim().Replace(" ", "-").ToLower();
    }
}
