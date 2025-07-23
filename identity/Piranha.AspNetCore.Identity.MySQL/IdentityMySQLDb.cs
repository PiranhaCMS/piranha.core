/*
 * Copyright (c) 2019 aatmmr
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core.mysql
 *
 */

using Microsoft.EntityFrameworkCore;

namespace Piranha.AspNetCore.Identity.MySQL;

public class IdentityMySQLDb : Identity.Db<IdentityMySQLDb>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="options">Configuration options</param>
    public IdentityMySQLDb(DbContextOptions<IdentityMySQLDb> options) : base(options) { }
}
