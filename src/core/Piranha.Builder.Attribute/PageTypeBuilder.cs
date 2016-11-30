/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Piranha.Extend;

namespace Piranha.Builder.Attribute
{
    public class PageTypeBuilder
    {
        #region Members
        private readonly List<Type> types = new List<Type>();
        private readonly IApi api;
        private readonly ILogger logger;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="logFactory">The optional log factory</param>
        public PageTypeBuilder(IApi api, ILoggerFactory logFactory = null) {
            this.api = api;
            this.logger = logFactory?.CreateLogger("Piranha.Builder.Attribute.PageTypeBuilder");
        }

        /// <summary>
        /// Adds a new type to build page types from
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The builder</returns>
        public PageTypeBuilder AddType(Type type) {
            types.Add(type);

            return this;
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public void Build() {
            foreach (var type in types) {
                var pageType = GetPageType(type);

                if (pageType != null)
                    api.PageTypes.Save(pageType);
            }
        }

        #region Private methods
        /// <summary>
        /// Gets the possible page type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The page type</returns>
        private PageType GetPageType(Type type) {
            var attr = type.GetTypeInfo().GetCustomAttribute<PageTypeAttribute>();

            if (attr != null) {
                logger?.LogInformation($"Importing PageType '{type.Name}'.");

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title)) {
                    var pageType = new PageType() {
                        Id = attr.Id,
                        Title = attr.Title,
                        Route = attr.Route,
                        View = attr.View
                    };

                    foreach (var prop in type.GetTypeInfo().GetProperties(App.PropertyBindings)) {
                        var regionType = GetRegionType(prop);

                        if (regionType != null)
                            pageType.Regions.Add(regionType);
                    }
                    return pageType;
                } else {
                    logger?.LogError($"Id and/or Title is missing for PageType '{type.Name}'.");
                }
            } 
            return null;
        }

        /// <summary>
        /// Gets the possible region type for the given property.
        /// </summary>
        /// <param name="prop">The property info</param>
        /// <returns>The region type</returns>
        private RegionType GetRegionType(PropertyInfo prop) {
            var attr = prop.GetCustomAttribute<RegionAttribute>();

            if (attr != null) {
                var isCollection = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(prop.PropertyType);

                var regionType = new RegionType() {
                    Id = prop.Name,
                    Title = attr.Title,
                    Collection = isCollection,
                    Max = attr.Max,
                    Min = attr.Min
                };

                Type type = null;

                if (!isCollection) {
                    type = prop.PropertyType;
                } else {
                    type = prop.PropertyType.GenericTypeArguments.First();
                }

                if (typeof(IField).GetTypeInfo().IsAssignableFrom(type)) {
                    var appFieldType = App.Fields.GetByType(type);

                    if (appFieldType == null) {
                        // This is a single field region, but the type is missing.
                        // Discard the entire region
                        logger?.LogError($"Missing field type [{type.Name}] for region '{regionType.Id}'.");
                        return null;
                    }

                    regionType.Fields.Add(new FieldType() {
                        Id = "Default",
                        Type = appFieldType.CLRType
                    });
                } else {
                    foreach (var fieldProp in type.GetTypeInfo().GetProperties(App.PropertyBindings)) {
                        var fieldType = GetFieldType(fieldProp);

                        if (fieldType != null)
                            regionType.Fields.Add(fieldType);
                    }
                    // Skip regions without fields.
                    if (regionType.Fields.Count == 0)
                        return null;
                }
                return regionType;
            }
            return null;
        }

        /// <summary>
        /// Gets the possible field type for the given property.
        /// </summary>
        /// <param name="prop">The property</param>
        /// <returns>The field type</returns>
        private FieldType GetFieldType(PropertyInfo prop) {
            var attr = prop.GetCustomAttribute<FieldAttribute>();

            if (attr != null) {
                var appFieldType = App.Fields.GetByType(prop.PropertyType);

                if (appFieldType != null) {
                    return new FieldType() {
                        Id = prop.Name,
                        Title = attr.Title,
                        Type = appFieldType.CLRType
                    };
                } else {
                    logger?.LogError($"Missing field type [{prop.PropertyType.Name}] for field '{prop.Name}'.");
                }
            }
            return null;
        }
        #endregion
    }
}
