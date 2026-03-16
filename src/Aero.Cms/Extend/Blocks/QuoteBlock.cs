

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Single column quote block.
/// </summary>
[BlockType(Name = "Quote", Category = "Content", Icon = "fas fa-quote-right", Component = "quote-block")]
public class QuoteBlock : Block, ISearchable, ITranslatable
{
    /// <summary>
    /// Gets/sets the author
    /// </summary>
    public StringField Author { get; set; }

    /// <summary>
    /// Gets/sets the text body.
    /// </summary>
    public TextField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body?.Value != null)
        {
            return Body.Value;
        }
        return "Empty";
    }

    /// <inheritdoc />
    public string GetIndexedContent()
    {
        return !string.IsNullOrEmpty(Body.Value) ? Body.Value : "";
    }
}
