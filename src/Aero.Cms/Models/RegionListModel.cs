

namespace Aero.Cms.Models;

/// <summary>
/// Region list for dynamic models.
/// </summary>
/// <typeparam name="T">The item type</typeparam>
[Serializable]
public class RegionList<T> : List<T>, IRegionList
{
    /// <summary>
    /// Gets/sets the page type id.
    /// </summary>
    public string TypeId { get; set; }

    /// <summary>
    /// Gets/sets the region id.
    /// </summary>
    public string RegionId { get; set; }

    /// <summary>
    /// Gets/sets the parent model.
    /// </summary>
    public IDynamicContent Model { get; set; }

    /// <summary>
    /// Adds a new item to the region list
    /// </summary>
    /// <param name="item">The item</param>
    public void Add(object item)
    {
        if (item.GetType() == typeof(T))
        {
            base.Add((T)item);
        }
        else
        {
            throw new ArgumentException("Item type does not match the list");
        }
    }
}
