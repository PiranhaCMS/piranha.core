/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Microsoft.EntityFrameworkCore;

namespace Piranha.Data.EF.PostgreSql
{
    [NoCoverage]
    public sealed class PostgreSqlDb : Db<PostgreSqlDb>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public PostgreSqlDb(DbContextOptions<PostgreSqlDb> options) : base(options)
        {
        }
    }
}