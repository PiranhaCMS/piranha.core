

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface ILanguageRepository
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<Language>> GetAll();

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or null if it doesn't exist</returns>
    Task<Language> GetById(string id);

    /// <summary>
    /// Gets the default model.
    /// </summary>
    /// <returns>The model</returns>
    Task<Language> GetDefault();

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task Save(Language model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task Delete(string id);
}
