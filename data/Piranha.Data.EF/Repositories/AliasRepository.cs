/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.Repositories
{
    public class AliasRepository : IAliasRepository
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
        public async Task<IEnumerable<Alias>> GetAll(Guid siteId)
        {
            return await _db.Aliases
                .AsNoTracking()
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
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        public Task<Alias> GetById(Guid id)
        {
            return _db.Aliases
                .AsNoTracking()
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
        public Task<Alias> GetByAliasUrl(string url, Guid siteId)
        {
            return _db.Aliases
                .AsNoTracking()
                .Where(a => a.SiteId == siteId && a.AliasUrl == url)
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
        /// Gets the model with the given redirect url.
        /// </summary>
        /// <param name="url">The unique url</param>
        /// <param name="siteId">The site id</param>
        /// <returns>The model</returns>
        public async Task<IEnumerable<Alias>> GetByRedirectUrl(string url, Guid siteId)
        {
            return await _db.Aliases
                .AsNoTracking()
                .Where(a => a.SiteId == siteId && a.RedirectUrl == url)
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
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task Save(Alias model)
        {
            var alias = await _db.Aliases
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                .ConfigureAwait(false);

            if (alias == null)
            {
                alias = new Data.Alias
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                await _db.Aliases.AddAsync(alias).ConfigureAwait(false);
            }
            alias.SiteId = model.SiteId;
            alias.AliasUrl = model.AliasUrl;
            alias.RedirectUrl = model.RedirectUrl;
            alias.Type = model.Type;
            alias.LastModified = DateTime.Now;

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var alias = await _db.Aliases
                .FirstOrDefaultAsync(a => a.Id == id)
                .ConfigureAwait(false);

            if (alias != null)
            {
                _db.Aliases.Remove(alias);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
