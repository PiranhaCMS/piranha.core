/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend;
using Piranha.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Piranha.AttributeBuilder
{
    public abstract class ContentTypeBuilder<T, TType> where T : ContentTypeBuilder<T, TType> where TType : ContentType
    {
        protected readonly List<Type> _types = new List<Type>();

        /// <summary>
        /// Adds a new type to build page types from
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The builder</returns>
        public T AddType(Type type)
        {
            _types.Add(type);

            return (T)this;
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public abstract T Build();

        /// <summary>
        /// Gets the possible content type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The content type</returns>
        protected abstract TType GetContentType(Type type);

        /// <summary>
        /// Gets the possible region type for the given property.
        /// </summary>
        /// <param name="prop">The property info</param>
        /// <returns>The region type</returns>
        protected Tuple<int?, RegionType> GetRegionType(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<RegionAttribute>();

            if (attr != null)
            {
                var isCollection = typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);

                var regionType = new RegionType
                {
                    Id = prop.Name,
                    Title = attr.Title,
                    Collection = isCollection,
                    ListTitleField = attr.ListTitle,
                    ListTitlePlaceholder = attr.ListPlaceholder,
                    ListExpand = attr.ListExpand
                };
                int? sortOrder = attr.SortOrder != Int32.MaxValue ? attr.SortOrder : (int?)null;

                // Get optional description
                var descAttr = prop.GetCustomAttribute<RegionDescriptionAttribute>();
                if (descAttr != null)
                {
                    regionType.Description = descAttr.Text;
                }                

                Type type = null;

                if (!isCollection)
                {
                    type = prop.PropertyType;
                }
                else
                {
                    type = prop.PropertyType.GenericTypeArguments.First();
                }

                if (typeof(IField).IsAssignableFrom(type))
                {
                    var appFieldType = App.Fields.GetByType(type);

                    if (appFieldType == null)
                    {
                        RegisterField(type);
                        appFieldType = App.Fields.GetByType(type);

                        // This is a single field region, but the type is missing.
                        // Discard the entire region
                        if (appFieldType == null)
                            return null;
                    }

                    regionType.Fields.Add(new FieldType
                    {
                        Id = "Default",
                        Type = appFieldType.TypeName
                    });
                }
                else
                {
                    foreach (var fieldProp in type.GetProperties(App.PropertyBindings))
                    {
                        var fieldType = GetFieldType(fieldProp);

                        if (fieldType != null)
                            regionType.Fields.Add(fieldType);
                    }
                    // Skip regions without fields.
                    if (regionType.Fields.Count == 0)
                        return null;
                }
                return new Tuple<int?, RegionType>(sortOrder, regionType);
            }
            return null;
        }

        /// <summary>
        /// Gets the possible field type for the given property.
        /// </summary>
        /// <param name="prop">The property</param>
        /// <returns>The field type</returns>
        protected FieldType GetFieldType(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<FieldAttribute>();

            if (attr != null)
            {
                var appFieldType = App.Fields.GetByType(prop.PropertyType);

                // Missing field type, check if we can register it on the fly
                if (appFieldType == null)
                {
                    RegisterField(prop.PropertyType);
                    appFieldType = App.Fields.GetByType(prop.PropertyType);
                }

                if (appFieldType != null)
                {
                    return new FieldType
                    {
                        Id = prop.Name,
                        Title = attr.Title,
                        Type = appFieldType.TypeName,
                        Options = attr.Options,
                        Placeholder = attr.Placeholder
                    };
                }
            }
            return null;
        }

        private void RegisterField(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
                type = type.GenericTypeArguments.First();

            if (typeof(IField).IsAssignableFrom(type))
            {
                if (type.GetCustomAttribute<FieldTypeAttribute>() != null)
                {
                    MethodInfo generic = null;

                    if (typeof(Extend.Fields.SelectFieldBase).IsAssignableFrom(type))
                    {
                        var method = typeof(Runtime.AppFieldList).GetMethod("RegisterSelect");
                        generic = method.MakeGenericMethod(type.GenericTypeArguments.First());
                    }
                    else
                    {
                        var method = typeof(Runtime.AppFieldList).GetMethod("Register");
                        generic = method.MakeGenericMethod(type);
                    }

                    generic.Invoke(App.Fields, null);
                }
            }
        }
    }
}
