

using System.Text.Json;
using Aero.Cms.Models;
using Aero.Cms.Repositories;
using Raven.Client.Documents;

namespace Aero.Cms.Data.Repositories;

internal class PostTypeRepository : IPostTypeRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db connection</param>
    public PostTypeRepository(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<PostType>> GetAll()
    {
        var models = new List<PostType>();
        var types = await _db.PostTypes
            
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var type in types)
        {
            models.Add(JsonSerializer.Deserialize<PostType>(type.Body));
        }
        return models;
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    public async Task<PostType> GetById(string id)
    {
        var type = await _db.PostTypes
            
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            return JsonSerializer.Deserialize<PostType>(type.Body);
        }
        return null;
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(PostType model)
    {
        var type = await _db.PostTypes
            .FirstOrDefaultAsync(t => t.Id == model.Id)
            .ConfigureAwait(false);

        if (type == null) {
            type = new Data.PostType
            {
                Id = model.Id,
                Created = DateTime.Now
            };
            //await _db.PostTypes.AddAsync(type).ConfigureAwait(false);
            await _db.session.StoreAsync(type).ConfigureAwait(false);
        }
        type.CLRType = model.CLRType;
        type.Body = JsonSerializer.Serialize(model);
        type.LastModified = DateTime.Now;

        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var type = await _db.PostTypes
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            //_db.PostTypes.Remove(type);
            _db.session.Delete(type);

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

