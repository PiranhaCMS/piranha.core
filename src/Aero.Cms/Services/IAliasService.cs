

using Aero.Cms.Models;

namespace Aero.Cms.Services;

public interface IAliasService
{
    /// <summary>
    /// Gets all available models for the specified site.
    /// </summary>
    /// <param name="siteId">The optional site id</param>
    /// <returns>The available models</returns>
    Task<IEnumerable<Alias>> GetAllAsync(string siteId = null);

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or null if it doesn't exist</returns>
    Task<Alias> GetByIdAsync(string id);

    /// <summary>
    /// Gets the model with the given alias url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The optional site id</param>
    /// <returns>The model</returns>
    Task<Alias> GetByAliasUrlAsync(string url, string siteId = null);

    /// <summary>
    /// Gets the model with the given redirect url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The optional site id</param>
    /// <returns>The model</returns>
    Task<IEnumerable<Alias>> GetByRedirectUrlAsync(string url, string siteId = null);

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    Task SaveAsync(Alias model);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Deletes the given model.
    /// </summary>
    /// <param name="model">The model</param>
    Task DeleteAsync(Alias model);
}
