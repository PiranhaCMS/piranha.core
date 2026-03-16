

namespace Aero.Cms.Manager.Extend;

public class ActionList<T> : List<T> where T : IAction
{
    /// <summary>
    /// Removes the item with the given internal id.
    /// </summary>
    /// <param name="internalId">The internal id</param>
    public void Remove(string internalId)
    {
        var item = this.FirstOrDefault(i => i.InternalId == internalId);

        if (item != null)
        {
            this.Remove(item);
        }
    }
}
