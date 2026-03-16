

namespace Aero.Cms.Models;

/// <summary>
/// Abstract class for an hierarchical item in a structure.
/// </summary>
[Serializable]
public abstract class StructureItem<TStructure, T> : Entity
    where T : StructureItem<TStructure, T>
    where TStructure : Structure<TStructure, T>
{
    /// <summary>
    /// Gets/sets the level in the hierarchy.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets/sets the child items.
    /// </summary>
    public TStructure Items { get; set; }
}
