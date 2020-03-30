/*
 * Copyright (c) .NET Foundation and Contributors
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
using System.Threading.Tasks;

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Class for simple access to the different content type
    /// builders available.
    /// </summary>
    public class ContentTypeBuilder
    {
        private readonly PageTypeBuilder _pageTypes;
        private readonly PostTypeBuilder _postTypes;
        private readonly SiteTypeBuilder _siteTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ContentTypeBuilder(IApi api)
        {
            _pageTypes = new PageTypeBuilder(api);
            _postTypes = new PostTypeBuilder(api);
            _siteTypes = new SiteTypeBuilder(api);
        }

        /// <summary>
        /// Adds all content types available in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The builder</returns>
        public ContentTypeBuilder AddAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    var pageAttr = type.GetCustomAttribute<PageTypeAttribute>();
                    if (pageAttr != null)
                    {
                        _pageTypes.AddType(type);
                        continue;
                    }

                    var postAttr = type.GetCustomAttribute<PostTypeAttribute>();
                    if (postAttr != null)
                    {
                        _postTypes.AddType(type);
                        continue;
                    }

                    var siteAttr = type.GetCustomAttribute<SiteTypeAttribute>();
                    if (siteAttr != null)
                    {
                        _siteTypes.AddType(type);
                        continue;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Builds all of the importer content types and saves the to the
        /// database.
        /// </summary>
        /// <returns>The builder</returns>
        public ContentTypeBuilder Build()
        {
            _pageTypes.Build();
            _postTypes.Build();
            _siteTypes.Build();

            return this;
        }

        /// <summary>
        /// Deletes all content types that does not currently exist in
        /// the builder.
        /// </summary>
        /// <returns>The builder</returns>
        public ContentTypeBuilder DeleteOrphans()
        {
            _pageTypes.DeleteOrphans();
            _postTypes.DeleteOrphans();
            _siteTypes.DeleteOrphans();

            return this;
        }
    }

    /// <summary>
    /// Abstract base class for importing a content type from attributes.
    /// </summary>
    /// <typeparam name="T">The builder type</typeparam>
    /// <typeparam name="TType">The content type</typeparam>
    public abstract class ContentTypeBuilder<T, TType>
        where T : ContentTypeBuilder<T, TType>
        where TType : ContentTypeBase
    {
        /// <summary>
        /// The currently imported types.
        /// </summary>
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
        public virtual T Build()
        {
            return BuildAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public abstract Task<T> BuildAsync();

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
                    ListExpand = attr.ListExpand,
                    Icon = attr.Icon,
                    Display = attr.Display
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
                    {
                        return null;
                    }
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
                    var fieldType = new FieldType
                    {
                        Id = prop.Name,
                        Title = attr.Title,
                        Type = appFieldType.TypeName,
                        Options = attr.Options,
                        Placeholder = attr.Placeholder
                    };

                    // Get optional description
                    var descAttr = prop.GetCustomAttribute<FieldDescriptionAttribute>();
                    if (descAttr != null)
                    {
                        fieldType.Description = descAttr.Text;
                    }
                    return fieldType;
                }
            }
            return null;
        }

        private void RegisterField(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                type = type.GenericTypeArguments.First();
            }

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
