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
using Npgsql;
using Npgsql.EntityFrameworkCore;


namespace Piranha.Data.EF.PostgreSql
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<PostgreSqlDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public PostgreSqlDb CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PostgreSqlDb>();
            builder.UseNpgsql("Server=localhost;Port=5432;Database=piranha;User ID=piranha;Password=piranha;");
            return new PostgreSqlDb(builder.Options);
        }
    }
}
#endif
