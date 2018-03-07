/*
 * Copyright (c) 2017-2018 Håkan Edling
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
using Piranha.Models;
using PageType = Piranha.Models.PageType;

namespace Piranha.Repositories
{
    public class SiteRepository : BaseRepositoryWithAll<Site>, ISiteRepository
    {
        private readonly Api _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public SiteRepository(Api api, IDb db, ICache cache = null)
            : base(db, cache)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        public Site GetByInternalId(string internalId)
        {
            var id = cache?.Get<Guid?>($"SiteId_{internalId}");
            Site model = null;

            if (id.HasValue)
            {
                model = GetById(id.Value);
            }
            else
            {
                id = db.Sites
                    .AsNoTracking()
                    .Where(s => s.InternalId == internalId)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                if (id != Guid.Empty)
                {
                    model = GetById(id.Value);
                }
            }

            return model;
        }

        /// <summary>
        /// Gets the model with the given hostname.
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <returns>The model</returns>
        public Site GetByHostname(string hostname)
        {
            var id = db.Sites
                .AsNoTracking()
                .Where(s => s.Hostnames.Contains(hostname))
                .Select(s => s.Id)
                .FirstOrDefault();

            return id != Guid.Empty ? GetById(id) : null;
        }

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        public Site GetDefault()
        {
            var model = cache?.Get<Site>($"Site_{Guid.Empty}");

            if (model != null)
            {
                return model;
            }

            var id = db.Sites
                .AsNoTracking()
                .Where(s => s.IsDefault)
                .Select(s => s.Id)
                .FirstOrDefault();

            if (id != Guid.Empty)
            {
                model = GetById(id);
            }

            return model;
        }

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        public Sitemap GetSitemap(Guid? id = null, bool onlyPublished = true)
        {
            if (!id.HasValue)
            {
                var site = GetDefault();

                if (site != null)
                    id = site.Id;
            }

            if (id == null)
            {
                return null;
            }

            var sitemap = onlyPublished && cache != null ? cache.Get<Sitemap>($"Sitemap_{id}") : null;

            if (sitemap != null)
            {
                return sitemap;
            }

            var pages = db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == id)
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .ToList();

            var pageTypes = _api.PageTypes.GetAll();

            if (onlyPublished)
            {
                pages = pages.Where(p => p.Published.HasValue).ToList();
            }
            sitemap = Sort(pages, pageTypes);

            if (onlyPublished && cache != null)
            {
                cache.Set($"Sitemap_{id}", sitemap);
            }

            return sitemap;
        }

        /// <summary>
        /// Removes the specified public sitemap from
        /// the cache.
        /// </summary>
        /// <param name="id">The site id</param>
        public void InvalidateSitemap(Guid id)
        {
            cache?.Remove($"Sitemap_{id}");
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Add(Site model)
        {
            PrepareInsert(model);

            // Check for title
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            // Ensure InternalId
            if (string.IsNullOrWhiteSpace(model.InternalId))
            {
                model.InternalId = Utils.GenerateInteralId(model.Title);
            }

            if (model.IsDefault)
            {
                // Make sure no other site is default first
                var def = GetDefault();

                if (def != null && def.Id != model.Id)
                {
                    def.IsDefault = false;
                    Update(def, false);
                }
            }
            else
            {
                // Make sure we have a default site
                var count = db.Sites.Count(s => s.IsDefault);
                if (count == 0)
                    model.IsDefault = true;
            }
            db.Sites.Add(model);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Update(Site model)
        {
            Update(model, true);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="checkDefault">If default site integrity should be validated</param>
        protected void Update(Site model, bool checkDefault = true)
        {
            PrepareUpdate(model);

            // Check for title
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            // Ensure InternalId
            if (string.IsNullOrWhiteSpace(model.InternalId))
            {
                model.InternalId = Utils.GenerateInteralId(model.Title);
            }

            if (checkDefault)
            {
                if (model.IsDefault)
                {
                    // Make sure no other site is default first
                    var def = GetDefault();

                    if (def != null && def.Id != model.Id)
                    {
                        def.IsDefault = false;
                        Update(def, false);
                    }
                }
                else
                {
                    // Make sure we have a default site
                    var count = db.Sites.Count(s => s.IsDefault && s.Id != model.Id);
                    if (count == 0)
                    {
                        model.IsDefault = true;
                    }
                }
            }
            var site = db.Sites.FirstOrDefault(s => s.Id == model.Id);
            if (site != null)
            {
                App.Mapper.Map(model, site);
            }
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Site model)
        {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"SiteId_{model.InternalId}", model.Id);
            if (model.IsDefault)
            {
                cache.Set($"Site_{Guid.Empty}", model);
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Site model)
        {
            cache.Remove($"SiteId_{model.InternalId}");
            if (model.IsDefault)
            {
                cache.Remove($"Site_{Guid.Empty}");
            }

            base.RemoveFromCache(model);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="pages">The full page list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The sitemap</returns>
        private Sitemap Sort(IEnumerable<Page> pages, IEnumerable<PageType> pageTypes, Guid? parentId = null, int level = 0)
        {
            var result = new Sitemap();

            foreach (var page in pages.Where(p => p.ParentId == parentId).OrderBy(p => p.SortOrder))
            {
                var item = App.Mapper.Map<Page, SitemapItem>(page);

                item.Level = level;
                item.PageTypeName = pageTypes.First(t => t.Id == page.PageTypeId).Title;
                item.Items = Sort(pages, pageTypes, page.Id, level + 1);

                result.Add(item);
            }

            return result;
        }
        #endregion
    }
}
