/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;

namespace Piranha.AspNetCore.Identity.SQLite
{
    public class IdentitySQLiteDb : Db<IdentitySQLiteDb>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public IdentitySQLiteDb(DbContextOptions<IdentitySQLiteDb> options) : base(options) { }
    }
}