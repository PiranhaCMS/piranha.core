/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */
using System.Text.Json.Serialization;

namespace Piranha.Extend;

/// <summary>
/// Base class for blocks.
/// </summary>
[JsonDerivedType(typeof(Piranha.Extend.Blocks.TextBlock), typeDiscriminator: "TextBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.HtmlBlock), typeDiscriminator: "HtmlBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.ImageBlock), typeDiscriminator: "ImageBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.QuoteBlock), typeDiscriminator: "QuoteBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.MarkdownBlock), typeDiscriminator: "MarkdownBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.SeparatorBlock), typeDiscriminator: "SeparatorBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.AudioBlock), typeDiscriminator: "AudioBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.VideoBlock), typeDiscriminator: "VideoBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.PageBlock), typeDiscriminator: "PageBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.PostBlock), typeDiscriminator: "PostBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.ContentBlock), typeDiscriminator: "ContentBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.ColumnBlock), typeDiscriminator: "ColumnBlock")]
[JsonDerivedType(typeof(Piranha.Extend.Blocks.ImageGalleryBlock), typeDiscriminator: "ImageGalleryBlock")]
public abstract class Block : Entity
{
    /// <summary>
    /// Gets/set the block type id.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets the title of the block when used in a block group.
    /// </summary>
    /// <returns>The title</returns>
    public virtual string GetTitle()
    {
        var blockType = App.Blocks.GetByType(GetType());
        var title = "[Not Implemented]";

        if (!string.IsNullOrEmpty(blockType.ListTitleField))
        {
            var prop = GetType().GetProperty(blockType.ListTitleField, App.PropertyBindings);

            if (prop != null && typeof(IField).IsAssignableFrom(prop.PropertyType))
            {
                var field = (IField)prop.GetValue(this);

                title = field.GetTitle();
            }
        }
        return title;
    }
}
