/*
 * Copyright (c) 2018-2019 Håkan Edling
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
    public class PageService : IPageService
    {
        private readonly IPageRepository _repo;
        private readonly IContentFactory _factory;
        private readonly ISiteService _siteService;
        private readonly IParamService _paramService;
        private readonly ICache _cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="cache">The optional model cache</param>
        public PageService(IPageRepository repo, IContentFactory factory, ISiteService siteService, IParamService paramService, ICache cache = null)
        {
            _repo = repo;
            _factory = factory;
            _siteService = siteService;
            _paramService = paramService;

            if ((int)App.CacheLevel > 2)
            {
                _cache = cache;
            }
        }

        /// <summary>
        /// Creates and initializes a new page of the specified type.
        /// </summary>
        /// <returns>The created page</returns>
        public T Create<T>(string typeId = null) where T : Models.PageBase
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(T).Name;
            }

            var type = App.PageTypes.GetById(typeId);

            if (type != null)
            {
                return _factory.Create<T>(type);
            }
            return null;
        }

        /// <summary>
        /// Creates and initializes a copy of the given page.
        /// </summary>
        /// <param name="originalPage">The orginal page</param>
        /// <returns>The created copy</returns>
        public T Copy<T>(T originalPage) where T : Models.PageBase
        {
            var model = Create<T>(originalPage.TypeId);
            model.OriginalPageId = originalPage.Id;
            model.Slug = null;

            return model;
        }

        /// <summary>
        /// Creates and initializes a copy of the given page.
        /// </summary>
        /// <param name="originalPage">The orginal page</param>
        /// <returns>The created copy</returns>
        public async Task<T> CopyAsync<T>(T originalPage) where T : Models.PageBase
        {
            var model = await GetByIdAsync<T>(originalPage.Id).ConfigureAwait(false);

            model.Id = Guid.NewGuid();
            model.OriginalPageId = originalPage.Id;
            model.Title = $"Copy of { model.Title }";
            model.NavigationTitle = null;
            model.Slug = null;
            model.Created = model.LastModified = DateTime.MinValue;
            model.Published = null;

            return model;
        }

        /// <summary>
        /// Detaches a copy and initializes it as a standalone page
        /// </summary>
        /// <returns>The standalone page</returns>
        public async Task DetachAsync<T>(T model) where T : Models.PageBase
        {
            if (!model.OriginalPageId.HasValue)
            {
                throw new ValidationException("Page is not an copy");
            }

            // Get the page and remove the original id
            var page = await GetByIdAsync<T>(model.Id).ConfigureAwait(false);
            page.OriginalPageId = null;

            // Reset blocks so they are recreated
            foreach (var pageBlock in page.Blocks)
            {
                pageBlock.Id = Guid.Empty;

                if (pageBlock is Extend.BlockGroup)
                {
                    foreach (var childBlock in ((Extend.BlockGroup)pageBlock).Items)
                    {
                        childBlock.Id = Guid.Empty;
                    }
                }
            }

            await SaveAsync(page).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public Task<IEnumerable<DynamicPage>> GetAllAsync(Guid? siteId = null)
        {
            return GetAllAsync<DynamicPage>(siteId);
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<T>> GetAllAsync<T>(Guid? siteId = null) where T : PageBase
        {
            var models = new List<T>();
            var pages = await _repo.GetAll((await EnsureSiteIdAsync(siteId).ConfigureAwait(false)).Value)
                .ConfigureAwait(false);

            foreach (var pageId in pages)
            {
                var page = await GetByIdAsync<T>(pageId).ConfigureAwait(false);

                if (page != null)
                {
                    models.Add(page);
                }
            }
            return models;
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public Task<IEnumerable<DynamicPage>> GetAllBlogsAsync(Guid? siteId = null)
        {
            return GetAllBlogsAsync<DynamicPage>(siteId);
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public async Task<IEnumerable<T>> GetAllBlogsAsync<T>(Guid? siteId = null) where T : Models.PageBase
        {
            var models = new List<T>();
            var pages = await _repo.GetAllBlogs((await EnsureSiteIdAsync(siteId).ConfigureAwait(false)).Value)
                .ConfigureAwait(false);

            foreach (var pageId in pages)
            {
                var page = await GetByIdAsync<T>(pageId).ConfigureAwait(false);

                if (page != null)
                {
                    models.Add(page);
                }
            }
            return models;
        }

        /// <summary>
        /// Gets the id of all pages that have a draft for
        /// the specified site.
        /// </summary>
        /// <param name="siteId">The unique site id</param>
        /// <returns>The pages that have a draft</returns>
        public async Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid? siteId = null)
        {
            return await _repo.GetAllDrafts((await EnsureSiteIdAsync(siteId).ConfigureAwait(false)).Value);
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public Task<DynamicPage> GetStartpageAsync(Guid? siteId = null)
        {
            return GetStartpageAsync<DynamicPage>(siteId);
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public async Task<T> GetStartpageAsync<T>(Guid? siteId = null) where T : Models.PageBase
        {
            siteId = await EnsureSiteIdAsync(siteId).ConfigureAwait(false);
            PageBase model = null;

            if (typeof(T) == typeof(Models.PageInfo))
            {
                model = _cache?.Get<PageInfo>($"PageInfo_{siteId.Value}");
            }
            else if (!typeof(DynamicPage).IsAssignableFrom(typeof(T)))
            {
                model = _cache?.Get<PageBase>($"Page_{siteId.Value}");

                if (model != null)
                {
                    _factory.Init(model, App.PageTypes.GetById(model.TypeId));
                }
            }

            if (model == null)
            {
                model = await _repo.GetStartpage<T>(siteId.Value).ConfigureAwait(false);

                OnLoad(model);
            }

            if (model != null && model is T)
            {
                return await MapOriginalAsync<T>((T)model).ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public Task<DynamicPage> GetByIdAsync(Guid id)
        {
            return GetByIdAsync<DynamicPage>(id);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public async Task<T> GetByIdAsync<T>(Guid id) where T : PageBase
        {
            PageBase model = null;

            if (typeof(T) == typeof(Models.PageInfo))
            {
                model = _cache?.Get<PageInfo>($"PageInfo_{id.ToString()}");
            }
            else if (!typeof(DynamicPage).IsAssignableFrom(typeof(T)))
            {
                model = _cache?.Get<PageBase>(id.ToString());

                if (model != null)
                {
                    _factory.Init(model, App.PageTypes.GetById(model.TypeId));
                }
            }

            if (model == null)
            {
                model = await _repo.GetById<T>(id).ConfigureAwait(false);

                OnLoad(model);
            }

            if (model != null && model is T)
            {
                return await MapOriginalAsync<T>((T)model).ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public Task<DynamicPage> GetBySlugAsync(string slug, Guid? siteId = null)
        {
            return GetBySlugAsync<DynamicPage>(slug, siteId);
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public async Task<T> GetBySlugAsync<T>(string slug, Guid? siteId = null) where T : Models.PageBase
        {
            siteId = await EnsureSiteIdAsync(siteId).ConfigureAwait(false);
            PageBase model = null;

            // Lets see if we can resolve the slug from cache
            var pageId = _cache?.Get<Guid?>($"PageId_{siteId}_{slug}");

            if (pageId.HasValue)
            {
                if (typeof(T) == typeof(Models.PageInfo))
                {
                    model = _cache?.Get<PageInfo>($"PageInfo_{pageId.ToString()}");
                }
                else if (!typeof(DynamicPage).IsAssignableFrom(typeof(T)))
                {
                    model = _cache?.Get<PageBase>(pageId.ToString());

                    if (model != null)
                    {
                        _factory.Init(model, App.PageTypes.GetById(model.TypeId));
                    }
                }
            }

            if (model == null)
            {
                model = await _repo.GetBySlug<T>(slug, siteId.Value).ConfigureAwait(false);

                OnLoad(model);
            }

            if (model != null && model is T)
            {
                return await MapOriginalAsync<T>((T)model).ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Gets the id for the page with the given slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional page id</param>
        /// <returns>The id</returns>
        public async Task<Guid?> GetIdBySlugAsync(string slug, Guid? siteId = null)
        {
            siteId = await EnsureSiteIdAsync(siteId).ConfigureAwait(false);

            // Lets see if we can resolve the slug from cache
            var pageId = _cache?.Get<Guid?>($"PageId_{siteId}_{slug}");

            if (!pageId.HasValue)
            {
                var info = await _repo.GetBySlug<PageInfo>(slug, siteId.Value).ConfigureAwait(false);

                if (info != null)
                {
                    pageId = info.Id;
                }
            }
            return pageId;
        }

        /// <summary>
        /// Gets the draft for the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        public Task<DynamicPage> GetDraftByIdAsync(Guid id)
        {
            return GetDraftByIdAsync<DynamicPage>(id);
        }

        /// <summary>
        /// Gets the draft for the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        public async Task<T> GetDraftByIdAsync<T>(Guid id) where T : PageBase
        {
            var draft = await _repo.GetDraftById<T>(id).ConfigureAwait(false);

            OnLoad(draft, true);

            return draft;
        }

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        public async Task MoveAsync<T>(T model, Guid? parentId, int sortOrder) where T : Models.PageBase
        {
            // Call hooks & save
            App.Hooks.OnBeforeSave<PageBase>(model);
            var affected = await _repo.Move(model, parentId, sortOrder).ConfigureAwait(false);
            App.Hooks.OnAfterSave<PageBase>(model);

            // Remove the moved page from cache
            await RemoveFromCache(model).ConfigureAwait(false);

            // Remove all affected pages from cache
            if (_cache != null)
            {
                foreach (var id in affected)
                {
                    var page = await GetByIdAsync<PageInfo>(id).ConfigureAwait(false);
                    if (page != null)
                    {
                        await RemoveFromCache(model).ConfigureAwait(false);
                    }
                }
            }
            await _siteService.InvalidateSitemapAsync(model.SiteId).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        public Task SaveAsync<T>(T model) where T : PageBase
        {
            return SaveAsync(model, false);
        }

        /// <summary>
        /// Saves the given page model as a draft
        /// </summary>
        /// <param name="model">The page model</param>
        public Task SaveDraftAsync<T>(T model) where T : PageBase
        {
            return SaveAsync(model, true);
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        /// <param name="isDraft">If we're saving as a draft</param>
        private async Task SaveAsync<T>(T model, bool isDraft) where T : PageBase
        {
            // Ensure id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Ensure title since this field isn't required in
            // the Content base class
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ValidationException("The Title field is required");
            }

            // Ensure type id since this field isn't required in
            // the Content base class
            if (string.IsNullOrWhiteSpace(model.TypeId))
            {
                throw new ValidationException("The TypeId field is required");
            }

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                var prefix = "";

                // Check if we should generate hierarchical slugs
                using (var config = new Config(_paramService))
                {
                    if (config.HierarchicalPageSlugs && model.ParentId.HasValue)
                    {
                        var parentSlug = (await GetByIdAsync<PageInfo>(model.ParentId.Value).ConfigureAwait(false))?.Slug;

                        if (!string.IsNullOrWhiteSpace(parentSlug))
                        {
                            prefix = parentSlug + "/";
                        }
                    }
                    model.Slug = prefix + Utils.GenerateSlug(!string.IsNullOrWhiteSpace(model.NavigationTitle) ? model.NavigationTitle : model.Title);
                }
            }
            else
            {
                model.Slug = Utils.GenerateSlug(model.Slug);
            }

            // Check if we're changing the state
            var current = await _repo.GetById<PageInfo>(model.Id).ConfigureAwait(false);
            var changeState = IsPublished(current) != IsPublished(model);

            IEnumerable<Guid> affected = new Guid[0];

            // Call before save hook
            App.Hooks.OnBeforeSave<PageBase>(model);

            // Handle revisions and save
            if (IsPublished(current) && isDraft)
            {
                // We're saving a draft since we have a previously
                // published version of the page
                await _repo.SaveDraft(model).ConfigureAwait(false);
            }
            else
            {
                if (current == null && isDraft)
                {
                    // If we're saving a draft as a normal page instance, make
                    // sure we remove the published date as this sould effectively
                    // publish the page.
                    model.Published = null;
                }
                else if (current != null && !isDraft)
                {
                    using (var config = new Config(_paramService))
                    {
                        // Save current as a revision before saving the model
                        // and if a draft revision exists, remove it.
                        await _repo.DeleteDraft(model.Id).ConfigureAwait(false);
                        await _repo.CreateRevision(model.Id, config.PageRevisions).ConfigureAwait(false);
                    }
                }

                // Save the main page
                affected = await _repo.Save(model).ConfigureAwait(false);
            }

            // Call after save hook
            App.Hooks.OnAfterSave<PageBase>(model);

            // Remove from cache
            await RemoveFromCache(model).ConfigureAwait(false);

            // Remove all affected pages from cache
            if (_cache != null)
            {
                foreach (var id in affected)
                {
                    var page = await GetByIdAsync<PageInfo>(id).ConfigureAwait(false);
                    if (page != null)
                    {
                        await RemoveFromCache(model).ConfigureAwait(false);
                    }
                }
            }

            // Invalidate sitemap if any other pages were affected
            if (changeState || affected.Count() > 0)
            {
                await _siteService.InvalidateSitemapAsync(model.SiteId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var model = await GetByIdAsync<PageInfo>(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync<T>(T model) where T : Models.PageBase
        {
            // Call hooks & save
            App.Hooks.OnBeforeDelete<PageBase>(model);
            await _repo.Delete(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete<PageBase>(model);

            // Remove from cache & invalidate sitemap
            await RemoveFromCache(model).ConfigureAwait(false);

            await _siteService.InvalidateSitemapAsync(model.SiteId).ConfigureAwait(false);
        }

        /// <summary>
        /// Merges the given model with the original model and
        /// returns it as a new instance.
        /// </summary>
        /// <param name="model">The model</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The new merged model</returns>
        private async Task<T> MapOriginalAsync<T>(T model) where T : PageBase
        {
            if (model == null || !model.OriginalPageId.HasValue)
            {
                return model;
            }

            var original = await GetByIdAsync<T>(model.OriginalPageId.Value).ConfigureAwait(false);

            if (original != null)
            {
                T copy = null;

                if (model is DynamicPage)
                {
                    // No need to clone as we don't cache dynamic models
                    copy = original;
                }
                else
                {
                    // Clone the original in case we are caching in system
                    // memory, otherwise we'll destroy the original.
                    copy = Utils.DeepClone<T>(original);

                    // Initialize all blocks & regions
                    _factory.Init(copy, App.PageTypes.GetById(copy.TypeId));
                }

                // Now let's move over the fields we want to the
                // soft copy.
                copy.Id = model.Id;
                copy.SiteId = model.SiteId;
                copy.Title = model.Title;
                copy.NavigationTitle = model.NavigationTitle;
                copy.Slug = model.Slug;
                copy.ParentId = model.ParentId;
                copy.SortOrder = model.SortOrder;
                copy.IsHidden = model.IsHidden;
                copy.Route = model.Route;
                copy.OriginalPageId = model.OriginalPageId;
                copy.Published = model.Published;
                copy.Created = model.Created;
                copy.LastModified = model.LastModified;

                return copy;
            }
            return null;
        }

        /// <summary>
        /// Checks if the given site id is empty, and if so
        /// gets the site id of the default site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The site id</returns>
        private async Task<Guid?> EnsureSiteIdAsync(Guid? siteId)
        {
            if (!siteId.HasValue)
            {
                var site = await _siteService.GetDefaultAsync().ConfigureAwait(false);

                if (site != null)
                {
                    return site.Id;
                }
            }
            return siteId;
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="isDraft">If this is a draft</param>
        private void OnLoad(PageBase model, bool isDraft = false)
        {
            if (model != null)
            {
                // Initialize model
                if (typeof(IDynamicModel).IsAssignableFrom(model.GetType()))
                {
                    _factory.InitDynamic((DynamicPage)model, App.PageTypes.GetById(model.TypeId));
                }
                else
                {
                    _factory.Init(model, App.PageTypes.GetById(model.TypeId));
                }

                App.Hooks.OnLoad<PageBase>(model);

                // Never cache drafts, dynamic or simple instances
                if (!isDraft && _cache != null && !(model is DynamicPage))
                {
                    if (model is PageInfo)
                    {
                        _cache.Set($"PageInfo_{model.Id.ToString()}", model);
                    }
                    else
                    {
                        _cache.Set(model.Id.ToString(), model);
                    }
                    _cache.Set($"PageId_{model.SiteId}_{model.Slug}", model.Id);
                    if (!model.ParentId.HasValue && model.SortOrder == 0)
                    {
                        if (model is PageInfo)
                        {
                            _cache.Set($"PageInfo_{model.SiteId}", model);
                        }
                        else
                        {
                            _cache.Set($"Page_{model.SiteId}", model);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        private async Task RemoveFromCache(PageBase model)
        {
            if (_cache != null)
            {
                _cache.Remove(model.Id.ToString());
                _cache.Remove($"PageInfo_{model.Id.ToString()}");
                _cache.Remove($"PageId_{model.SiteId}_{model.Slug}");
                if (!model.ParentId.HasValue && model.SortOrder == 0)
                {
                    _cache.Remove($"Page_{model.SiteId}");
                    _cache.Remove($"PageInfo_{model.SiteId}");
                }

                // Remove the site & clear the sitemap from cache
                await _siteService.RemoveSitemapFromCacheAsync(model.SiteId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks if the given page is published
        /// </summary>
        /// <param name="model">The page model</param>
        /// <returns>If the page is published</returns>
        private bool IsPublished (PageBase model)
        {
            return model != null && model.Published.HasValue && model.Published.Value <= DateTime.Now;
        }
    }
}
