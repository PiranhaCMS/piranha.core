/*
 * Copyright (c) 2018 Håkan Edling
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
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

namespace Piranha.Repositories
{
    public class AliasRepository : BaseRepository<Alias>, IAliasRepository
    {
        private readonly Api _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public AliasRepository(Api api, IDb db, ICache cache = null)
            : base(db, cache)
        {
            _api = api;
        }

        /// <summary>
        /// Gets all available models for the specified site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The available models</returns>
        public IEnumerable<Alias> GetAll(Guid? siteId)
        {
            var models = new List<Alias>();

            if (!siteId.HasValue)
            {
                var site = _api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var aliases = db.Aliases
                .AsNoTracking()
                .Where(a => a.SiteId == siteId)
                .OrderBy(a => a.AliasUrl)
                .ThenBy(a => a.RedirectUrl)
                .Select(a => a.Id);

            foreach (var a in aliases)
            {
                var model = GetById(a);
                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the model with the given alias url.
        /// </summary>
        /// <param name="url">The unique url</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The model</returns>
        public Alias GetByAliasUrl(string url, Guid? siteId = null)
        {
            if (!siteId.HasValue)
            {
                var site = _api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var id = cache?.Get<Guid?>($"AliasId_{siteId}_{url}");
            Alias model = null;

            if (id.HasValue)
            {
                model = GetById(id.Value);
            }
            else
            {
                id = db.Aliases
                    .AsNoTracking()
                    .Where(a => a.SiteId == siteId && a.AliasUrl == url)
                    .Select(a => a.Id)
                    .FirstOrDefault();

                if (id != Guid.Empty)
                    model = GetById(id.Value);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given redirect url.
        /// </summary>
        /// <param name="url">The unique url</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The model</returns>
        public IEnumerable<Alias> GetByRedirectUrl(string url, Guid? siteId = null)
        {
            if (!siteId.HasValue)
            {
                var site = _api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var models = new List<Alias>();

            var aliases = db.Aliases
                .AsNoTracking()
                .Where(a => a.SiteId == siteId && a.RedirectUrl == url)
                .Select(a => a.Id)
                .ToList();

            foreach (var id in aliases)
            {
                models.Add(GetById(id));
            }
            return models;
        }

        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Add(Alias model)
        {
            PrepareInsert(model);

            // Check for alias url
            if (string.IsNullOrWhiteSpace(model.AliasUrl))
                throw new ArgumentException("Alias Url cannot be empty");

            // Check for redirect url
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
                throw new ArgumentException("Redirect Url cannot be empty");

            // Fix urls
            if (!model.AliasUrl.StartsWith("/"))
            {
                model.AliasUrl = "/" + model.AliasUrl;
            }
            if (!model.RedirectUrl.StartsWith("/") && !model.RedirectUrl.StartsWith("http://") && !model.RedirectUrl.StartsWith("https://"))
            {
                model.RedirectUrl = "/" + model.RedirectUrl;
            }

            db.Aliases.Add(model);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Update(Alias model)
        {
            PrepareUpdate(model);

            // Check for alias url
            if (string.IsNullOrWhiteSpace(model.AliasUrl))
                throw new ArgumentException("Alias Url cannot be empty");

            // Check for redirect url
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
                throw new ArgumentException("Redirect Url cannot be empty");

            // Fix urls
            if (!model.AliasUrl.StartsWith("/"))
            {
                model.AliasUrl = "/" + model.AliasUrl;
            }
            if (!model.RedirectUrl.StartsWith("/") && !model.RedirectUrl.StartsWith("http://") && !model.RedirectUrl.StartsWith("https://"))
            {
                model.RedirectUrl = "/" + model.RedirectUrl;
            }

            var alias = db.Aliases.FirstOrDefault(s => s.Id == model.Id);
            if (alias != null)
            {
                App.Mapper.Map<Alias, Alias>(model, alias);
            }
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Alias model)
        {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"AliasId_{model.SiteId}_{model.AliasUrl}", model.Id);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Alias model)
        {
            cache.Remove($"AliasId_{model.SiteId}_{model.AliasUrl}");

            base.RemoveFromCache(model);
        }
    }
}
