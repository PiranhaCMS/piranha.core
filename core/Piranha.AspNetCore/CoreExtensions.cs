﻿/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Builder;

public static class CoreExtensions
{
    /// <summary>
    /// Uses the piranha middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranha(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.SiteMiddleware>()
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
    public static IApplicationBuilder UsePiranhaAliases(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.AliasMiddleware>();
    }

    /// <summary>
    /// Uses the piranha archive middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaArchives(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.ArchiveMiddleware>();
    }

    /// <summary>
    /// Uses the piranha page middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaPages(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.PageMiddleware>();
    }

    /// <summary>
    /// Uses the piranha post middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaPosts(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.PostMiddleware>();
    }

    /// <summary>
    /// Uses the piranha startpage middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaStartPage(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.StartPageMiddleware>();
    }

    /// <summary>
    /// Uses the piranha site routing middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaSites(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.SiteMiddleware>();        
    }

    /// <summary>
    /// Uses the piranha sitemap generation middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaSitemap(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.SitemapMiddleware>();        
    }
}
