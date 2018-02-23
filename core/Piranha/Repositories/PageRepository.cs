/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Piranha.Repositories
{
    public class PageRepository : ContentRepository<Page, PageField>, IPageRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db connection</param>
        /// <param name="modelCache">The optional model cache</param>
        public PageRepository(Api api, IDb db, ICache modelCache = null) : base(api, db, modelCache) { }

        /// <summary>
        /// Gets all of the available pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public IEnumerable<Models.DynamicPage> GetAll(Guid? siteId = null) {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();

                if (site != null)
                    siteId = site.Id;
            }

            var pages = db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == siteId)
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .Select(p => p.Id);

            var models = new List<Models.DynamicPage>();

            foreach (var page in pages) {
                var model = GetById(page);

                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public IEnumerable<Models.DynamicPage> GetAllBlogs(Guid? siteId = null) {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();

                if (site != null)
                    siteId = site.Id;
            }

            var pages = db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == siteId && p.ContentType == "Blog")
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .Select(p => p.Id);

            var models = new List<Models.DynamicPage>();

            foreach (var page in pages) {
                var model = GetById(page);

                if (model != null)
                    models.Add(model);
            }
            return models;            
        }          

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public Models.DynamicPage GetStartpage(Guid? siteId = null) {
            return GetStartpage<Models.DynamicPage>(siteId);
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public T GetStartpage<T>(Guid? siteId = null) where T : Models.GenericPage<T> {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var page = cache != null ? cache.Get<Page>($"Page_{siteId}") : null;

            if (page == null) {
                page = db.Pages
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.SiteId == siteId && p.ParentId == null && p.SortOrder == 0);
                if (page != null) {
                    if (cache != null)
                        AddToCache(page);
                }
            }

            if (page != null) {
                return Load<T, Models.PageBase>(page, api.PageTypes.GetById(page.PageTypeId));
            }
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public Models.DynamicPage GetById(Guid id) {
            return GetById<Models.DynamicPage>(id);
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public T GetById<T>(Guid id) where T : Models.GenericPage<T> {
            var page = cache != null ? cache.Get<Page>(id.ToString()) : null;

            if (page == null) {
                page = db.Pages
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.Id == id);

                if (page != null) {
                    if (cache != null)
                        AddToCache(page);
                }
            }

            if (page != null)
                return Load<T, Models.PageBase>(page, api.PageTypes.GetById(page.PageTypeId));
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public Models.DynamicPage GetBySlug(string slug, Guid? siteId = null) {
            return GetBySlug<Models.DynamicPage>(slug, siteId);
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public T GetBySlug<T>(string slug, Guid? siteId = null) where T : Models.GenericPage<T> {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            // See if we can get the page id for the slug from cache.
            var pageId = cache != null ? cache.Get<Guid?>($"PageId_{siteId}_{slug}") : (Guid?)null;

            if (pageId.HasValue) {
                // Load the page by id instead
                return GetById<T>(pageId.Value);
            } else {
                // No cache found, load from database
                var page = db.Pages
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.SiteId == siteId && p.Slug == slug);

                if (page != null) {
                    if (cache != null)
                        AddToCache(page);
                    return Load<T, Models.PageBase>(page, api.PageTypes.GetById(page.PageTypeId));
                }                    
                return null;
            }
        }

        /// <summary>
        /// Gets the id for the page with the given slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional page id</param>
        /// <returns>The id</returns>
        public Guid? GetIdBySlug(string slug, Guid? siteId = null) {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            // See if we can get the page id for the slug from cache.
            var pageId = cache != null ? cache.Get<Guid?>($"PageId_{siteId}_{slug}") : (Guid?)null;

            if (pageId.HasValue) {
                return pageId;
            } else {
                // No cache found, load from database
                var page = db.Pages
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.SiteId == siteId && p.Slug == slug);

                if (page != null) {
                    if (cache != null)
                        AddToCache(page);
                    return page.Id;
                }                    
                return null;                
            }
        }

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        public void Move<T>(T model, Guid? parentId, int sortOrder) where T : Models.GenericPage<T> {
            IEnumerable<Page> oldSiblings = null;
            IEnumerable<Page> newSiblings = null;

            // Only get siblings if we need to invalidate from cache
            if (cache != null) {
                oldSiblings = db.Pages
                    .Where(p => p.ParentId == model.ParentId && p.Id != model.Id)
                    .ToList();
                newSiblings = db.Pages
                    .Where(p => p.ParentId == parentId)
                    .ToList();
            }

            // Remove the old position for the page
            MovePages(model.Id, model.SiteId, model.ParentId, model.SortOrder + 1, false);
            // Add room for the new position of the page
            MovePages(model.Id, model.SiteId, parentId, sortOrder, true);

            // Update the position of the current page
            var page = db.Pages
                .FirstOrDefault(p => p.Id == model.Id);
            page.ParentId = parentId;
            page.SortOrder = sortOrder;

            db.SaveChanges();

            // Remove all siblings from cache
            if (cache != null) {
                foreach (var sibling in oldSiblings)
                    RemoveFromCache(sibling);
                foreach (var sibling in newSiblings)
                    RemoveFromCache(sibling);
                ((SiteRepository)api.Sites).InvalidateSitemap(model.SiteId);
            }
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        public void Save<T>(T model) where T : Models.GenericPage<T> {
            var type = api.PageTypes.GetById(model.TypeId);

            if (type != null) {
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                var page = db.Pages
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.Id == model.Id);

                // If not, create a new page
                if (page == null) {
                    page = new Page() {
                        Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                        ParentId = model.ParentId,
                        SortOrder = model.SortOrder,
                        PageTypeId = model.TypeId,
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    db.Pages.Add(page);
                    model.Id = page.Id;

                    // Make room for the new page
                    MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true);
                } else {
                    page.LastModified = DateTime.Now;

                    // Check if the page has been moved
                    if (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder) {
                        // Remove the old position for the page
                        MovePages(page.Id, model.SiteId, page.ParentId, page.SortOrder + 1, false);
                        // Add room for the new position of the page
                        MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true);
                    }
                }

                // Ensure that we have a slug
                if (string.IsNullOrWhiteSpace(model.Slug)) {
                    var prefix = "";

                    // Check if we should generate hierarchical slugs
                    using (var config = new Config(api)) {
                        if (config.HierarchicalPageSlugs && page.ParentId.HasValue) {
                            var parentSlug = db.Pages
                                .AsNoTracking()
                                .FirstOrDefault(p => p.Id == page.ParentId)?.Slug;

                            if (!string.IsNullOrWhiteSpace(parentSlug)) {
                                prefix = parentSlug + "/";
                            }
                        }
                    }
                    model.Slug = prefix + Utils.GenerateSlug(model.NavigationTitle != null ? model.NavigationTitle : model.Title);
                } else model.Slug = Utils.GenerateSlug(model.Slug);

                // Set content type
                model.ContentType = type.ContentTypeId;

                // Map basic fields
                App.Mapper.Map<Models.PageBase, Page>(model, page);

                // Map regions
                foreach (var regionKey in currentRegions) {
                    // Check that the region exists in the current model
                    if (HasRegion(model, regionKey)) {
                        var regionType = type.Regions.Single(r => r.Id == regionKey);

                        if (!regionType.Collection) {
                            MapRegion(model, page, GetRegion(model, regionKey), regionType, regionKey);
                        } else {
                            var items = new List<Guid>();
                            var sortOrder = 0;
                            foreach (var region in GetEnumerable(model, regionKey)) {
                                var fields = MapRegion(model, page, region, regionType, regionKey, sortOrder++);

                                if (fields.Count > 0)
                                    items.AddRange(fields);
                            }
                            // Now delete removed collection items
                            var removedFields = db.PageFields
                                .Where(f => f.PageId == model.Id && f.RegionId == regionKey && !items.Contains(f.Id))
                                .ToList();

                            if (removedFields.Count > 0)
                                db.PageFields.RemoveRange(removedFields);
                        }
                    }
                }

                db.SaveChanges();

                if (cache != null)
                    RemoveFromCache(page);
                ((SiteRepository)api.Sites).InvalidateSitemap(model.SiteId);                        
            }
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public virtual void Delete(Guid id) {
            var model = db.Pages
                .Include(p => p.Fields)
                .FirstOrDefault(p => p.Id == id);

            if (model != null) {
                db.Pages.Remove(model);

                // Move all remaining pages after this page in the site structure.
                MovePages(id, model.SiteId, model.ParentId, model.SortOrder + 1, false);

                db.SaveChanges();

                // Check if we have the page in cache, and if so remove it
                if (cache != null) {
                    var page = cache.Get<Page>(model.Id.ToString());
                    if (page != null)
                        RemoveFromCache(page);
                    ((SiteRepository)api.Sites).InvalidateSitemap(model.SiteId);
                }   
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public virtual void Delete<T>(T model) where T : Models.GenericPage<T> {
            Delete(model.Id);
        }

        #region Private helper methods
        /// <summary>
        /// Moves the pages around. This is done when a page is deleted or moved in the structure.
        /// </summary>
        /// <param name="pageId">The id of the page that is moved</param>
        /// <param name="siteId">The site id</param>
        /// <param name="parentId">The parent id</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="increase">If sort order should be increase or decreased</param>
        private void MovePages(Guid pageId, Guid siteId, Guid? parentId, int sortOrder, bool increase) {
            var pages = db.Pages
                .Where(p => p.SiteId == siteId && p.ParentId == parentId && p.SortOrder >= sortOrder && p.Id != pageId)
                .ToList();

            foreach (var page in pages)
                page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
        }
        #endregion

        /// <summary>
        /// Internal method for getting the data page by id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page</returns>
        internal Page GetPageById(Guid id) {
            var page = cache != null ? cache.Get<Page>(id.ToString()) : null;

            if (page == null) {
                page = db.Pages
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.Id == id);

                if (page != null) {
                    if (cache != null)
                        AddToCache(page);
                }
            }
            return page;
        }

        #region Private cache methods
        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="page">The page</param>
        private void AddToCache(Page page) {
            cache.Set(page.Id.ToString(), page);
            cache.Set($"PageId_{page.SiteId}_{page.Slug}", page.Id);
            if (!page.ParentId.HasValue && page.SortOrder == 0)
                cache.Set($"Page_{page.SiteId}", page);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="page">The page</param>
        private void RemoveFromCache(Page page) {
            cache.Remove(page.Id.ToString());
            cache.Remove($"PageId_{page.SiteId}_{page.Slug}");
            if (!page.ParentId.HasValue && page.SortOrder == 0)
                cache.Remove($"Page_{page.SiteId}");
        }
        #endregion
    }
}
