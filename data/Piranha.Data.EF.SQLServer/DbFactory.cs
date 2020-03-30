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

namespace Piranha.Data.EF.SQLServer
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<SQLServerDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public SQLServerDb CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SQLServerDb>();
            builder.UseSqlServer("data source=.\\sqlexpress;initial catalog=piranha.dev;integrated security=true;multipleactiveresultsets=true;");
            return new SQLServerDb(builder.Options);
        }
    }
}
#endif