/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Repositories
{
    public class AliasRepository : BaseRepositoryWithAll<Alias>, IAliasRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public AliasRepository(IDb db, ICache cache = null)
            : base(db, cache) { }

        /// <summary>
        /// Gets the model with the given alias url.
        /// </summary>
        /// <param name="url">The unique url</param>
        /// <returns>The model</returns>
        public Alias GetByAliasUrl(string url) {
            var id = cache != null ? cache.Get<Guid?>($"AliasId_{url}") : null;
            Alias model = null;

            if (id.HasValue) {
                model = GetById(id.Value);
            } else {
                model = db.Aliases
                    .AsNoTracking()
                    .FirstOrDefault(a => a.AliasUrl == url);

                if (cache != null && model != null)
                    AddToCache(model);
            }
            return model;
        }

        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Add(Alias model) {
            PrepareInsert(model);

            // Check for alias url
            if (string.IsNullOrWhiteSpace(model.AliasUrl))
                throw new ArgumentException("Alias Url cannot be empty");

            // Check for redirect url
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
                throw new ArgumentException("Redirect Url cannot be empty");

            // Fix urls
            if (!model.AliasUrl.StartsWith("/"))
                model.AliasUrl = "/" + model.AliasUrl;
            if (!model.RedirectUrl.StartsWith("/"))
                model.RedirectUrl = "/" + model.RedirectUrl;

            db.Aliases.Add(model);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Update(Alias model) {
            PrepareUpdate(model);

            // Check for alias url
            if (string.IsNullOrWhiteSpace(model.AliasUrl))
                throw new ArgumentException("Alias Url cannot be empty");

            // Check for redirect url
            if (string.IsNullOrWhiteSpace(model.RedirectUrl))
                throw new ArgumentException("Redirect Url cannot be empty");

            // Fix urls
            if (!model.AliasUrl.StartsWith("/"))
                model.AliasUrl = "/" + model.AliasUrl;
            if (!model.RedirectUrl.StartsWith("/"))
                model.RedirectUrl = "/" + model.RedirectUrl;

            var alias = db.Aliases.FirstOrDefault(s => s.Id == model.Id);
            if (alias != null) {
                App.Mapper.Map<Alias, Alias>(model, alias);
            }
        }        

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Alias model) {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"AliasId_{model.AliasUrl}", model.Id);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Alias model) {
            cache.Remove($"AliasId_{model.AliasUrl}");

            base.RemoveFromCache(model);
        }
    }
}
