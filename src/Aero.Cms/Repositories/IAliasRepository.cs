

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface IAliasRepository
{
    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <param name="siteId">The optional site id</param>
    /// <returns>The available models</returns>
    Task<IEnumerable<Alias>> GetAll(string siteId);

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or NULL if it doesn't exist</returns>
    Task<Alias> GetById(string id);

    /// <summary>
    /// Gets the model with the given alias url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The site id</param>
    /// <returns>The model</returns>
    Task<Alias> GetByAliasUrl(string url, string siteId);

    /// <summary>
    /// Gets the models with the given redirect url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The site id</param>
    /// <returns>The models</returns>
    Task<IEnumerable<Alias>> GetByRedirectUrl(string url, string siteId);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task Save(Alias model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task Delete(string id);
}
