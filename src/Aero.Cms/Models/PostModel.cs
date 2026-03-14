

namespace Aero.Cms.Models;

/// <summary>
/// Generic post model.
/// </summary>
/// <typeparam name="T">The model type</typeparam>
[Serializable]
public class Post<T> : PostBase where T : Post<T>
{
    /// <summary>
    /// Creates a new post model using the given post type id.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="typeId">The unique post type id</param>
    /// <returns>The new model</returns>
    public static Task<T> CreateAsync(IApi api, string typeId = null)
    {
        return api.Posts.CreateAsync<T>(typeId);
    }
}
