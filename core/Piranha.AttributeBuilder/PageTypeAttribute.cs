/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.AttributeBuilder;

/// <summary>
/// Attribute for marking a class as a page type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PageTypeAttribute : ContentTypeBaseAttribute
{
    /// <summary>
    /// Gets/sets if this page type should be used as
    /// an archive.
    /// </summary>
    public bool IsArchive { get; set; }

    /// <summary>
    /// Gets/sets if the page type should use the block editor
    /// for its main content. The default value is True.
    /// </summary>
    public bool UseBlocks { get; set; }

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
    /// Default constructor.
    /// </summary>
    public PageTypeAttribute() : base()
    {
        UseBlocks = true;
    }
}
