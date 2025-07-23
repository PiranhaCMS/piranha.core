/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Models;

/// <summary>
/// Interface for content that can be tagged.
/// </summary>
public interface ITaggedContent
{
    /// <summary>
    /// Gets/sets the available tags.
    /// </summary>
    IList<Taxonomy> Tags { get; set; }
}
