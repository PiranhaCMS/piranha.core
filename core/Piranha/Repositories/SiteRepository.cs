/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Repositories
{
    public class SiteRepository : BaseRepositoryWithAll<Site>, ISiteRepository
    {
        public class SiteMapping
        {
            public Guid Id { get; set; }
            public string Hostnames { get; set; }
        }

        private readonly Api api;
        // This is a hack as we don't really want to transform the models, we only want
        // to access the create methods.
        private readonly IContentService<Site, SiteField, Models.SiteContentBase> contentService;
        private const string SITE_MAPPINGS = "Site_Mappings";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db context</param>
        /// <param name="factory">The content service factory</param>
        /// <param name="cache">The optional model cache</param>
        public SiteRepository(Api api, IDb db, IContentServiceFactory factory, ICache cache = null)
            : base(db, cache) 
        { 
            this.api = api;
            this.contentService = factory.CreateSiteService();
        }

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        public Site GetByInternalId(string internalId) {
            var id = cache != null ? cache.Get<Guid?>($"SiteId_{internalId}") : null;
            Site model = null;

            if (id.HasValue) {
                model = GetById(id.Value);
            } else {
                id = db.Sites
                    .AsNoTracking()
                    .Where(s => s.InternalId == internalId)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                if (id != Guid.Empty)
                    model = GetById(id.Value);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given hostname.
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <returns>The model</returns>
        public Site GetByHostname(string hostname) {
            IList<SiteMapping> mappings;

            if (cache != null)
            {
                mappings = cache.Get<IList<SiteMapping>>(SITE_MAPPINGS);

                if (mappings == null)
                {
                    mappings = db.Sites
                        .AsNoTracking()
                        .Where(s => s.Hostnames != null)
                        .Select(s => new SiteMapping
                        {
                            Id = s.Id,
                            Hostnames = s.Hostnames
                        })
                        .ToList();
                    cache.Set(SITE_MAPPINGS, mappings);
                }
            }
            else
            {
                mappings = db.Sites
                    .AsNoTracking()
                    .Where(s => s.Hostnames.Contains(hostname))
                    .Select(s => new SiteMapping
                    {
                        Id = s.Id,
                        Hostnames = s.Hostnames
                    })
                    .ToList();
            }
            
            foreach (var mapping in mappings)
            {
                foreach (var host in mapping.Hostnames.Split(new char[] { ',' }))
                {
                    if (host.Trim().ToLower() == hostname)
                        return GetById(mapping.Id);
                }
            }
            return null;
        }        

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        public Site GetDefault() {
            Site model = cache != null ? cache.Get<Site>($"Site_{Guid.Empty}") : null;

            if (model == null) {
                var id = db.Sites
                    .AsNoTracking()
                    .Where(s => s.IsDefault)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                if (id != Guid.Empty)
                    model = GetById(id);
            }
            return model;
        }

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <returns>The site content model</returns>
        public Models.DynamicSiteContent GetContentById(Guid id)
        {
            return GetContentById<Models.DynamicSiteContent>(id);
        }

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <typeparam name="T">The site model type</typeparam>
        /// <returns>The site content model</returns>
        public T GetContentById<T>(Guid id) where T : Models.SiteContent<T>
        {
            var site = cache != null ? cache.Get<Data.Site>($"SiteContent_{id}") : null;
            if (site == null)
            {
                site = db.Sites
                    .Include(s => s.Fields)
                    .Where(s => s.Id == id)
                    .FirstOrDefault();

                if (site == null)
                    return null;
            }

            if (string.IsNullOrEmpty(site.SiteTypeId))
                return null;

            var type = api.SiteTypes.GetById(site.SiteTypeId);
            if (type == null)
                return null;

            if (cache != null)
                cache.Set($"SiteContent_{id}", site);

            return contentService.Transform<T>(site, type);
        }

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        public Models.Sitemap GetSitemap(Guid? id = null, bool onlyPublished = true) {
            if (!id.HasValue) {
                var site = GetDefault();

                if (site != null)
                    id = site.Id;
            }

            if (id != null) {
                var sitemap = onlyPublished && cache != null ? cache.Get<Models.Sitemap>($"Sitemap_{id}") : null;

                if (sitemap == null) {
                    var pages = db.Pages
                        .AsNoTracking()
                        .Where(p => p.SiteId == id)
                        .OrderBy(p => p.ParentId)
                        .ThenBy(p => p.SortOrder)
                        .ToList();

                    var pageTypes = api.PageTypes.GetAll();

                    if (onlyPublished)
                        pages = pages.Where(p => p.Published.HasValue).ToList();
                    sitemap = Sort(pages, pageTypes);

                    if (onlyPublished && cache != null)
                        cache.Set($"Sitemap_{id}", sitemap);
                }
                return sitemap;
            }
            return null;
        }

        /// <summary>
        /// Saves the given site content to the site with the 
        /// given id.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="content">The site content</param>
        /// <typeparam name="T">The site content type</typeparam>
        public void SaveContent<T>(Guid siteId, T content) where T : Models.SiteContent<T>
        {
            var site = db.Sites
                .Include(s => s.Fields)
                .FirstOrDefault(s => s.Id == siteId);

            if (site != null)
            {
                if (string.IsNullOrEmpty(site.SiteTypeId))
                    throw new MissingFieldException("Can't save content for a site that doesn't have a Site Type Id.");

                var type = api.SiteTypes.GetById(site.SiteTypeId);
                if (type == null)
                    throw new MissingFieldException("The specified Site Type is missing. Can't save content.");

                content.Id = siteId;
                content.TypeId = site.SiteTypeId;
                content.Title = site.Title;

                contentService.Transform(content, type, site);

                // Since we've updated global site content, update the
                // global last modified date for the site.
                site.ContentLastModified = DateTime.Now;

                db.SaveChanges();

                if (cache != null)
                    cache.Remove($"SiteContent_{siteId}");
            }
        }

        /// <summary>
        /// Creates and initializes a new site content model of the specified type.
        /// </summary>
        /// <returns>The created site content</returns>
        public T CreateContent<T>(string typeId = null) where T : Models.SiteContentBase {
            if (string.IsNullOrWhiteSpace(typeId))
                typeId = typeof(T).Name;

            return contentService.Create<T>(api.SiteTypes.GetById(typeId));
        }

        /// <summary>
        /// Invalidates the cached version of the sitemap with the
        /// given id, if caching is enabled.
        /// </summary>
        /// <param name="id">The site id</param>
        public void InvalidateSitemap(Guid id)
        {
            cache?.Remove($"Sitemap_{id}");
        }

        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Add(Site model) {
            PrepareInsert(model);

            // Check for title
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title cannot be empty");

            // Ensure InternalId
            if (string.IsNullOrWhiteSpace(model.InternalId))
                model.InternalId = Utils.GenerateInteralId(model.Title);

            if (model.IsDefault) {
                // Make sure no other site is default first
                var def = GetDefault();

                if (def != null && def.Id != model.Id) {
                    def.IsDefault = false;
                    Update(def, false);
                }
            } else {
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
        protected override void Update(Site model) {
            Update(model, true);
        }        

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="checkDefault">If default site integrity should be validated</param>
        protected void Update(Site model, bool checkDefault = true) {
            PrepareUpdate(model);

            // Check for title
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title cannot be empty");

            // Ensure InternalId
            if (string.IsNullOrWhiteSpace(model.InternalId))
                model.InternalId = Utils.GenerateInteralId(model.Title);

            if (checkDefault) {
                if (model.IsDefault) {
                    // Make sure no other site is default first
                    var def = GetDefault();

                    if (def != null && def.Id != model.Id) {
                        def.IsDefault = false;
                        Update(def, false);
                    }
                } else {
                    // Make sure we have a default site
                    var count = db.Sites.Count(s => s.IsDefault && s.Id != model.Id);
                    if (count == 0)
                        model.IsDefault = true;
                }
            }
            var site = db.Sites.FirstOrDefault(s => s.Id == model.Id);
            if (site != null) {
                App.Mapper.Map<Site, Site>(model, site);
            }
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Site model) {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"SiteId_{model.InternalId}", model.Id);
            if (model.IsDefault)
                cache.Set($"Site_{Guid.Empty}", model);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Site model) {
            cache.Remove($"SiteId_{model.InternalId}");
            if (model.IsDefault)
                cache.Remove($"Site_{Guid.Empty}");
            cache.Remove(SITE_MAPPINGS);

            base.RemoveFromCache(model);
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="pages">The full page list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The sitemap</returns>
        private Models.Sitemap Sort(IEnumerable<Page> pages, IEnumerable<Models.PageType> pageTypes, Guid? parentId = null, int level = 0) {
            var result = new Models.Sitemap();

            foreach (var page in pages.Where(p => p.ParentId == parentId).OrderBy(p => p.SortOrder)) {
                var item = App.Mapper.Map<Page, Models.SitemapItem>(page);

                if (!string.IsNullOrEmpty(page.RedirectUrl))
                    item.Permalink = page.RedirectUrl;

                item.Level = level;
                item.PageTypeName = pageTypes.First(t => t.Id == page.PageTypeId).Title;
                item.Items = Sort(pages, pageTypes, page.Id, level + 1);

                result.Add(item);
            }
            return result;
        }
    }
}
