

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface IParamRepository
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<Param>> GetAll();

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or NULL if it doesn't exist</returns>
    Task<Param> GetById(string id);

    /// <summary>
    /// Gets the model with the given internal id.
    /// </summary>
    /// <param name="key">The unique key</param>
    /// <returns>The model</returns>
    Task<Param> GetByKey(string key);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task Save(Param model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task Delete(string id);
}
