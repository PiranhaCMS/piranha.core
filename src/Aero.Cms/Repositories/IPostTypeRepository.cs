

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface IPostTypeRepository
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<PostType>> GetAll();

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    Task<PostType> GetById(string id);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task Save(PostType model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task Delete(string id);
}
