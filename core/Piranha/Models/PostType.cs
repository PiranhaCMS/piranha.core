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

[Serializable]
public sealed class PostType : ContentTypeBase
{
    /// <summary>
    /// Gets/sets if the post type should use the block editor
    /// for its main content. The default value is true.
    /// </summary>
    public bool UseBlocks { get; set; } = true;

    /// <summary>
    /// Gets/sets if primary image should be used for the
    /// post type. The default value is true.
    /// </summary>
    public bool UsePrimaryImage { get; set; } = true;

    /// <summary>
    /// Gets/sets if excerpt should be used for the
    /// post type. The default value is true.
    /// </summary>
    public bool UseExcerpt { get; set; } = true;

    /// <summary>
    /// Gets/sets the allowed block types. An empty collection means
    /// that all types are allowed.
    /// </summary>
    public IList<string> BlockItemTypes { get; set; } = new List<string>();

    /// <summary>
    /// Validates that the post type is correctly defined.
    /// </summary>
    public void Ensure()
    {
        if (Regions.Select(r => r.Id).Distinct().Count() != Regions.Count)
        {
            throw new InvalidOperationException($"Region Id not unique for post type {Id}");
        }

        foreach (var region in Regions)
        {
            region.Title = region.Title ?? region.Id;

            if (region.Fields.Select(f => f.Id).Distinct().Count() != region.Fields.Count)
            {
                throw new InvalidOperationException($"Field Id not unique for post type {Id}");
            }

            foreach (var field in region.Fields)
            {
                field.Id = field.Id ?? "Default";
                field.Title = field.Title ?? field.Id;
            }
        }
    }
}
