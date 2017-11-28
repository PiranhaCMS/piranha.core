/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

public static class CoreExtensions
{
    /// <summary>
    /// Uses the piranha middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranha(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.PageMiddleware>()
            .UseMiddleware<Piranha.AspNetCore.StartPageMiddleware>();
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
    /// Uses the piranha startpage middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaStartPage(this IApplicationBuilder builder) {
        return builder
            .UseMiddleware<Piranha.AspNetCore.StartPageMiddleware>();
    }
}
