/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

#if corefx
using Microsoft.Extensions.DependencyInjection;
#endif
using System;

namespace Piranha.Data
{
    /// <summary>
    /// Database extensions.
    /// </summary>
    public static class Db
    {
        /// <summary>
        /// The currently available migrations.
        /// </summary>
        public static readonly DbMigration[] Migrations = new DbMigration[] {
            new DbMigration() {
                Name = "InitialCreate",
                Script = "Piranha.Data.Migrations.1.sql"
            }
        };

#if corefx
        /// <summary>
        /// Registers the Piranha db initializer.
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="options">The connection builder</param>
        public static void AddPiranhaDb(this IServiceCollection services, Action<DbBuilder> options) {
            services.AddSingleton<Action<DbBuilder>>(options);
        }
#endif
    }
}
