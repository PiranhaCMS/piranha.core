/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data.RavenDb.Data;

/// <summary>
/// Interface for content that has an Excerpt property.
/// </summary>
public interface IExcerpt
{
    /// <summary>
    /// Gets/sets the optional excerpt.
    /// </summary>
    string Excerpt { get; set; }
}
