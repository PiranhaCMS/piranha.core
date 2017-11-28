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
    public class PageRepository : IPageRepository
    {
        #region Members
        private readonly Api api;
        private readonly IDb db;
        private readonly ICache cache;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db connection</param>
        /// <param name="modelCache">The optional model cache</param>
        public PageRepository(Api api, IDb db, ICache modelCache = null) {
            this.api = api;
            this.db = db;
            this.cache = modelCache;
        }

        /// <summary>
        /// Gets all of the available pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        public IEnumerable<Models.DynamicPage> GetAll(string siteId = null) {
            if (string.IsNullOrEmpty(siteId)) {
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
        /// Gets the site startpage.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public Models.DynamicPage GetStartpage(string siteId = null) {
            return GetStartpage<Models.DynamicPage>(siteId);
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public T GetStartpage<T>(string siteId = null) where T : Models.Page<T> {
            if (string.IsNullOrEmpty(siteId)) {
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

            if (page != null)
                return Load<T>(page);
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public Models.DynamicPage GetById(string id) {
            return GetById<Models.DynamicPage>(id);
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public T GetById<T>(string id) where T : Models.Page<T> {
            var page = cache != null ? cache.Get<Page>(id) : null;

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
                return Load<T>(page);
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public Models.DynamicPage GetBySlug(string slug, string siteId = null) {
            return GetBySlug<Models.DynamicPage>(slug, siteId);
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        public T GetBySlug<T>(string slug, string siteId = null) where T : Models.Page<T> {
            if (string.IsNullOrEmpty(siteId)) {
                var site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            // See if we can get the page id for the slug from cache.
            string pageId = cache != null ? cache.Get<string>($"PageId_{siteId}_{slug}") : null;

            if (!string.IsNullOrEmpty(pageId)) {
                // Load the page by id instead
                return GetById<T>(pageId);
            } else {
                // No cache found, load from database
                var page = db.Pages
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.SiteId == siteId && p.Slug == slug);

                if (page != null) {
                    return Load<T>(page);
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
        public void Move<T>(T model, string parentId, int sortOrder) where T : Models.Page<T> {
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
        public void Save<T>(T model) where T : Models.Page<T> {
            var type = api.PageTypes.GetById(model.TypeId);

            if (type != null) {
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                var page = db.Pages
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.Id == model.Id);

                // If not, create a new page
                if (page == null) {
                    page = new Page() {
                        Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Guid.NewGuid().ToString(),
                        ParentId = model.ParentId,
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
                        if (config.HierarchicalPageSlugs && !string.IsNullOrWhiteSpace(page.ParentId)) {
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
                            var sortOrder = 0;
                            foreach (var region in GetEnumerable(model, regionKey)) {
                                MapRegion(model, page, region, regionType, regionKey, sortOrder++);
                            }
                            // Now delete removed collection items
                            var removedFields = db.PageFields
                                .Where(f => f.PageId == model.Id && f.RegionId == regionKey && f.SortOrder == f.SortOrder)
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
        public virtual void Delete(string id) {
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
                    var page = cache.Get<Page>(model.Id);
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
        public virtual void Delete<T>(T model) where T : Models.Page<T> {
            Delete(model.Id);
        }

        #region Private get methods
        /// <summary>
        /// Loads the given data into a new model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="page">The data entity</param>
        /// <returns>The page model</returns>
        private T Load<T>(Page page) where T : Models.Page<T> {
            var type = api.PageTypes.GetById(page.PageTypeId);

            if (type != null) {
                // Create an initialized model
                var model = (T)typeof(T).GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { api, page.PageTypeId });
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                // Map basic fields
                App.Mapper.Map<Page, Models.PageBase>(page, model);

                // Map page type route (if available)
                if (string.IsNullOrWhiteSpace(model.Route) && type.Routes.Count > 0)
                    model.Route = type.Routes.First();

                // Map regions
                foreach (var regionKey in currentRegions) {
                    var region = type.Regions.Single(r => r.Id == regionKey);
                    var fields = page.Fields.Where(f => f.RegionId == regionKey).OrderBy(f => f.SortOrder).ToList();

                    if (!region.Collection) {
                        foreach (var fieldDef in region.Fields) {
                            var field = fields.SingleOrDefault(f => f.FieldId == fieldDef.Id && f.SortOrder == 0);

                            if (field != null) {
                                if (region.Fields.Count == 1) {
                                    SetSimpleValue(model, regionKey, field);
                                    break;
                                } else {
                                    SetComplexValue(model, regionKey, fieldDef.Id, field);
                                }
                            }
                        }
                    } else {
                        
                        var fieldCount = page.Fields.Count(f => f.RegionId == regionKey && f.FieldId == region.Fields[0].Id);
                        var sortOrder = 0;

                        do {
                            if (region.Fields.Count == 1) {
                                var field = fields.SingleOrDefault(f => f.FieldId == region.Fields[0].Id && f.SortOrder == sortOrder);
                                if (field != null)
                                    AddSimpleValue(model, regionKey, field);
                            } else {
                                AddComplexValue(model, regionKey, fields.Where(f => f.SortOrder == sortOrder).ToList());
                            }
                            sortOrder++;
                        } while (fieldCount > sortOrder);
                    }
                }
                return model;
            }
            return null;
        }

        /// <summary>
        /// Adds a simple single field value to a collection region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="field">The field</param>
        private void AddSimpleValue<T>(T model, string regionId, PageField field) where T : Models.Page<T> {
            if (model is Models.DynamicPage) {
                ((IList)((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions)[regionId]).Add(
                    DeserializeValue(field));
            } else {
                ((IList)model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model)).Add(
                    DeserializeValue(field));
            }
        }

        /// <summary>
        /// Sets the value of a simple single field region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="field">The field</param>
        private void SetSimpleValue<T>(T model, string regionId, PageField field) where T : Models.Page<T> {
            if (model is Models.DynamicPage) {
                ((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions)[regionId] =
                    DeserializeValue(field);
            } else {
                model.GetType().GetProperty(regionId, App.PropertyBindings).SetValue(model,
                    DeserializeValue(field));
            }
        }

        /// <summary>
        /// Adds a complex region to a collection region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="fields">The field</param>
        private void AddComplexValue<T>(T model, string regionId, IList<Data.PageField> fields) where T : Models.Page<T> {
            if (model is Models.DynamicPage) {
                var list = (IList)((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions)[regionId];
                var obj = Models.DynamicPage.CreateRegion(api, model.TypeId, regionId);

                foreach (var field in fields) {
                    if (((IDictionary<string, object>)obj).ContainsKey(field.FieldId)) {
                        ((IDictionary<string, object>)obj)[field.FieldId] =
                            DeserializeValue(field);
                    }
                }
                list.Add(obj);

            } else {
                var list = (IList)model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                var obj = Activator.CreateInstance(list.GetType().GenericTypeArguments.First());

                foreach (var field in fields) {
                    var prop = obj.GetType().GetProperty(field.FieldId, App.PropertyBindings);
                    if (prop != null) {
                        prop.SetValue(obj, DeserializeValue(field));
                    }
                }
                list.Add(obj);
            }
        }

        /// <summary>
        /// Sets the value of a complex region.
        /// </summary>
        /// <typeparam name="T">The model</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="fieldId">The field id</param>
        /// <param name="field">The field</param>
        private void SetComplexValue<T>(T model, string regionId, string fieldId, PageField field) where T : Models.Page<T> {
            if (model is Models.DynamicPage) {
                ((IDictionary<string, object>)((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions)[regionId])[fieldId] =
                    DeserializeValue(field);
            } else {
                var obj = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                if (obj != null)
                    obj.GetType().GetProperty(fieldId, App.PropertyBindings).SetValue(obj,
                        DeserializeValue(field));
            }
        }

        /// <summary>
        /// Deserializes the given field value.
        /// </summary>
        /// <param name="field">The page field</param>
        /// <returns>The value</returns>
        private object DeserializeValue(PageField field) {
            var type = App.Fields.GetByType(field.CLRType);

            if (type != null) {
                var val = (Extend.IField)App.DeserializeObject(field.Value, type.Type);
                if (val != null)
                    val.Init(api);
                return val;
            } 
            return null;
        }
        #endregion

        #region Private set methods
        /// <summary>
        /// Checks if the given model has a region with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>If the region exists</returns>
        private bool HasRegion<T>(T model, string regionId) where T : Models.Page<T> {
            if (model is Models.DynamicPage) {
                return ((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions).ContainsKey(regionId);
            } else {
                return model.GetType().GetProperty(regionId, App.PropertyBindings) != null;
            }
        }

        /// <summary>
        /// Gets the region with the given key.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The region</returns>
        private object GetRegion<T>(T model, string regionId) where T : Models.Page<T> {
            if (model is Models.DynamicPage) {
                return ((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions)[regionId];
            } else {
                return model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
            }
        }

        /// <summary>
        /// Gets a field value from a complex region.
        /// </summary>
        /// <param name="region">The region</param>
        /// <param name="fieldId">The field id</param>
        /// <returns>The value</returns>
        private object GetComplexValue(object region, string fieldId) {
            if (region is ExpandoObject) {
                return ((IDictionary<string, object>)region)[fieldId];
            } else {
                return region.GetType().GetProperty(fieldId, App.PropertyBindings).GetValue(region);
            }
        }

        /// <summary>
        /// Gets the enumerator for the given region collection.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The enumerator</returns>
        private IEnumerable GetEnumerable<T>(T model, string regionId) where T : Models.Page<T> {
            object value = null;

            if (model is Models.DynamicPage) {
                value = ((IDictionary<string, object>)((Models.DynamicPage)(object)model).Regions)[regionId];
            } else {
                value = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
            }
            if (value is IEnumerable)
                return (IEnumerable)value;
            return null;
        }

        /// <summary>
        /// Maps a region to the given data entity.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="page">The data entity</param>
        /// <param name="region">The region to map</param>
        /// <param name="regionType">The region type</param>
        /// <param name="regionId">The region id</param>
        /// <param name="sortOrder">The optional sort order</param>
        private void MapRegion<T>(T model, Page page, object region, Models.RegionType regionType, string regionId, int sortOrder = 0) where T : Models.Page<T> {
            // Now map all of the fields
            for (var n = 0; n < regionType.Fields.Count; n++) {
                var fieldDef = regionType.Fields[n];
                var fieldType = App.Fields.GetByShorthand(fieldDef.Type);
                if (fieldType == null)
                    fieldType = App.Fields.GetByType(fieldDef.Type);

                if (fieldType != null) {
                    object fieldValue = null;
                    if (regionType.Fields.Count == 1) {
                        // Get the field value for simple region
                        fieldValue = region;
                    } else {
                        // Get the field value for complex region
                        fieldValue = GetComplexValue(region, fieldDef.Id);
                    }

                    if (fieldValue != null) {
                        // Check that the returned value matches the type specified
                        // for the page type, otherwise deserialization won't work
                        // when the model is retrieved from the database.
                        if (fieldValue.GetType() != fieldType.Type)
                            throw new ArgumentException("Given page field value does not match the configured page type");

                        // Check if we have the current field in the database already
                        var field = page.Fields
                            .SingleOrDefault(f => f.RegionId == regionId && f.FieldId == fieldDef.Id && f.SortOrder == sortOrder);

                        // If not, create a new field
                        if (field == null) {
                            field = new PageField() {
                                Id = Guid.NewGuid().ToString(),
                                PageId = page.Id,
                                RegionId = regionId,
                                FieldId = fieldDef.Id
                            };
                            page.Fields.Add(field);
                        }

                        // Update field info & value
                        field.CLRType = fieldType.TypeName;
                        field.SortOrder = sortOrder;
                        field.Value = App.SerializeObject(fieldValue, fieldType.Type);
                    }
                }
            }
        }
        #endregion

        #region Private helper methods
        /// <summary>
        /// Moves the pages around. This is done when a page is deleted or moved in the structure.
        /// </summary>
        /// <param name="pageId">The id of the page that is moved</param>
        /// <param name="siteId">The site id</param>
        /// <param name="parentId">The parent id</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="increase">If sort order should be increase or decreased</param>
        private void MovePages(string pageId, string siteId, string parentId, int sortOrder, bool increase) {
            var pages = db.Pages
                .Where(p => p.SiteId == siteId && p.ParentId == parentId && p.SortOrder >= sortOrder && p.Id != pageId)
                .ToList();

            foreach (var page in pages)
                page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
        }
        #endregion

        #region Private cache methods
        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="page">The page</param>
        private void AddToCache(Page page) {
            cache.Set(page.Id, page);
            cache.Set($"PageId_{page.SiteId}_{page.Slug}", page.Id);
            if (string.IsNullOrEmpty(page.ParentId) && page.SortOrder == 0)
                cache.Set($"Page_{page.SiteId}", page);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="page">The page</param>
        private void RemoveFromCache(Page page) {
            cache.Remove(page.Id);
            cache.Remove($"PageId_{page.SiteId}_{page.Slug}");
            if (string.IsNullOrEmpty(page.ParentId) && page.SortOrder == 0)
                cache.Remove($"Page_{page.SiteId}");
        }
        #endregion
    }
}
