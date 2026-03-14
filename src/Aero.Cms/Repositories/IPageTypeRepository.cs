

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface IPageTypeRepository
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    Task<IEnumerable<PageType>> GetAll();

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    Task<PageType> GetById(string id);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task Save(PageType model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task Delete(string id);
}
