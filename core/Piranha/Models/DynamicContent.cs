/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Dynamic;

namespace Piranha.Models;

/// <summary>
/// Dynamic content model.
/// </summary>
[Serializable]
public class DynamicContent : Content<DynamicContent>, IDynamicContent, ICategorizedContent, ITaggedContent, IBlockContent
{
    /// <summary>
    /// Gets/sets the regions.
    /// </summary>
    public dynamic Regions { get; set; } = new ExpandoObject();

    /// <summary>
    /// Gets/sets the optional category.
    /// </summary>
    public Taxonomy Category { get; set; }

    /// <summary>
    /// Gets/sets the available tags.
    /// </summary>
    public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

    /// <summary>
    /// Gets/sets the blocks.
    /// </summary>
    public IList<Extend.Block> Blocks { get; set; } = new List<Extend.Block>();
}
