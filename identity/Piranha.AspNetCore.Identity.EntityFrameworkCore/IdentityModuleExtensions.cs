/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Piranha.AspNetCore.Identity.Data;

namespace Piranha.AspNetCore.Identity.EntityFrameworkCore
{
    public static class IdentityModuleExtensions
    {
        /// <summary>
        /// Adds the Piranha identity module.
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="dbOptions">Options for configuring the database</param>
        /// <returns>The services</returns>
        public static IServiceCollection AddPiranhaIdentityEF<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptions) where T : DbContext, IDb
        {
            services.AddDbContext<T>(dbOptions);
            services.AddScoped<IDb, T>();
            services.AddScoped<T, T>();
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<T>();

            return services;
        }
    }
}