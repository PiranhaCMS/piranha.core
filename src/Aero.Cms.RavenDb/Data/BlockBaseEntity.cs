

namespace Aero.Cms.RavenDb.Data;

/// <summary>
/// Abstract base class for all content blocks
/// </summary>
[Serializable]
public abstract class BlockBase<T> where T : BlockFieldBase
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// This is not part of the data model. It's only used
    /// for internal mapping.
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets/sets the CLR type of the block.
    /// </summary>
    public string CLRType { get; set; }

    /// <summary>
    /// Gets/sets the available fields.
    /// </summary>
    public List<T> Fields { get; set; } = new List<T>();
}
