

using Aero.Cms.Models;

namespace Aero.Cms.Services;

public interface IArchiveService
{
    /// <summary>
    /// Gets the post archive for the specified archive page
    /// with the given filters applied.
    /// </summary>
    /// <param name="archiveId">The unique archive page id</param>
    /// <param name="currentPage">The optional page number</param>
    /// <param name="categoryId">The optional category id</param>
    /// <param name="tagId">The optional tag id</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional year</param>
    /// <param name="pageSize">The optional page size. If not provided, this will be read from config</param>
    /// <returns>The post archive</returns>
    Task<PostArchive<DynamicPost>> GetByIdAsync(string archiveId, int? currentPage = 1,
        string categoryId = null, string tagId = null, int? year = null, int? month = null, int? pageSize = null);

    /// <summary>
    /// Gets the post archive for the specified archive page
    /// with the given filters applied.
    /// </summary>
    /// <param name="archiveId">The unique archive page id</param>
    /// <param name="currentPage">The optional page number</param>
    /// <param name="categoryId">The optional category id</param>
    /// <param name="tagId">The optional tag id</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional year</param>
    /// <param name="pageSize">The optional page size. If not provided, this will be read from config</param>
    /// <typeparam name="T">The post type</typeparam>
    /// <returns>The post archive</returns>
    Task<PostArchive<T>> GetByIdAsync<T>(string archiveId, int? currentPage = 1,
        string categoryId = null, string tagId = null, int? year = null, int? month = null, int? pageSize = null)
        where T : Models.PostBase;
}
