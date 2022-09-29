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
/// Interface for content that should be categorized.
/// </summary>
public interface ICategorizedContent
{
    /// <summary>
    /// Gets/sets the optional category.
    /// </summary>
    Taxonomy Category { get; set; }
}
