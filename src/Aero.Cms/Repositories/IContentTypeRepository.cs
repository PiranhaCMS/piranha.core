

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface IContentTypeRepository
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<ContentType>> GetAllAsync();

    /// <summary>
    /// Gets all available models from the specified group.
    /// </summary>
    /// <param name="group">The content group</param>
    /// <returns>The available models</returns>
    Task<IEnumerable<ContentType>> GetByGroupAsync(string group);

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    Task<ContentType> GetByIdAsync(string id);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task SaveAsync(ContentType model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(string id);
}
