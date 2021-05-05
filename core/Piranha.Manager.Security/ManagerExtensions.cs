/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class ManagerModuleExtensions
{
    /// <summary>
    /// Adds the mappings needed for the Piranha Manager Security to
    /// the endpoint routes.
    /// </summary>
    /// <param name="builder">The route builder</param>
    public static void MapPiranhaManagerSecurity(this IEndpointRouteBuilder builder)
    {
        builder.MapRazorPages();
    }

    public static IMvcBuilder AddPiranhaManagerSecurityOptions(this IMvcBuilder builder)
    {
        return builder
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AllowAnonymousToAreaPage("Manager", "/login");
            });
    }
}
