
using System.Text.Json.Serialization;
using Aero.Cms.Extend.Blocks;

namespace Aero.Cms.Extend;

/// <summary>
/// Base class for blocks.
/// </summary>
[JsonDerivedType(typeof(TextBlock), typeDiscriminator: "TextBlock")]
[JsonDerivedType(typeof(HtmlBlock), typeDiscriminator: "HtmlBlock")]
[JsonDerivedType(typeof(ImageBlock), typeDiscriminator: "ImageBlock")]
[JsonDerivedType(typeof(QuoteBlock), typeDiscriminator: "QuoteBlock")]
[JsonDerivedType(typeof(MarkdownBlock), typeDiscriminator: "MarkdownBlock")]
[JsonDerivedType(typeof(SeparatorBlock), typeDiscriminator: "SeparatorBlock")]
[JsonDerivedType(typeof(AudioBlock), typeDiscriminator: "AudioBlock")]
[JsonDerivedType(typeof(VideoBlock), typeDiscriminator: "VideoBlock")]
[JsonDerivedType(typeof(PageBlock), typeDiscriminator: "PageBlock")]
[JsonDerivedType(typeof(PostBlock), typeDiscriminator: "PostBlock")]
[JsonDerivedType(typeof(ContentBlock), typeDiscriminator: "ContentBlock")]
[JsonDerivedType(typeof(ColumnBlock), typeDiscriminator: "ColumnBlock")]
[JsonDerivedType(typeof(ImageGalleryBlock), typeDiscriminator: "ImageGalleryBlock")]
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
