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
using Microsoft.EntityFrameworkCore;

using Piranha.Models;

namespace Piranha.Repositories;

internal class SiteTypeRepository : ISiteTypeRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db connection</param>
    public SiteTypeRepository(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<SiteType>> GetAll()
    {
        var models = new List<SiteType>();
        var types = await _db.SiteTypes
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var type in types)
        {
            models.Add(JsonSerializer.Deserialize<SiteType>(type.Body));
        }
        return models;
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique i</param>
    /// <returns></returns>
    public async Task<SiteType> GetById(string id)
    {
        var type = await _db.SiteTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id)
            .ConfigureAwait(false);

        if (type != null)
        {
            return JsonSerializer.Deserialize<SiteType>(type.Body);
        }
        return null;
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(SiteType model)
    {
        var type = await _db.SiteTypes
            .FirstOrDefaultAsync(t => t.Id == model.Id)
            .ConfigureAwait(false);

        if (type == null) {
            type = new Data.SiteType
            {
                Id = model.Id,
                Created = DateTime.Now
            };
            //await _db.SiteTypes.AddAsync(type).ConfigureAwait(false);
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
        var type = await _db.SiteTypes
            .FirstOrDefaultAsync(t => t.Id == id).ConfigureAwait(false);

        if (type != null)
        {
            //_db.SiteTypes.Remove(type);
            _db.session.Delete(type);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
