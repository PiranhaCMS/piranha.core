

namespace Aero.Cms.Data.Data;

/// <summary>
/// Reusable content block.
/// </summary>
[Serializable]
public sealed class Block : BlockBase<BlockField>
{
    /// <summary>
    /// Gets/sets the optional title. This property
    /// is only used for reusable blocks within the
    /// block library.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets if this is a reusable block.
    /// </summary>
    public bool IsReusable { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
