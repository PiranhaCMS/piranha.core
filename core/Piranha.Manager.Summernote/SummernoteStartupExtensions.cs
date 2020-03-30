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

public static class SummernoteStartupExtensions
{
    /// <summary>
    /// Adds the Summernote editor module if simple startup is used.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <returns>The updated builder</returns>
    public static PiranhaServiceBuilder UseSummernote(this PiranhaServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddPiranhaSummernote();

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Summernote editor module if simple startup is used.
    /// </summary>
    /// <param name="applicationBuilder">The application builder</param>
    /// <returns>The updated builder</returns>
    public static PiranhaApplicationBuilder UseSummernote(this PiranhaApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UsePiranhaSummernote();

        return applicationBuilder;
    }
}
