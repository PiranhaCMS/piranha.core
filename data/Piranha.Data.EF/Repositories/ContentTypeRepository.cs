/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.Json;
using Raven.Client.Documents;

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
    public async Task<IEnumerable<AeroContentType>> GetAll()
    {
        var models = new List<AeroContentType>();
        var types = await _db.ContentTypes
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var type in types)
        {
            models.Add(JsonSerializer.Deserialize<AeroContentType>(type.Body));
        }
        return models;
    }

    /// <summary>
    /// Gets all available models from the specified group.
    /// </summary>
    /// <param name="group">The content group</param>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<AeroContentType>> GetByGroup(string group)
    {
        var models = new List<AeroContentType>();
        var types = await _db.ContentTypes
            .Where(t => t.Group == group)
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var type in types)
        {
            models.Add(JsonSerializer.Deserialize<AeroContentType>(type.Body));
        }
        return models;
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns></returns>
    public async Task<AeroContentType> GetById(string id)
    {
        var type = await _db.ContentTypes
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            return JsonSerializer.Deserialize<AeroContentType>(type.Body);
        }
        return null;
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(AeroContentType model)
    {
        var type = await _db.ContentTypes
            .FirstOrDefaultAsync(t => t.Id == model.Id)
            .ConfigureAwait(false);

        if (type == null) {
            type = new AeroContentType
            {
                Id = model.Id,
                Group = model.Group,
                Created = DateTime.Now
            };
            //await _db.ContentTypes.AddAsync(type).ConfigureAwait(false);
            await _db.session.StoreAsync(type).ConfigureAwait(false);
        }
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
        var type = await _db.ContentTypes
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            //_db.ContentTypes.Remove(type);
            _db.session.Delete(type);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

