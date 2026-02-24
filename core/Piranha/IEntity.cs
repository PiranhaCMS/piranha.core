/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha;

/// <summary>
/// Interface for database entities.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    string Id { get; set; }
}
