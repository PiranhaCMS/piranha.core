

using Aero.Cms.Models;
using Aero.Cms.Runtime;

public static class AeroExtensions
{
    /// <summary>
    /// Adds the given taxonomies to the list.
    /// </summary>
    /// <param name="list">The list</param>
    /// <param name="titles">The taxonomies</param>
    public static void Add(this List<Taxonomy> list, params string[] titles)
    {
        foreach (var title in titles)
        {
            list.Add(new Taxonomy
            {
                Title = title
            });
        }
    }

    /// <summary>
    /// Get a list content types by content group id
    /// </summary>
    /// <param name="list">The list</param>
    /// <param name="contentGroupId">Content group type id</param>
    public static List<ContentType> GetByGroupId(this CachedList<ContentType> list, string contentGroupId)
    {
        return Aero.Cms.App.ContentTypes.Where(ct => string.Equals(ct.Group, contentGroupId, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
