/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

public static class EFModuleExtensions
{
    /// <summary>
    /// Adds the Piranha EF module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaEF(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ServiceLifetime lifetime = ServiceLifetime.Scoped) {
        // Store the configuration
        Piranha.EF.Module.DbConfig = options;

        // Add the db options
        services.AddDbContext<Piranha.EF.Db>(options, lifetime);
        if (lifetime == ServiceLifetime.Scoped)
            services.AddScoped<Piranha.IDataService, Piranha.EF.DataService>();
        else if (lifetime == ServiceLifetime.Singleton)
            services.AddSingleton<Piranha.IDataService, Piranha.EF.DataService>();
        else if (lifetime == ServiceLifetime.Transient)
            services.AddTransient<Piranha.IDataService, Piranha.EF.DataService>();

        // Add the EF module
        Piranha.App.Modules.Add(new Piranha.EF.Module());

        return services;
    }
}
