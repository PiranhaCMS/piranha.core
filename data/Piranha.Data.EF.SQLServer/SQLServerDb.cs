/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Piranha.Data.EF.SQLServer;

[ExcludeFromCodeCoverage]
public sealed class SQLServerDb : Db<SQLServerDb>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="options">Configuration options</param>
    public SQLServerDb(DbContextOptions<SQLServerDb> options) : base(options)
    {
    }
}
