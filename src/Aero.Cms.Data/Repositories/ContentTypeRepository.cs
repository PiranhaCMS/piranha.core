

using Aero.Cms.Models;
using Aero.Cms.Repositories;
using Marten;


namespace Aero.Cms.Data.Repositories;

internal class ContentTypeRepository : IContentTypeRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db connection</param>
    public ContentTypeRepository(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<ContentType>> GetAllAsync()
    {
        return await _db.ContentTypes
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all available models from the specified group.
    /// </summary>
    /// <param name="group">The content group</param>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<ContentType>> GetByGroupAsync(string group)
    {
        return await _db.ContentTypes
            .Where(t => t.Group == group)
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns></returns>
    public async Task<ContentType> GetByIdAsync(string id)
    {
        return await _db.ContentTypes
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task SaveAsync(ContentType model)
    {
        _db.session.Store(model);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task DeleteAsync(string id)
    {
        var type = await GetByIdAsync(id).ConfigureAwait(false);

        if (type != null)
        {
            _db.session.Delete(type);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

