/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;
using Piranha.Repositories;
using Raven.Client.Documents;

namespace Piranha.Data.RavenDb.Repositories;

internal class AliasRepository : IAliasRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db context</param>
    public AliasRepository(IDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all available models for the specified site.
    /// </summary>
    /// <param name="siteId">The site id</param>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<Alias>> GetAll(string siteId)
    {
        return await _db.Aliases
            .Where(a => a.SiteId == siteId)
            .OrderBy(a => a.AliasUrl)
            .ThenBy(a => a.RedirectUrl)
            .Select(a => new Alias
            {
                Id = a.Id,
                SiteId = a.SiteId,
                AliasUrl = a.AliasUrl,
                RedirectUrl = a.RedirectUrl,
                Type = a.Type,
                Created = a.Created,
                LastModified = a.LastModified
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or NULL if it doesn't exist</returns>
    public Task<Alias> GetById(string id)
    {
        return _db.Aliases
            .Where(a => a.Id == id)
            .Select(a => new Alias
            {
                Id = a.Id,
                SiteId = a.SiteId,
                AliasUrl = a.AliasUrl,
                RedirectUrl = a.RedirectUrl,
                Type = a.Type,
                Created = a.Created,
                LastModified = a.LastModified
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets the model with the given alias url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The site id</param>
    /// <returns>The model</returns>
    public async Task<Alias> GetByAliasUrl(string url, string siteId)
    {
        var aliases = await _db.Aliases
            .Where(a => a.SiteId == siteId && a.AliasUrl.Equals(url, StringComparison.OrdinalIgnoreCase))
            .Select(a => new Alias
            {
                Id = a.Id,
                SiteId = a.SiteId,
                AliasUrl = a.AliasUrl,
                RedirectUrl = a.RedirectUrl,
                Type = a.Type,
                Created = a.Created,
                LastModified = a.LastModified
            })
            .FirstOrDefaultAsync();
        
        return aliases;
    }

    /// <summary>
    /// Gets the model with the given redirect url.
    /// </summary>
    /// <param name="url">The unique url</param>
    /// <param name="siteId">The site id</param>
    /// <returns>The model</returns>
    public async Task<IEnumerable<Alias>> GetByRedirectUrl(string url, string siteId)
    {
        return await _db.Aliases
            .Where(a => a.SiteId == siteId && a.RedirectUrl.ToLower() == url.ToLower())
            .Select(a => new Alias
            {
                Id = a.Id,
                SiteId = a.SiteId,
                AliasUrl = a.AliasUrl,
                RedirectUrl = a.RedirectUrl,
                Type = a.Type,
                Created = a.Created,
                LastModified = a.LastModified
            })
            .ToListAsync();
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(Alias model)
    {
        var alias = await _db.Aliases
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (alias == null)
        {
            alias = new Data.Alias
            {
                Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId().ToString(),
                Created = DateTime.Now
            };
            // await _db.Aliases.AddAsync(alias).ConfigureAwait(false);
            await _db.session.StoreAsync(alias);
        }

        alias.SiteId = model.SiteId;
        alias.AliasUrl = model.AliasUrl;
        alias.RedirectUrl = model.RedirectUrl;
        alias.Type = model.Type;
        alias.LastModified = DateTime.Now;

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var alias = await _db.Aliases
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alias != null)
        {
            //_db.Aliases.Remove(alias);
            _db.session.Delete(alias);
            await _db.SaveChangesAsync();
        }
    }
}

