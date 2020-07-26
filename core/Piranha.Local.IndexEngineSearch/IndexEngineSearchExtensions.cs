/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Local;
using Piranha.Local.Services;

public static class IndexEngineSearchExtensions
{
    /// <summary>
    /// Adds the IndexEngine Search module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="filename">The optional filename for the search db</param>
    /// <returns>The services</returns>
    public static PiranhaServiceBuilder UseIndexEngineSearch(this PiranhaServiceBuilder serviceBuilder,
        string filename = "idx.db")
    {
        serviceBuilder.Services.AddPiranhaIndexEngineSearch(filename);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the Azure Search module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="filename">The optional filename for the search db</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaIndexEngineSearch(this IServiceCollection services,
        string filename = "idx.db")
    {
        // Add the identity module
        App.Modules.Register<IndexEngineSearchModule>();

        // Register the search service
        services.AddSingleton<ISearch>(new IndexEngineSearchService(filename));

        return services;
    }
}