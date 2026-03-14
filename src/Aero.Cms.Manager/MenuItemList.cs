

namespace Aero.Cms.Manager;

public class MenuItemList : List<MenuItem>
{
    /// <summary>
    /// Gets the menu item with the given internal id.
    /// </summary>
    public MenuItem this[string internalId] {
        get
        {
            return this.FirstOrDefault(i => i.InternalId == internalId);
        }
    }
}
