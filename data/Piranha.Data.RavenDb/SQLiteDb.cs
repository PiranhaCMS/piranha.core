/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Raven.Client.Documents.Session;

namespace Piranha.Data.RavenDb;

/// <summary>
/// The SQLite db context.
/// </summary>
[Obsolete("SQLite is no longer supported. Please use the RavenDB provider instead.", true)]
public class SQLiteDb : DbRavenBase
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current raven db session</param>
    public SQLiteDb(IAsyncDocumentSession db) : base(db, null) { }
}
