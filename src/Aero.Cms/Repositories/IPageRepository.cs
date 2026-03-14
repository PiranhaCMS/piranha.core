

using Aero.Cms.Models;

namespace Aero.Cms.Repositories;

public interface IPageRepository
{
    /// <summary>
    /// Gets all of the available pages for the current site.
    /// </summary>
    /// <param name="siteId">The site id</param>
    /// <returns>The pages</returns>
    Task<IEnumerable<string>> GetAll(string siteId);

    /// <summary>
    /// Gets the available blog pages for the current site.
    /// </summary>
    /// <param name="siteId">The site id</param>
    /// <returns>The pages</returns>
    Task<IEnumerable<string>> GetAllBlogs(string siteId);

    /// <summary>
    /// Gets the id of all pages that have a draft for
    /// the specified site.
    /// </summary>
    /// <param name="siteId">The unique site id</param>
    /// <returns>The pages that have a draft</returns>
    Task<IEnumerable<string>> GetAllDrafts(string siteId);

    /// <summary>
    /// Gets the comments available for the page with the specified id. If no page id
    /// is provided all comments are fetched.
    /// </summary>
    /// <param name="pageId">The unique page id</param>
    /// <param name="onlyApproved">If only approved comments should be fetched</param>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The available comments</returns>
    Task<IEnumerable<Comment>> GetAllComments(string pageId, bool onlyApproved,
        int page, int pageSize);

    /// <summary>
    /// Gets the pending comments available for the page with the specified id.
    /// </summary>
    /// <param name="pageId">The unique page id</param>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The available comments</returns>
    Task<IEnumerable<Comment>> GetAllPendingComments(string pageId,
        int page, int pageSize);

    /// <summary>
    /// Gets the site startpage.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="siteId">The site id</param>
    /// <returns>The page model</returns>
    Task<T> GetStartpage<T>(string siteId) where T : PageBase;

    /// <summary>
    /// Gets the page model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <returns>The page model</returns>
    Task<T> GetById<T>(string id) where T : PageBase;

    /// <summary>
    /// Gets the page models with the specified id's.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="ids">The unique id's</param>
    /// <returns>The page models</returns>
    Task<IEnumerable<T>> GetByIds<T>(params string[] ids) where T : PageBase;

    /// <summary>
    /// Gets the page model with the specified slug.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="slug">The unique slug</param>
    /// <param name="siteId">The site id</param>
    /// <returns>The page model</returns>
    Task<T> GetBySlug<T>(string slug, string siteId) where T : PageBase;

    /// <summary>
    /// Gets the draft for the page model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <returns>The draft, or null if no draft exists</returns>
    Task<T> GetDraftById<T>(string id) where T : PageBase;

    /// <summary>
    /// Moves the current page in the structure.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The page to move</param>
    /// <param name="parentId">The new parent id</param>
    /// <param name="sortOrder">The new sort order</param>
    /// <returns>The other pages that were affected by the move</returns>
    Task<IEnumerable<string>> Move<T>(T model, string parentId, int sortOrder) where T : PageBase;

    /// <summary>
    /// Gets the comment with the given id.
    /// </summary>
    /// <param name="id">The comment id</param>
    /// <returns>The model</returns>
    Task<Comment> GetCommentById(string id);

    /// <summary>
    /// Saves the given page model
    /// </summary>
    /// <param name="model">The page model</param>
    /// <returns>The other pages that were affected by the move</returns>
    Task<IEnumerable<string>> Save<T>(T model) where T : PageBase;

    /// <summary>
    /// Saves the given model as a draft revision.
    /// </summary>
    /// <param name="model">The page model</param>
    Task SaveDraft<T>(T model) where T : PageBase;

    /// <summary>
    /// Saves the comment.
    /// </summary>
    /// <param name="pageId">The unique page id</param>
    /// <param name="model">The comment model</param>
    Task SaveComment(string pageId, Comment model);

    /// <summary>
    /// Creates a revision from the current version
    /// of the page with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="revisions">The maximum number of revisions that should be stored</param>
    Task CreateRevision(string id, int revisions);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The other pages that were affected by the move</returns>
    Task<IEnumerable<string>> Delete(string id);

    /// <summary>
    /// Deletes the current draft revision for the page
    /// with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteDraft(string id);

    /// <summary>
    /// Deletes the comment with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteComment(string id);
}
