

namespace Aero.Cms.RavenDb.Data;

/// <summary>
/// Connection between a page and a content.
/// </summary>
public interface IContentBlock
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets/sets the block id.
    /// </summary>
    string BlockId { get; set; }

    /// <summary>
    /// Gets/sets the zero based sort index.
    /// </summary>
    int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the block data.
    /// </summary>
    Block Block { get; set; }
}
