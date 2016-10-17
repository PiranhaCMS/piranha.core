using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    public class PageRepository : RepositoryBase<Data.Page, Models.PageModel>, IPageRepository
    {
        #region Members
        /// <summary>
        /// The current Api.
        /// </summary>
        private readonly Api api;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        internal PageRepository(Api api, Db db) : base(db) {
            this.api = api;
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <returns>The page model</returns>
        public Models.PageModel GetStartpage() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The page model</returns>
        public T GetStartpage<T>() where T : Models.PageModel<T> {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public T GetById<T>(Guid id) where T : Models.PageModel<T> {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The page model</returns>
        public Models.PageModel GetBySlug(string slug) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <returns>The page model</returns>
        public T GetBySlug<T>(string slug) where T : Models.PageModel<T> {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all page models with the specified parent id.
        /// </summary>
        /// <param name="parentId">The parent id</param>
        /// <returns>The page models</returns>
        public IList<Models.PageModel> GetByParentId(Guid? parentId) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        public void Save<TModelType>(Models.PageModel<TModelType> model) where TModelType : Models.PageModel<TModelType> {
            var type = api.PageTypes.GetById(model.PageTypeId);

            if (type != null) {
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                // Check if we have the page in the database already
                var page = db.Pages
                    .Include(p => p.Fields)
                    .SingleOrDefault(p => p.Id == model.Id);

                // If not, create a new page
                if (page == null) {
                    page = new Data.Page() {
                        Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                        PageTypeId = model.PageTypeId
                    };
                    db.Pages.Add(page);
                    model.Id = page.Id;
                }

                // Map basic fields
                page.Title = model.Title;
                page.NavigationTitle = model.NavigationTitle;
                page.Slug = model.Slug;
                page.ParentId = model.ParentId;
                page.SortOrder = model.SortOrder;
                page.MetaKeywords = model.MetaKeywords;
                page.MetaDescription = model.MetaDescription;
                page.Route = model.Route;
                page.Published = model.Published;

                // Map regions
                foreach (var regionKey in currentRegions) {
                    // Check that the region exists in the current model
                    if (HasRegion(model, regionKey)) { 
                        var regionType = type.Regions.Single(r => r.Id == regionKey);

                        if (!regionType.Collection) {
                            /*
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
                                        fieldValue = GetSimpleValue(model, regionKey);
                                    } else {
                                        // Get the field value for complex region
                                        fieldValue = GetComplexValue(model, regionKey, fieldDef.Id);
                                    }

                                    // Check that the returned value matches the type specified
                                    // for the page type, otherwise deserialization won't work
                                    // when the model is retrieved from the database.
                                    if (fieldValue.GetType() != fieldType.Type)
                                        throw new ArgumentException("Given page field value does not match the configured page type");

                                    // Check if we have the current field in the database already
                                    var field = page.Fields
                                        .SingleOrDefault(f => f.RegionId == regionKey && f.FieldId == fieldDef.Id);

                                    // If not, create a new field
                                    if (field == null) {
                                        field = new Data.PageField() {
                                            PageId = page.Id,
                                            RegionId = regionKey,
                                            FieldId = fieldDef.Id
                                        };
                                        db.PageFields.Add(field);
                                    }

                                    // Update field info & value
                                    field.CLRType = fieldType.CLRType;
                                    field.SortOrder = 0;
                                    field.Value = JsonConvert.SerializeObject(fieldValue);
                                }
                            }
                            */
                            MapRegion(model, page, regionType, regionKey);
                        } else {
                            // Remove all old fields for the current region
                            var oldFields = page.Fields.Where(f => f.RegionId == regionKey).ToArray();
                            if (oldFields.Length > 0)
                                db.PageFields.RemoveRange(oldFields);

                            var sortOrder = 0;
                            foreach (var region in GetEnumerable(model, regionKey)) {
                                //MapRegion(model, page, regionType, regionKey, sortOrder++);
                            }
                        }
                    }
                }
                // Save everything for the model
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the base entity query.
        /// </summary>
        /// <returns>The query</returns>
        protected override IQueryable<Data.Page> Query() {
            return db.Pages
                .Include(p => p.Fields);
        }

        #region Private methods
        /// <summary>
        /// Checks if the given model has a region with the given id.
        /// </summary>
        /// <typeparam name="TModelType">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>If the region exists</returns>
        private bool HasRegion<TModelType>(Models.PageModel<TModelType> model, string regionId) where TModelType : Models.PageModel<TModelType> {
            if (model is Models.PageModel) {
                return ((IDictionary<string, object>)((Models.PageModel)(object)model).Regions).ContainsKey(regionId);
            } else {
                return model.GetType().GetProperty(regionId, App.PropertyBindings) != null;
            }
        }

        /// <summary>
        /// Gets a simple value from a one field region.
        /// </summary>
        /// <typeparam name="TModelType">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The value</returns>
        private object GetSimpleValue<TModelType>(Models.PageModel<TModelType> model, string regionId) where TModelType : Models.PageModel<TModelType> {
            if (model is Models.PageModel) {
                return ((IDictionary<string, object>)((Models.PageModel)(object)model).Regions)[regionId];
            } else {
                return model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
            }
        }

        /// <summary>
        /// Gets a field value from a complex region.
        /// </summary>
        /// <typeparam name="TModelType">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="fieldId">The field id</param>
        /// <returns>The value</returns>
        private object GetComplexValue<TModelType>(Models.PageModel<TModelType> model, string regionId, string fieldId) where TModelType : Models.PageModel<TModelType> {
            if (model is Models.PageModel) {
                return ((IDictionary<string, object>)((IDictionary<string, object>)((Models.PageModel)(object)model).Regions)[regionId])[fieldId];
            } else {
                var obj = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                if (obj != null)
                    return obj.GetType().GetProperty(fieldId, App.PropertyBindings).GetValue(obj);
            }
            return null;
        }

        /// <summary>
        /// Gets the enumerator for the given region collection.
        /// </summary>
        /// <typeparam name="TModelType">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The enumerator</returns>
        private IEnumerable GetEnumerable<TModelType>(Models.PageModel<TModelType> model, string regionId) where TModelType : Models.PageModel<TModelType> {
            object value = null;

            if (model is Models.PageModel) {
                value = ((IDictionary<string, object>)((Models.PageModel)(object)model).Regions)[regionId];
            } else {
                value = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
            }
            if (value is IEnumerable)
                return (IEnumerable)value;
            return null;
        }

        private void MapRegion<TModelType>(Models.PageModel<TModelType> model, Data.Page page, Models.PageTypeRegion regionType, string regionId, int sortOrder = 0) where TModelType : Models.PageModel<TModelType> {
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
                        fieldValue = GetSimpleValue(model, regionId);
                    } else {
                        // Get the field value for complex region
                        fieldValue = GetComplexValue(model, regionId, fieldDef.Id);
                    }

                    // Check that the returned value matches the type specified
                    // for the page type, otherwise deserialization won't work
                    // when the model is retrieved from the database.
                    if (fieldValue.GetType() != fieldType.Type)
                        throw new ArgumentException("Given page field value does not match the configured page type");

                    // Check if we have the current field in the database already
                    var field = page.Fields
                        .SingleOrDefault(f => f.RegionId == regionId && f.FieldId == fieldDef.Id);

                    // If not, create a new field
                    if (field == null) {
                        field = new Data.PageField() {
                            PageId = page.Id,
                            RegionId = regionId,
                            FieldId = fieldDef.Id
                        };
                        db.PageFields.Add(field);
                    }

                    // Update field info & value
                    field.CLRType = fieldType.CLRType;
                    field.SortOrder = sortOrder;
                    field.Value = JsonConvert.SerializeObject(fieldValue);
                }
            }
        }
        #endregion
    }
}
