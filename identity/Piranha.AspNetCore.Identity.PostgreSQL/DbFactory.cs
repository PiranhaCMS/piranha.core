﻿#if DEBUG
/*
 * Copyright (c) 2019 Jason Underhill
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Piranha.AspNetCore.Identity.PostgreSQL
{
    /// <summary>
    /// Factory for creating a db context. Only used in dev mode
    /// when creating migrations.
    /// </summary>
    [NoCoverage]
    public class DbFactory : IDesignTimeDbContextFactory<IdentityPostgreSQLDb>
    {
        /// <summary>
        /// Creates a new db context.
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The db context</returns>
        public IdentityPostgreSQLDb CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentityPostgreSQLDb>();
            builder.UseNpgsql("server=localhost;port=5432;database=piranha;uid=root;password=password");
            return new IdentityPostgreSQLDb(builder.Options);
        }
    }
}
#endif
