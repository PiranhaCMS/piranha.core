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
/// The different types of taxonomies available.
/// </summary>
[Serializable]
public enum TaxonomyType
{
    /// <summary>
    /// The type has not been specified.
    /// </summary>
    NotSet,
    /// <summary>
    /// The taxonomy is a category.
    /// </summary>
    Category,
    /// <summary>
    /// The taxonomy is a tag.
    /// </summary>
    Tag
}
