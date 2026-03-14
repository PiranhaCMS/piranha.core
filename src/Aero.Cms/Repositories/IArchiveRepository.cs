

namespace Aero.Cms.Repositories;

public interface IArchiveRepository
{
    /// <summary>
    /// Gets the total post count for the specified archive
    /// and filter.
    /// </summary>
    /// <param name="archiveId">The archive id</param>
    /// <param name="categoryId">The optional category id</param>
    /// <param name="tagId">The optional tag id</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional month</param>
    /// <returns>The total post count</returns>
    Task<int> GetPostCount(string archiveId, string categoryId = null, string tagId = null, int? year = null, int? month = null);

    /// <summary>
    /// Gets the id of the posts in the current archive
    /// matching the specified filter.
    /// </summary>
    /// <param name="archiveId">The archive id</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="currentPage">The current page</param>
    /// <param name="categoryId">The optional category id</param>
    /// <param name="tagId">The optional tag id</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional month</param>
    /// <returns>The matching posts</returns>
    Task<IEnumerable<string>> GetPosts(string archiveId, int pageSize, int currentPage, string categoryId = null, string tagId = null, int? year = null, int? month = null);
}
