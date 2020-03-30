/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha;
using Piranha.AspNetCore;

public static class TinyMCEStartupExtensions
{
    /// <summary>
    /// Adds the Tiny MCE editor module if simple startup is used.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <returns>The updated builder</returns>
    public static PiranhaServiceBuilder UseTinyMCE(this PiranhaServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddPiranhaTinyMCE();

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Tiny MCE editor module if simple startup is used.
    /// </summary>
    /// <param name="applicationBuilder">The application builder</param>
    /// <returns>The updated builder</returns>
    public static PiranhaApplicationBuilder UseTinyMCE(this PiranhaApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UsePiranhaTinyMCE();

        return applicationBuilder;
    }
}
