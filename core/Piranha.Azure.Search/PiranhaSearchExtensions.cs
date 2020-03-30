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
using Piranha.Azure.Search;
using Piranha.Azure.Search.Services;

public static class PiranhaSearchExtensions
{
    /// <summary>
    /// Adds the Azure Search module.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <param name="serviceName">The unique name of the azure search service</param>
    /// <param name="apiKey">The admin api key</param>
    /// <returns>The services</returns>
    public static PiranhaServiceBuilder UseAzureSearch(this PiranhaServiceBuilder serviceBuilder,
        string serviceName, string apiKey)
    {
        serviceBuilder.Services.AddPiranhaAzureSearch(serviceName, apiKey);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the Azure Search module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="serviceName">The unique name of the azure search service</param>
    /// <param name="apiKey">The admin api key</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaAzureSearch(this IServiceCollection services,
        string serviceName, string apiKey)
    {
        // Add the identity module
        App.Modules.Register<Module>();

        // Register the search service
        services.AddSingleton<ISearch>(new AzureSearchService(serviceName, apiKey));

        return services;
    }
}