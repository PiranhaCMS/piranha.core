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

namespace Piranha.AspNetCore.Identity.SQLServer
{
    public class IdentitySQLServerDb : Db<IdentitySQLServerDb>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public IdentitySQLServerDb(DbContextOptions<IdentitySQLServerDb> options) : base(options) { }
    }
}