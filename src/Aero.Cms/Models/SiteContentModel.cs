

namespace Aero.Cms.Models;

/// <summary>
/// Generic post model.
/// </summary>
/// <typeparam name="T">The model type</typeparam>
[Serializable]
public class SiteContent<T> : SiteContentBase where T : SiteContent<T>
{
    /// <summary>
    /// Creates a new site content model using the given site type id.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="typeId">The unique site type id</param>
    /// <returns>The new model</returns>
    public static Task<T> CreateAsync(IApi api, string typeId = null)
    {
        return api.Sites.CreateContentAsync<T>(typeId);
    }
}
