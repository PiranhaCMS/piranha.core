

using System.Text.RegularExpressions;
using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Single column HTML block.
/// </summary>
[BlockType(Name = "Markdown", Category = "Content", Icon = "fab fa-markdown", Component = "markdown-block")]
public class MarkdownBlock : Block, ISearchable, ITranslatable
{
    /// <summary>
    /// Gets/sets the Markdown body.
    /// </summary>
    public MarkdownField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body?.Value != null)
        {
            var html = App.Markdown.Transform(Body.Value);

            var title = Regex.Replace(html, @"<[^>]*>", "");

            if (title.Length > 40)
            {
                title = title.Substring(0, 40) + "...";
            }
            return title;
        }
        return "Empty";
    }

    /// <inheritdoc />
    public string GetIndexedContent()
    {
        return !string.IsNullOrEmpty(Body.Value) ? Body.Value : "";
    }

    /// <summary>
    /// Implicitly converts the markdown block to a HTML string.
    /// </summary>
    /// <param name="block">The block</param>
    public static implicit operator string(MarkdownBlock block)
    {
        return App.Markdown.Transform(block.Body?.Value);
    }
}
