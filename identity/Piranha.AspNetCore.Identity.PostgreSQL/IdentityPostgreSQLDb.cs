/*
 * Copyright (c) 2018 Jason Underhill
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;

namespace Piranha.AspNetCore.Identity.PostgreSQL;

public class IdentityPostgreSQLDb : Db<IdentityPostgreSQLDb>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="options">Configuration options</param>
    public IdentityPostgreSQLDb(DbContextOptions<IdentityPostgreSQLDb> options) : base(options) { }
}
