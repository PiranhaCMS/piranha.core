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

namespace Piranha.AspNetCore.Identity.SQLServer
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<IdentitySQLServerDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public IdentitySQLServerDb CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentitySQLServerDb>();
            builder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=piranha.core;Integrated Security=True;MultipleActiveResultSets=True");
            return new IdentitySQLServerDb(builder.Options);
        }
    }
}
#endif