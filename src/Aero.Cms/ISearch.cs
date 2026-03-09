

using Aero.Cms.Models;

namespace Aero.Cms;

/// <summary>
/// Service for indexing content for searching.
/// </summary>
public interface ISearch
{
    /// <summary>
    /// Creates or updates the searchable content for the
    /// given page.
    /// </summary>
    /// <param name="page">The page</param>
    Task SavePageAsync(PageBase page);

    /// <summary>
    /// Deletes the given page from the search index.
    /// </summary>
    /// <param name="page">The page to delete</param>
    Task DeletePageAsync(PageBase page);

    /// <summary>
    /// Creates or updates the searchable content for the
    /// given post.
    /// </summary>
    /// <param name="post">The post</param>
    Task SavePostAsync(PostBase post);

    /// <summary>
    /// Deletes the given post from the search index.
    /// </summary>
    /// <param name="post">The post to delete</param>
    Task DeletePostAsync(PostBase post);
}
