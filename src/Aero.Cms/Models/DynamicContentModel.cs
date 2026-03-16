

using System.Dynamic;

namespace Aero.Cms.Models;

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
    public List<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

    /// <summary>
    /// Gets/sets the blocks.
    /// </summary>
    public List<Extend.Block> Blocks { get; set; } = new List<Extend.Block>();
}
