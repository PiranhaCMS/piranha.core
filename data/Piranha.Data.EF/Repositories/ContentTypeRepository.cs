/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Piranha.Models;

namespace Piranha.Repositories;

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
    public async Task<IEnumerable<ContentType>> GetAll()
    {
        var models = new List<ContentType>();
        var types = await _db.ContentTypes
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var type in types)
        {
            models.Add(JsonConvert.DeserializeObject<ContentType>(type.Body));
        }
        return models;
    }

    /// <summary>
    /// Gets all available models from the specified group.
    /// </summary>
    /// <param name="group">The content group</param>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<ContentType>> GetByGroup(string group)
    {
        var models = new List<ContentType>();
        var types = await _db.ContentTypes
            .AsNoTracking()
            .Where(t => t.Group == group)
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var type in types)
        {
            models.Add(JsonConvert.DeserializeObject<ContentType>(type.Body));
        }
        return models;
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns></returns>
    public async Task<ContentType> GetById(string id)
    {
        var type = await _db.ContentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            return JsonConvert.DeserializeObject<ContentType>(type.Body);
        }
        return null;
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(ContentType model)
    {
        var type = await _db.ContentTypes
            .FirstOrDefaultAsync(t => t.Id == model.Id)
            .ConfigureAwait(false);

        if (type == null) {
            type = new Data.ContentType
            {
                Id = model.Id,
                Group = model.Group,
                Created = DateTime.Now
            };
            await _db.ContentTypes.AddAsync(type).ConfigureAwait(false);
        }
        type.Body = JsonConvert.SerializeObject(model);
        type.LastModified = DateTime.Now;

        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var type = await _db.ContentTypes
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            _db.ContentTypes.Remove(type);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
