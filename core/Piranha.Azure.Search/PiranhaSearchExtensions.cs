/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using System;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Azure.Search;

public static class PiranhaSearchExtensions
{
    /// <summary>
    /// Adds the Piranha identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaAzureSearch(this IServiceCollection services)
    {
        // Add the identity module
        App.Modules.Register<Module>();

        return services;
    }
}