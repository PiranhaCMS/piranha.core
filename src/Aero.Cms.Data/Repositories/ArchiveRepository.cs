using Aero.Cms.Data.Data;
using Aero.Cms.Repositories;
using Raven.Client.Documents;

namespace Aero.Cms.Data.Repositories;

internal class ArchiveRepository : IArchiveRepository
{
    /// <summary>
    /// The current db context.
    /// </summary>
    private readonly IDb _db;

    /// <summary>
    /// Default internal constructor.
    /// </summary>
    /// <param name="db">The current db context</param>
    public ArchiveRepository(IDb db)
    {
        _db = db;
    }

    public Task<int> GetPostCount(string archiveId, string categoryId = null, string tagId = null, int? year = null,
        int? month = null)
    {
        return GetQuery(archiveId, categoryId, tagId, year, month)
            .CountAsync();
    }

    public async Task<IEnumerable<string>> GetPosts(string archiveId, int pageSize, int currentPage,
        string categoryId = null, string tagId = null, int? year = null, int? month = null)
    {
        return await GetQuery(archiveId, categoryId, tagId, year, month)
            .OrderByDescending(p => p.Published)
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.Id)
            .ToListAsync();
    }

    private IQueryable<Post> GetQuery(string archiveId, string categoryId = null, string tagId = null,
        int? year = null, int? month = null)
    {
        // Build the query.
        var now = DateTime.Now;
        var query = _db.Posts
            .Where(p => p.BlogId == archiveId && p.Published <= now);

        if (!string.IsNullOrEmpty(categoryId))
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrEmpty(tagId))
        {
            query = query.Where(p => p.Tags.Any(t => t.TagId == tagId));
        }

        if (year.HasValue)
        {
            DateTime from;
            DateTime to;

            if (month.HasValue)
            {
                from = new DateTime(year.Value, month.Value, 1);
                to = from.AddMonths(1);
            }
            else
            {
                from = new DateTime(year.Value, 1, 1);
                to = from.AddYears(1);
            }

            query = query.Where(p => p.Published >= from && p.Published < to);
        }

        return query;
    }
}

