/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */
    
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents.Session;
            
namespace Piranha;

/// <summary>
/// The SQLite db context.
/// </summary>
public class SQLiteDb : Db<SQLiteDb>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current raven db session</param>
    public SQLiteDb(IAsyncDocumentSession db) : base(db) { }
}
