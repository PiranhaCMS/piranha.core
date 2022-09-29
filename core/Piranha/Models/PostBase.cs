/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.ComponentModel.DataAnnotations;

namespace Piranha.Models;

/// <summary>
/// Base class for post models.
/// </summary>
[Serializable]
public abstract class PostBase : RoutedContentBase, ICategorizedContent, ITaggedContent
{
    /// <summary>
    /// Gets/sets the blog page id.
    /// </summary>
    [Required]
    public Guid BlogId { get; set; }

    /// <summary>
    /// Gets/sets the category.
    /// </summary>
    [Required]
    public Taxonomy Category { get; set; }

    /// <summary>
    /// Gets/sets the available tags.
    /// </summary>
    public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();
}
