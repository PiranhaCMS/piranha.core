/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Microsoft.AspNetCore.Builder;
using Piranha.AspNetCore;

/// <summary>
/// Extensions methods for hosting Piranha in an ASP.NET
/// application.
/// </summary>
public static class PiranhaHostingExtensions
{
    /// <summary>
    /// Simple startup with integrated middleware that also adds common
    /// dependencies needed for an integrated web application.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <param name="options">Action for configuring the builder</param>
    /// <returns>The updated application builder</returns>
    public static IApplicationBuilder UsePiranha(this IApplicationBuilder builder, Action<PiranhaApplicationBuilder> options)
    {
        var applicationBuilder = new PiranhaApplicationBuilder(builder);

        applicationBuilder.Builder
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization();

        options?.Invoke(applicationBuilder);

        return applicationBuilder.Builder;
    }
}