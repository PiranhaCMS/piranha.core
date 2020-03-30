#if DEBUG
/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Piranha.AspNetCore.Identity.SQLite
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<IdentitySQLiteDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public IdentitySQLiteDb CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentitySQLiteDb>();
            builder.UseSqlite("Filename=./piranha.identity.db");
            return new IdentitySQLiteDb(builder.Options);
        }
    }
}
#endif