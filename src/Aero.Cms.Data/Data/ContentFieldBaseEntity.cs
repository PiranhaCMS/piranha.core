

namespace Aero.Cms.Data.Data;

[Serializable]
public abstract class ContentFieldBase : Entity
{
    /// <summary>
    /// Gets/sets the region id.
    /// </summary>
    public string RegionId { get; set; }

    /// <summary>
    /// Gets/sets the field id.
    /// </summary>
    public string FieldId { get; set; }

    /// <summary>
    /// Gets/sets the sort order.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the sort order of the value.
    /// </summary>
    public string CLRType { get; set; }

    /// <summary>
    /// Gets/sets the JSON serialized value.
    /// </summary>
    public string Value { get; set; }
}
