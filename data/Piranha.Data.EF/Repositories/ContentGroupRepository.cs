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
using Piranha.Data.EF;
using Piranha.Models;

namespace Piranha.Repositories;

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
            .AsNoTracking()
            .OrderBy(g => g.Title)
            .ToListAsync()
            .ConfigureAwait(false);

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
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id)
            .ConfigureAwait(false);

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
            .FirstOrDefaultAsync(g => g.Id == model.Id)
            .ConfigureAwait(false);

        if (group == null) {
            group = new Data.ContentGroup
            {
                Id = model.Id,
                Created = DateTime.Now
            };
            await _db.ContentGroups.AddAsync(group).ConfigureAwait(false);
        }
        Module.Mapper.Map<ContentGroup, Data.ContentGroup>(model, group);
        group.LastModified = DateTime.Now;

        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task DeleteAsync(string id)
    {
        var group = await _db.ContentGroups
            .FirstOrDefaultAsync(g => g.Id == id)
            .ConfigureAwait(false);

        if (group != null)
        {
            _db.ContentGroups.Remove(group);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
