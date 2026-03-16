

namespace Aero.Cms.Extend;

/// <summary>
/// Attribute for specifying an allowed block item type for a block group.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BlockItemTypeAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the type of the accepted child item.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BlockItemTypeAttribute() { }

    /// <summary>
    /// Creates and initializes the item type.
    /// </summary>
    /// <param name="type">The specified item type</param>
    public BlockItemTypeAttribute(Type type)
    {
        Type = type;
    }
}
