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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class SiteService : ISiteService
    {
        [Serializable]
        public class SiteMapping
        {
            public Guid Id { get; set; }
            public string Hostnames { get; set; }
        }

        private readonly ISiteRepository _repo;
        private readonly IContentFactory _factory;
        private readonly ICache _cache;
        private const string SITE_MAPPINGS = "Site_Mappings";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="factory">The content factory</param>
        /// <param name="cache">The optional model cache</param>
        public SiteService(ISiteRepository repo, IContentFactory factory, ICache cache = null)
        {
            _repo = repo;
            _factory = factory;

            if ((int)App.CacheLevel > 0)
            {
                _cache = cache;
            }
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public Task<IEnumerable<Site>> GetAllAsync()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public async Task<Site> GetByIdAsync(Guid id)
        {
            var model = _cache?.Get<Site>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetById(id).ConfigureAwait(false);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <returns>The model</returns>
        public async Task<Site> GetByInternalIdAsync(string internalId)
        {
            var id = _cache?.Get<Guid?>($"SiteId_{internalId}");
            Site model = null;

            if (id != null)
            {
                model = await GetByIdAsync(id.Value).ConfigureAwait(false);
            }
            else
            {
                model = await _repo.GetByInternalId(internalId).ConfigureAwait(false);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given hostname.
        /// </summary>
        /// <param name="hostname">The hostname</param>
        /// <returns>The model</returns>
        public async Task<Site> GetByHostnameAsync(string hostname)
        {
            IList<SiteMapping> mappings;

            if (_cache != null)
            {
                mappings = _cache.Get<IList<SiteMapping>>(SITE_MAPPINGS);

                if (mappings == null)
                {
                    var sites = await GetAllAsync().ConfigureAwait(false);
                    mappings = sites
                        .Where(s => s.Hostnames != null)
                        .Select(s => new SiteMapping
                        {
                            Id = s.Id,
                            Hostnames = s.Hostnames
                        })
                        .ToList();
                    _cache.Set(SITE_MAPPINGS, mappings);
                }
            }
            else
            {
                var sites = await GetAllAsync().ConfigureAwait(false);
                mappings = sites
                    .Where(s => s.Hostnames != null)
                    .Select(s => new SiteMapping
                    {
                        Id = s.Id,
                        Hostnames = s.Hostnames
                    })
                    .ToList();
            }

            foreach (var mapping in mappings)
            {
                foreach (var host in mapping.Hostnames.Split(new [] { ',' }))
                {
                    if (host.Trim().ToLower() == hostname)
                    {
                        return await GetByIdAsync(mapping.Id).ConfigureAwait(false);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <returns>The model, or NULL if it does not exist</returns>
        public async Task<Site> GetDefaultAsync()
        {
            var model = _cache?.Get<Site>($"Site_{Guid.Empty}");

            if (model == null)
            {
                model = await _repo.GetDefault().ConfigureAwait(false);

                OnLoad(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <returns>The site content model</returns>
        public Task<DynamicSiteContent> GetContentByIdAsync(Guid id)
        {
            return GetContentByIdAsync<DynamicSiteContent>(id);
        }

        /// <summary>
        /// Gets the site content for given site id.
        /// </summary>
        /// <param name="id">Site id</param>
        /// <typeparam name="T">The site model type</typeparam>
        /// <returns>The site content model</returns>
        public async Task<T> GetContentByIdAsync<T>(Guid id) where T : SiteContent<T>
        {
            SiteContentBase model = null;

            if (!typeof(DynamicSiteContent).IsAssignableFrom(typeof(T)))
            {
                model = _cache?.Get<SiteContentBase>($"SiteContent_{id}");

                if (model != null)
                {
                    await _factory.InitAsync(model, App.SiteTypes.GetById(model.TypeId));
                }
            }

            if (model == null)
            {
                model = await _repo.GetContentById<T>(id).ConfigureAwait(false);

                await OnLoadContentAsync(model).ConfigureAwait(false);
            }

            if (model != null && model is T)
            {
                return (T)model;
            }
            return null;
        }

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <returns>The sitemap</returns>
        public async Task<Sitemap> GetSitemapAsync(Guid? id = null, bool onlyPublished = true)
        {
            if (!id.HasValue)
            {
                var site = await GetDefaultAsync().ConfigureAwait(false);

                if (site != null)
                {
                    id = site.Id;
                }
            }

            if (id != null)
            {
                var sitemap = onlyPublished ? _cache?.Get<Models.Sitemap>($"Sitemap_{id}") : null;

                if (sitemap == null)
                {
                    sitemap = await _repo.GetSitemap(id.Value, onlyPublished).ConfigureAwait(false);

                    if (onlyPublished)
                    {
                        _cache?.Set($"Sitemap_{id}", sitemap);
                    }
                }
                return sitemap;
            }
            return null;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task SaveAsync(Site model)
        {
            // Ensure id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Ensure InternalId
            if (string.IsNullOrWhiteSpace(model.InternalId))
            {
                model.InternalId = Utils.GenerateInteralId(model.Title);
            }

            // Ensure InternalId uniqueness
            var site = await _repo.GetByInternalId(model.InternalId).ConfigureAwait(false);
            if (site != null && site.Id != model.Id)
            {
                throw new ValidationException($"The InternalId field must be unique");
            }

            // Ensure we have a default site
            if (model.IsDefault)
            {
                // Make sure no other site is default first
                var def = await GetDefaultAsync().ConfigureAwait(false);

                if (def != null && def.Id != model.Id)
                {
                    def.IsDefault = false;
                    await _repo.Save(def).ConfigureAwait(false);
                }
            }
            else
            {
                // Make sure we have a default site
                var def = await _repo.GetDefault().ConfigureAwait(false);
                if (def == null || def.Id == model.Id)
                    model.IsDefault = true;
            }
            // Call hooks & save
            App.Hooks.OnBeforeSave(model);
            await _repo.Save(model).ConfigureAwait(false);
            App.Hooks.OnAfterSave(model);

            // Remove from cache
            RemoveFromCache(model);
        }

        /// <summary>
        /// Saves the given site content to the site with the
        /// given id.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="model">The site content model</param>
        /// <typeparam name="T">The site content type</typeparam>
        public async Task SaveContentAsync<T>(Guid siteId, T model) where T : SiteContent<T>
        {
            // Ensure id
            if (model.Id != siteId)
            {
                model.Id = siteId;
            }
            if (model.Id == Guid.Empty)
            {
                throw new ValidationException($"The Id field is required for this operation");
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Call hooks & save
            App.Hooks.OnBeforeSave<Models.SiteContentBase>(model);
            await _repo.SaveContent(siteId, model).ConfigureAwait(false);
            App.Hooks.OnAfterSave<Models.SiteContentBase>(model);

            // Remove from cache
            RemoveContentFromCache(model);
        }

        /// <summary>
        /// Creates and initializes a new site content model of the specified type.
        /// </summary>
        /// <returns>The created site content</returns>
        public Task<T> CreateContentAsync<T>(string typeId = null) where T : Models.SiteContentBase
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(T).Name;
            }

            var type = App.SiteTypes.GetById(typeId);

            if (type != null)
            {
                return _factory.CreateAsync<T>(type);
            }
            return null;
        }

        /// <summary>
        /// Invalidates the cached version of the sitemap with the
        /// given id, if caching is enabled.
        /// </summary>
        /// <param name="id">The site id</param>
        /// <param name="updateLastModified">If the global last modified date should be updated</param>
        public async Task InvalidateSitemapAsync(Guid id, bool updateLastModified = true)
        {
            if (updateLastModified)
            {
                var site = await GetByIdAsync(id).ConfigureAwait(false);

                if (site != null)
                {
                    site.ContentLastModified = DateTime.Now;
                    await SaveAsync(site).ConfigureAwait(false);
                }
            }
            _cache?.Remove($"Sitemap_{id}");
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var model = await GetByIdAsync(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync(Site model)
        {
            // Call hooks & delete
            App.Hooks.OnBeforeDelete(model);
            await _repo.Delete(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete(model);

            // Remove from cache
            RemoveFromCache(model);
        }

        /// <summary>
        /// Removes the sitemap from the cache.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task RemoveSitemapFromCacheAsync(Guid id)
        {
            if (_cache != null)
            {
                var site = await GetByIdAsync(id).ConfigureAwait(false);

                if (site != null)
                {
                    _cache.Remove($"Sitemap_{id}");
                }
            }
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        private void OnLoad(Site model)
        {
            if (model != null)
            {
                App.Hooks.OnLoad(model);

                if (_cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                    _cache.Set($"SiteId_{model.InternalId}", model.Id);
                    if (model.IsDefault)
                    {
                        _cache.Set($"Site_{Guid.Empty}", model);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        private async Task OnLoadContentAsync(Models.SiteContentBase model)
        {
            if (model != null)
            {
                // Initialize model
                if (model is IDynamicContent dynamicModel)
                {
                    await _factory.InitDynamicAsync(dynamicModel, App.SiteTypes.GetById(model.TypeId));
                }
                else
                {
                    await _factory.InitAsync(model, App.SiteTypes.GetById(model.TypeId));
                }

                App.Hooks.OnLoad(model);

                if (_cache != null && !(model is DynamicSiteContent))
                {
                    _cache.Set($"SiteContent_{model.Id}", model);
                }
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveFromCache(Site model)
        {
            if (_cache != null)
            {
                _cache.Remove(model.Id.ToString());
                _cache.Remove($"SiteId_{model.InternalId}");

                if (model.IsDefault)
                {
                    _cache.Remove($"Site_{Guid.Empty}");
                }
                _cache.Remove(SITE_MAPPINGS);
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private void RemoveContentFromCache<T>(T model) where T : Models.SiteContentBase
        {
            _cache?.Remove($"SiteContent_{model.Id}");
        }
    }
}
