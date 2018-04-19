/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Piranha.Data;

namespace Piranha.Repositories
{
    public abstract class ContentRepository<TContent, TField> 
        where TContent : Content<TField> 
        where TField : ContentField
    {
        //
        // Members
        protected readonly Api api;
        protected readonly IDb db;
        protected readonly IServiceProvider services;
        protected readonly ICache cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db connection</param>
        /// <param name="modelCache">The optional model cache</param>
        protected ContentRepository(Api api, IDb db, IServiceProvider services, ICache modelCache = null) {
            this.api = api;
            this.db = db;
            this.services = services;
            this.cache = modelCache;
        }

        /// <summary>
        /// Loads the given data into a new model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="content">The content entity</param>
        /// <param name="type">The content type</param>
        /// <returns>The page model</returns>
        protected T Load<T, TModelBase>(TContent content, Models.ContentType type, Action<TContent, T> process = null) 
            where T : Models.Content, TModelBase
            where TModelBase : Models.Content
        {
            if (type != null) {
                var modelType = typeof(T);

                if (!typeof(Models.IDynamicModel).IsAssignableFrom(modelType)) {
                    modelType = Type.GetType(type.CLRType);

                    if (modelType != typeof(T) && !typeof(T).IsAssignableFrom(modelType))
                        return null;
                }

                // Create an initialized model
                var model = (T)modelType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { api, type.Id });
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                // Map basic fields
                App.Mapper.Map<TContent, TModelBase>(content, model);

                if (model is Models.RoutedContent) {
                    var routeModel = (Models.RoutedContent)(object)model;

                    // Map route (if available)
                    if (string.IsNullOrWhiteSpace(routeModel.Route) && type.Routes.Count > 0)
                        routeModel.Route = type.Routes.First();
                }

                // Map regions
                foreach (var regionKey in currentRegions) {
                    var region = type.Regions.Single(r => r.Id == regionKey);
                    var fields = content.Fields.Where(f => f.RegionId == regionKey).OrderBy(f => f.SortOrder).ToList();

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
                        var fieldCount = content.Fields.Where(f => f.RegionId == regionKey).Select(f => f.SortOrder).DefaultIfEmpty(-1).Max() + 1;
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
                process?.Invoke(content, model);

                return model;
            }
            return null;
        }

        /// <summary>
        /// Sets the value of a simple single field region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="field">The field</param>
        protected void SetSimpleValue<T>(T model, string regionId, TField field) where T : Models.Content {
            if (model is Models.IDynamicModel) {
                ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId] =
                    DeserializeValue(field);
            } else {
                model.GetType().GetProperty(regionId, App.PropertyBindings).SetValue(model,
                    DeserializeValue(field));
            }
        }

        /// <summary>
        /// Adds a simple single field value to a collection region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="field">The field</param>
        private void AddSimpleValue<T>(T model, string regionId, TField field) where T : Models.Content {
            if (model is Models.IDynamicModel) {
                ((IList)((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId]).Add(
                    DeserializeValue(field));
            } else {
                ((IList)model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model)).Add(
                    DeserializeValue(field));
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
        protected void SetComplexValue<T>(T model, string regionId, string fieldId, TField field) where T : Models.Content {
            if (model is Models.IDynamicModel) {
                ((IDictionary<string, object>)((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId])[fieldId] =
                    DeserializeValue(field);
            } else {
                var obj = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                if (obj != null)
                    obj.GetType().GetProperty(fieldId, App.PropertyBindings).SetValue(obj,
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
        private void AddComplexValue<T>(T model, string regionId, IList<TField> fields) where T : Models.Content {
            if (fields.Count > 0) {
                if (model is Models.IDynamicModel) {
                    var list = (IList)((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId];
                    var obj = ((Models.IDynamicModel)model).CreateRegion(api, regionId); 

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
        }

        /// <summary>
        /// Deserializes the given field value.
        /// </summary>
        /// <param name="field">The page field</param>
        /// <returns>The value</returns>
        protected object DeserializeValue(TField field) {
            var type = App.Fields.GetByType(field.CLRType);

            if (type != null) {
                var val = (Extend.IField)App.DeserializeObject(field.Value, type.Type);
                if (val != null) {
                    var init = val.GetType().GetMethod("Init");

                    if (init != null) {
                        var param = new List<object>();

                        foreach (var p in init.GetParameters()) {
                            if (p.ParameterType == typeof(IApi))
                                param.Add(api);
                            else param.Add(services.GetService(p.ParameterType));
                        }
                        init.Invoke(val, param.ToArray());
                    }
                    //val.Init(api);
                }
                return val;
            } 
            return null;
        }

        /// <summary>
        /// Checks if the given model has a region with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>If the region exists</returns>
        protected bool HasRegion<T>(T model, string regionId) where T : Models.Content {
            if (model is Models.IDynamicModel) {
                return ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions).ContainsKey(regionId);
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
        protected object GetRegion<T>(T model, string regionId) where T : Models.Content {
            if (model is Models.IDynamicModel) {
                return ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId];
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
        protected object GetComplexValue(object region, string fieldId) {
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
        protected IEnumerable GetEnumerable<T>(T model, string regionId) where T : Models.Content {
            object value = null;

            if (model is Models.IDynamicModel) {
                value = ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId];
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
        /// <param name="content">The content entity</param>
        /// <param name="region">The region to map</param>
        /// <param name="regionType">The region type</param>
        /// <param name="regionId">The region id</param>
        /// <param name="sortOrder">The optional sort order</param>
        protected IList<Guid> MapRegion<T>(T model, TContent content, object region, Models.RegionType regionType, string regionId, int sortOrder = 0) where T : Models.Content {
            var items = new List<Guid>();

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
                            throw new ArgumentException("Given field value does not match the configured type");

                        // Check if we have the current field in the database already
                        var field = content.Fields
                            .SingleOrDefault(f => f.RegionId == regionId && f.FieldId == fieldDef.Id && f.SortOrder == sortOrder);

                        // If not, create a new field
                        if (field == null) {
                            field = Activator.CreateInstance<TField>();
                            field.Id = Guid.NewGuid();
                            field.RegionId = regionId;
                            field.FieldId = fieldDef.Id;

                            content.Fields.Add(field);
                        }

                        // Update field info & value
                        field.CLRType = fieldType.TypeName;
                        field.SortOrder = sortOrder;
                        field.Value = App.SerializeObject(fieldValue, fieldType.Type);

                        items.Add(field.Id);
                    }
                }
            }
            return items;
        }        
    }
}
