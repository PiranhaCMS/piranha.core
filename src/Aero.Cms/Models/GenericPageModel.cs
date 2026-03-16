

namespace Aero.Cms.Models;

/// <summary>
/// Generic page model.
/// </summary>
/// <typeparam name="T">The model type</typeparam>
[Serializable]
public class GenericPage<T> : PageBase where T : GenericPage<T>
{
    /// <summary>
    /// Creates a new page model using the given page type id.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="typeId">The unique page type id</param>
    /// <returns>The new model</returns>
    public static Task<T> CreateAsync(IApi api, string typeId = null)
    {
        return api.Pages.CreateAsync<T>(typeId);
    }
}
