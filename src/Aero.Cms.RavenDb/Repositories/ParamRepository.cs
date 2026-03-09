

using Aero.Cms.Models;
using Aero.Cms.Repositories;
using Raven.Client.Documents;

namespace Aero.Cms.RavenDb.Repositories;

internal class ParamRepository : IParamRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db context</param>
    public ParamRepository(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<Param>> GetAll()
    {
        return await _db.Params
            
            .OrderBy(p => p.Key)
            .Select(p => new Param
            {
                Id = p.Id,
                Key = p.Key,
                Description = p.Description,
                Value = p.Value,
                Created = p.Created,
                LastModified = p.LastModified
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or NULL if it doesn't exist</returns>
    public Task<Param> GetById(string id)
    {
        return _db.Params
            .Customize(x => x.WaitForNonStaleResults())
            .Select(p => new Param
            {
                Id = p.Id,
                Key = p.Key,
                Description = p.Description,
                Value = p.Value,
                Created = p.Created,
                LastModified = p.LastModified
            })
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Gets the model with the given key.
    /// </summary>
    /// <param name="key">The unique key</param>
    /// <returns>The model</returns>
    public Task<Param> GetByKey(string key)
    {
        return _db.Params
            .Customize(x => x.WaitForNonStaleResults())
            .Select(p => new Param
            {
                Id = p.Id,
                Key = p.Key,
                Description = p.Description,
                Value = p.Value,
                Created = p.Created,
                LastModified = p.LastModified
            })
            .FirstOrDefaultAsync(p => p.Key == key);
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(Param model)
    {
        var param = await _db.Params
            .Customize(x => x.WaitForNonStaleResults())
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (param == null)
        {
            param = new Data.Param
            {
                Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId().ToString(),
                Created = DateTime.Now
            };
            //await _db.Params.AddAsync(param).ConfigureAwait(false);
            await _db.session.StoreAsync(param);
        }
        param.Key = model.Key;
        param.Description = model.Description;
        param.Value = model.Value;
        param.LastModified = DateTime.Now;

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var param = await _db.Params
            .Customize(x => x.WaitForNonStaleResults())
            .FirstOrDefaultAsync(p => p.Id == id);

        if (param != null)
        {
            //_db.Params.Remove(param);
            _db.session.Delete(param);

            await _db.SaveChangesAsync();
        }
    }
}

