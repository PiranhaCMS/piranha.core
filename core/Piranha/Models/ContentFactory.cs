/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Piranha.Extend;

namespace Piranha.Models
{
    /// <summary>
    /// Factory for creating templated content.
    /// </summary>
    internal class ContentFactory : IDisposable
    {
        #region Members
        /// <summary>
        /// The current content types.
        /// </summary>
        private readonly IEnumerable<ContentType> types;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="types"></param>
        public ContentFactory(IEnumerable<ContentType> types)
        {
            this.types = types;
        }

        /// <summary>
        /// Creates and initializes a new content model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="typeId">The content type id</param>
        /// <returns>The model</returns>
        public T Create<T>(string typeId) where T : Content
        {
            var contentType = types
                .SingleOrDefault(t => t.Id == typeId);

            if (contentType == null)
            {
                return null;
            }

            var model = Activator.CreateInstance<T>();
            model.TypeId = typeId;

            if (model is IDynamicModel dynModel)
            {
                foreach (var region in contentType.Regions)
                {
                    object value = null;

                    if (region.Collection)
                    {
                        var reg = CreateDynamicRegion(region);

                        if (reg != null)
                        {
                            value = Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(reg.GetType()));
                            ((IRegionList)value).Model = dynModel;
                            ((IRegionList)value).TypeId = typeId;
                            ((IRegionList)value).RegionId = region.Id;
                        }
                    }
                    else
                    {
                        value = CreateDynamicRegion(region);
                    }

                    if (value != null)
                        ((IDictionary<string, object>)dynModel.Regions).Add(region.Id, value);
                }
            }
            else
            {
                var type = model.GetType();

                foreach (var region in contentType.Regions)
                {
                    if (!region.Collection)
                    {
                        var prop = type.GetProperty(region.Id, App.PropertyBindings);
                        if (prop != null)
                        {
                            prop.SetValue(model, CreateRegion(prop.PropertyType, region));
                        }
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The content type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public object CreateDynamicRegion(string typeId, string regionId)
        {
            var contentType = types
                .SingleOrDefault(t => t.Id == typeId);

            var region = contentType?.Regions.SingleOrDefault(r => r.Id == regionId);

            return region != null ? CreateDynamicRegion(region) : null;
        }

        /// <summary>
        /// Creates a dynamic region.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="typeId">The content type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The region value</returns>
        public TValue CreateRegion<TValue>(string typeId, string regionId)
        {
            var contentType = types
                .SingleOrDefault(t => t.Id == typeId);

            var region = contentType?.Regions.SingleOrDefault(r => r.Id == regionId);

            if (region != null)
            {
                return (TValue)CreateRegion(typeof(TValue), region);
            }

            return default(TValue);
        }

        /// <summary>
        /// Disposes the factory.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Private methods
        /// <summary>
        /// Creates a dynamic region value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <returns>The created value</returns>
        private object CreateDynamicRegion(RegionType region)
        {
            if (region.Fields.Count == 1)
            {
                var type = App.Fields.GetByShorthand(region.Fields[0].Type) ?? App.Fields.GetByType(region.Fields[0].Type);

                if (type != null)
                {
                    return Activator.CreateInstance(type.Type);
                }
            }
            else
            {
                var reg = new ExpandoObject();

                foreach (var field in region.Fields)
                {
                    var type = GetFieldType(field);

                    if (type != null)
                        ((IDictionary<string, object>)reg).Add(field.Id, Activator.CreateInstance(type.Type));
                }

                return reg;
            }

            return null;
        }

        /// <summary>
        /// Creates a region value.
        /// </summary>
        /// <param name="regionType">The region type</param>
        /// <param name="region">The region</param>
        /// <returns>The created value</returns>
        private object CreateRegion(Type regionType, RegionType region)
        {
            if (region.Fields.Count == 1)
            {
                return CreateField(region.Fields[0], regionType);
            }
            var reg = Activator.CreateInstance(regionType);
            var type = reg.GetType();

            foreach (var field in region.Fields)
            {
                var fieldType = GetFieldType(field);

                if (type == null)
                {
                    continue;
                }

                var prop = type.GetProperty(field.Id, App.PropertyBindings);

                if (prop != null && fieldType.Type == prop.PropertyType)
                {
                    prop.SetValue(reg, Activator.CreateInstance(fieldType.Type));
                }
            }

            return reg;
        }

        /// <summary>
        /// Gets the registered field type.
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>The type, null if not found</returns>
        private AppField GetFieldType(FieldType field)
        {
            var type = App.Fields.GetByShorthand(field.Type) ?? App.Fields.GetByType(field.Type);

            return type;
        }

        /// <summary>
        /// Creates a field of the given type.
        /// </summary>
        /// <param name="field">The field type</param>
        /// <param name="expectedType">The expected type</param>
        /// <returns></returns>
        private object CreateField(FieldType field, Type expectedType = null)
        {
            var type = GetFieldType(field);

            if (type != null && (expectedType == null || type.Type == expectedType))
            {
                return Activator.CreateInstance(type.Type);
            }

            return null;
        }
        #endregion
    }
}
