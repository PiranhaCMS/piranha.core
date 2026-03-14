

using Markdig;

namespace Aero.Cms.Extend;

/// <inheritdoc />
public class DefaultMarkdown : IMarkdown
{
    /// <summary>
    /// Gets/sets the additional pipeline to use for markdown transformation.
    /// </summary>
    public MarkdownPipeline _pipeline { get; set; }

    /// <inheritdoc />
    public string Transform(string md)
    {
        if (!string.IsNullOrEmpty(md))
        {
            return Markdown.ToHtml(md, _pipeline);
        }
        return md;
    }
}
