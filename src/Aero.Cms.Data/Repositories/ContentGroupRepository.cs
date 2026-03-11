

using Aero.Cms.Models;
using Aero.Cms.Repositories;
using Raven.Client.Documents;

namespace Aero.Cms.Data.Repositories;

internal class ContentGroupRepository : IContentGroupRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db connection</param>
    public ContentGroupRepository(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<ContentGroup>> GetAllAsync()
    {
        var models = new List<ContentGroup>();
        var groups = await _db.ContentGroups
            .OrderBy(g => g.Title)
            .ToListAsync();

        foreach (var group in groups)
        {
            models.Add(Module.Mapper.Map<Data.ContentGroup, ContentGroup>(group));
        }
        return models;
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns></returns>
    public async Task<ContentGroup> GetByIdAsync(string id)
    {
        var group = await _db.ContentGroups
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group != null)
        {
            return Module.Mapper.Map<Data.ContentGroup, ContentGroup>(group);
        }
        return null;
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task SaveAsync(ContentGroup model)
    {
        var group = await _db.ContentGroups
            .FirstOrDefaultAsync(g => g.Id == model.Id);

        if (group == null) {
            group = new Data.ContentGroup
            {
                Id = model.Id,
                Created = DateTime.Now
            };
            
            await _db.session.StoreAsync(group);
            //await _db.ContentGroups.AddAsync(group).ConfigureAwait(false);
        }
        Module.Mapper.Map<ContentGroup, Data.ContentGroup>(model, group);
        group.LastModified = DateTime.Now;

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task DeleteAsync(string id)
    {
        var group = await _db.ContentGroups
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group != null)
        {
            //_db.ContentGroups.Remove(group);
            _db.session.Delete(group);
            await _db.SaveChangesAsync();
        }
    }
}

