

using Aero.Cms.Models;

namespace Aero.Cms.Services;

public interface IContentService
{
    /// <summary>
    /// Creates and initializes a new content model.
    /// </summary>
    /// <param name="typeId">The content type id</param>
    /// <returns>The created page</returns>
    Task<T> CreateAsync<T>(string typeId) where T : GenericContent;

    /// <summary>
    /// Gets all of the available content for the optional
    /// group id.
    /// </summary>
    /// <param name="groupId">The optional group id</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The available content</returns>
    Task<IEnumerable<DynamicContent>> GetAllAsync(string groupId = null, string languageId = null);

    /// <summary>
    /// Gets all of the available content for the optional
    /// group id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="groupId">The optional group id</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The available content</returns>
    Task<IEnumerable<T>> GetAllAsync<T>(string groupId = null, string languageId = null) where T : GenericContent;

    /// <summary>
    /// Gets the content model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The content model</returns>
    Task<DynamicContent> GetByIdAsync(string id, string languageId = null);

    /// <summary>
    /// Gets the content model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The content model</returns>
    Task<T> GetByIdAsync<T>(string id, string languageId = null) where T : GenericContent;

    /// <summary>
    /// Gets all available categories for the specified group.
    /// </summary>
    /// <param name="groupId">The group id</param>
    /// <returns>The available categories</returns>
    Task<IEnumerable<Taxonomy>> GetAllCategoriesAsync(string groupId);

    /// <summary>
    /// Gets all available tags for the specified groupd.
    /// </summary>
    /// <param name="groupId">The group id</param>
    /// <returns>The available tags</returns>
    Task<IEnumerable<Taxonomy>> GetAllTagsAsync(string groupId);

    /// <summary>
    /// Gets the current translation status for the content model
    /// with the given id.
    /// </summary>
    /// <param name="contentId">The unique content id</param>
    /// <returns>The translation status</returns>
    Task<TranslationStatus> GetTranslationStatusByIdAsync(string contentId);

    /// <summary>
    /// Gets the translation summary for the content group with
    /// the given id.
    /// </summary>
    /// <param name="groupId">The group id</param>
    /// <returns>The translation summary</returns>
    Task<TranslationSummary> GetTranslationStatusByGroupAsync(string groupId);

    /// <summary>
    /// Saves the given content model
    /// </summary>
    /// <param name="model">The content model</param>
    /// <param name="languageId">The optional language id</param>
    Task SaveAsync<T>(T model, string languageId = null) where T : GenericContent;

    /// <summary>
    /// Deletes the content model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Deletes the given content model.
    /// </summary>
    /// <param name="model">The content model</param>
    Task DeleteAsync<T>(T model) where T : GenericContent;
}
