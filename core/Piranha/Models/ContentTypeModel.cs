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

[Serializable]
public sealed class ContentType : ContentTypeBase
{
    /// <summary>
    /// Gets/sets the group name of the content type.
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Group { get; set; }

    /// <summary>
    /// Gets/sets if the page type should use the block editor
    /// for its main content. The default value is True.
    /// </summary>
    public bool UseBlocks { get; set; }

    /// <summary>
    /// Gets/sets if the content type should be
    /// categorized.
    /// </summary>
    public bool UseCategory { get; set; }

    /// <summary>
    /// Gets/sets if excerpt should be used for the
    /// content type. The default value is true.
    /// </summary>
    public bool UseExcerpt { get; set; } = true;

    /// <summary>
    /// Gets/sets if primary image should be used for the
    /// content type. The default value is true.
    /// </summary>
    public bool UsePrimaryImage { get; set; } = true;

    /// <summary>
    /// Gets/sets if tags should be used for the content type.
    /// </summary>
    public bool UseTags { get; set; }

    /// <summary>
    /// Validates that the content type is correctly defined.
    /// </summary>
    public void Ensure()
    {
        if (string.IsNullOrEmpty(Group))
        {
            throw new InvalidOperationException($"No group specified for content type {Id}");
        }

        if (Regions.Select(r => r.Id).Distinct().Count() != Regions.Count)
        {
            throw new InvalidOperationException($"Region Id not unique for content type {Id}");
        }

        foreach (var region in Regions)
        {
            region.Title = region.Title ?? region.Id;

            if (region.Fields.Select(f => f.Id).Distinct().Count() != region.Fields.Count)
            {
                throw new InvalidOperationException($"Field Id not unique for content type {Id}");
            }

            foreach (var field in region.Fields)
            {
                field.Id = field.Id ?? "Default";
                field.Title = field.Title ?? field.Id;
            }
        }
    }
}
