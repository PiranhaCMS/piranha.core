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
    public async Task<IEnumerable<AeroContentType>> GetByGroup(string group)
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
    public async Task<AeroContentType> GetById(string id)
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
    public async Task Save(AeroContentType model)
    {
        await _db.session.StoreAsync(model, model.Id).ConfigureAwait(false);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var type = await GetById(id).ConfigureAwait(false);

        if (type != null)
        {
            _db.session.Delete(type);
            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

