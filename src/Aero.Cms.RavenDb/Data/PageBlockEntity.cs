

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

/// <summary>
/// Connection between a page and a block.
/// </summary>
[Serializable]
public sealed class PageBlock : Entity, IContentBlock
{
    /// <summary>
    /// This property is not used any more, but is kept for atm
    /// backwards compatible SQLite migrations.
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// Gets/sets the page id.
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// Gets/sets the block id.
    /// </summary>
    public string BlockId { get; set; }

    /// <summary>
    /// Gets/sets the zero based sort index.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the page containing the block.
    /// </summary>
    [JsonIgnore]
    public Page Page { get; set; }

    /// <summary>
    /// Gets/sets the block data.
    /// </summary>
    public Block Block { get; set; }
}
