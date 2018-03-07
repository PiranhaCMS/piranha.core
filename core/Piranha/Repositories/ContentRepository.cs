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
using Piranha.Extend;
using Piranha.Models;
using ContentType = Piranha.Models.ContentType;

namespace Piranha.Repositories
{
    public abstract class ContentRepository<TContent, TField>
        where TContent : Content<TField>
        where TField : ContentField
    {
        //
        // Members
        protected readonly Api Api;
        protected readonly IDb Db;
        protected readonly ICache Cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db connection</param>
        /// <param name="modelCache">The optional model cache</param>
        protected ContentRepository(Api api, IDb db, ICache modelCache = null)
        {
            Api = api;
            Db = db;
            Cache = modelCache;
        }

        /// <summary>
        /// Loads the given data into a new model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="content">The content entity</param>
        /// <param name="type">The content type</param>
        /// <returns>The page model</returns>
        protected T Load<T, TModelBase>(TContent content, ContentType type, Action<TContent, T> process = null)
            where T : RoutedContent, TModelBase
            where TModelBase : RoutedContent
        {
            if (type == null)
            {
                return null;
            }

            // Create an initialized model
            var model = (T)typeof(T).GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { Api, type.Id });
            var currentRegions = type.Regions.Select(r => r.Id).ToArray();

            // Map basic fields
            App.Mapper.Map<TContent, TModelBase>(content, model);

            // Map page type route (if available)
            if (string.IsNullOrWhiteSpace(model.Route) && type.Routes.Count > 0)
                model.Route = type.Routes.First();

            // Map regions
            foreach (var regionKey in currentRegions)
            {
                var region = type.Regions.Single(r => r.Id == regionKey);
                var fields = content.Fields.Where(f => f.RegionId == regionKey).OrderBy(f => f.SortOrder).ToList();

                if (!region.Collection)
                {
                    foreach (var fieldDef in region.Fields)
                    {
                        var field = fields.SingleOrDefault(f => f.FieldId == fieldDef.Id && f.SortOrder == 0);

                        if (field != null)
                        {
                            if (region.Fields.Count == 1)
                            {
                                SetSimpleValue(model, regionKey, field);
                                break;
                            }
                            SetComplexValue(model, regionKey, fieldDef.Id, field);
                        }
                    }
                }
                else
                {
                    var fieldCount = content.Fields.Count(f => f.RegionId == regionKey && f.FieldId == region.Fields[0].Id);
                    var sortOrder = 0;

                    do
                    {
                        if (region.Fields.Count == 1)
                        {
                            var field = fields.SingleOrDefault(f => f.FieldId == region.Fields[0].Id && f.SortOrder == sortOrder);
                            if (field != null)
                                AddSimpleValue(model, regionKey, field);
                        }
                        else
                        {
                            AddComplexValue(model, regionKey, fields.Where(f => f.SortOrder == sortOrder).ToList());
                        }
                        sortOrder++;
                    } while (fieldCount > sortOrder);
                }
            }
            process?.Invoke(content, model);

            return model;
        }

        /// <summary>
        /// Sets the value of a simple single field region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="field">The field</param>
        protected void SetSimpleValue<T>(T model, string regionId, TField field) where T : RoutedContent
        {
            if (model is IDynamicModel dynamicModel)
            {
                ((IDictionary<string, object>)dynamicModel.Regions)[regionId] =
                    DeserializeValue(field);
            }
            else
            {
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
        private void AddSimpleValue<T>(T model, string regionId, TField field) where T : RoutedContent
        {
            if (model is IDynamicModel dynamicModel)
            {
                ((IList)((IDictionary<string, object>)dynamicModel.Regions)[regionId]).Add(
                    DeserializeValue(field));
            }
            else
            {
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
        protected void SetComplexValue<T>(T model, string regionId, string fieldId, TField field) where T : RoutedContent
        {
            if (model is IDynamicModel dynamicModel)
            {
                ((IDictionary<string, object>)((IDictionary<string, object>)dynamicModel.Regions)[regionId])[fieldId] =
                    DeserializeValue(field);
            }
            else
            {
                var obj = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                obj?.GetType().GetProperty(fieldId, App.PropertyBindings).SetValue(obj,
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
        private void AddComplexValue<T>(T model, string regionId, IList<TField> fields) where T : RoutedContent
        {
            if (fields.Count <= 0)
            {
                return;
            }

            if (model is IDynamicModel dynamicModel)
            {
                var list = (IList)((IDictionary<string, object>)dynamicModel.Regions)[regionId];
                var obj = dynamicModel.CreateRegion(Api, regionId);

                foreach (var field in fields)
                {
                    if (((IDictionary<string, object>)obj).ContainsKey(field.FieldId))
                    {
                        ((IDictionary<string, object>)obj)[field.FieldId] =
                            DeserializeValue(field);
                    }
                }
                list.Add(obj);

            }
            else
            {
                var list = (IList)model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                var obj = Activator.CreateInstance(list.GetType().GenericTypeArguments.First());

                foreach (var field in fields)
                {
                    var prop = obj.GetType().GetProperty(field.FieldId, App.PropertyBindings);
                    if (prop != null)
                    {
                        prop.SetValue(obj, DeserializeValue(field));
                    }
                }
                list.Add(obj);
            }
        }

        /// <summary>
        /// Deserializes the given field value.
        /// </summary>
        /// <param name="field">The page field</param>
        /// <returns>The value</returns>
        protected object DeserializeValue(TField field)
        {
            var type = App.Fields.GetByType(field.ClrType);

            if (type == null)
            {
                return null;
            }

            var val = (IField)App.DeserializeObject(field.Value, type.Type);
            val?.Init(Api);

            return val;
        }

        /// <summary>
        /// Checks if the given model has a region with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>If the region exists</returns>
        protected bool HasRegion<T>(T model, string regionId) where T : RoutedContent
        {
            if (model is IDynamicModel dynamicModel)
            {
                return ((IDictionary<string, object>)dynamicModel.Regions).ContainsKey(regionId);
            }

            return model.GetType().GetProperty(regionId, App.PropertyBindings) != null;
        }

        /// <summary>
        /// Gets the region with the given key.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The region</returns>
        protected object GetRegion<T>(T model, string regionId) where T : RoutedContent
        {
            if (model is IDynamicModel dynamicModel)
            {
                return ((IDictionary<string, object>)dynamicModel.Regions)[regionId];
            }

            return model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
        }

        /// <summary>
        /// Gets a field value from a complex region.
        /// </summary>
        /// <param name="region">The region</param>
        /// <param name="fieldId">The field id</param>
        /// <returns>The value</returns>
        protected object GetComplexValue(object region, string fieldId)
        {
            if (region is ExpandoObject o)
            {
                return ((IDictionary<string, object>)o)[fieldId];
            }

            return region.GetType().GetProperty(fieldId, App.PropertyBindings).GetValue(region);
        }

        /// <summary>
        /// Gets the enumerator for the given region collection.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The enumerator</returns>
        protected IEnumerable GetEnumerable<T>(T model, string regionId) where T : RoutedContent
        {
            object value;

            if (model is IDynamicModel dynamicModel)
            {
                value = ((IDictionary<string, object>)dynamicModel.Regions)[regionId];
            }
            else
            {
                value = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
            }
            if (value is IEnumerable enumerable)
            {
                return enumerable;
            }

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
        protected IList<Guid> MapRegion<T>(T model, TContent content, object region, RegionType regionType, string regionId, int sortOrder = 0) where T : RoutedContent
        {
            var items = new List<Guid>();

            // Now map all of the fields
            foreach (var fieldDef in regionType.Fields)
            {
                var fieldType = App.Fields.GetByShorthand(fieldDef.Type) ?? App.Fields.GetByType(fieldDef.Type);

                if (fieldType == null)
                {
                    continue;
                }

                var fieldValue = regionType.Fields.Count == 1 ? region : GetComplexValue(region, fieldDef.Id);

                if (fieldValue == null)
                {
                    continue;
                }

                // Check that the returned value matches the type specified
                // for the page type, otherwise deserialization won't work
                // when the model is retrieved from the database.
                if (fieldValue.GetType() != fieldType.Type)
                {
                    throw new ArgumentException("Given field value does not match the configured type");
                }

                // Check if we have the current field in the database already
                var def = fieldDef;
                var field = content.Fields
                    .SingleOrDefault(f => f.RegionId == regionId && f.FieldId == def.Id && f.SortOrder == sortOrder);

                // If not, create a new field
                if (field == null)
                {
                    field = Activator.CreateInstance<TField>();
                    field.Id = Guid.NewGuid();
                    field.RegionId = regionId;
                    field.FieldId = fieldDef.Id;

                    content.Fields.Add(field);
                }

                // Update field info & value
                field.ClrType = fieldType.TypeName;
                field.SortOrder = sortOrder;
                field.Value = App.SerializeObject(fieldValue, fieldType.Type);

                items.Add(field.Id);
            }
            return items;
        }
    }
}
