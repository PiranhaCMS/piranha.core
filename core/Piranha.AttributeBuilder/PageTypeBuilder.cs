﻿/*
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
using System.Linq;
using System.Reflection;
using Piranha.Models;

namespace Piranha.AttributeBuilder
{
    public class PageTypeBuilder : ContentTypeBuilder<PageTypeBuilder, PageType>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>        
        public PageTypeBuilder(IApi api) : base(api) { }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public override PageTypeBuilder Build()
        {
            foreach (var type in types)
            {
                var pageType = GetContentType(type);

                if (pageType != null)
                    api.PageTypes.Save(pageType);
            }
            return this;
        }

        /// <summary>
        /// Deletes all page types in the database that doesn't
        ///  exist in the database,
        /// </summary>
        /// <returns>The builder</returns>
        public PageTypeBuilder DeleteOrphans()
        {
            var orphans = new List<PageType>();
            var importTypes = new List<PageType>();

            // Get all page types added for import.
            foreach (var type in types)
            {
                var importType = GetContentType(type);

                if (importType != null)
                    importTypes.Add(importType);
            }

            // Get all previously imported page types.
            foreach (var pageType in api.PageTypes.GetAll())
            {
                if (importTypes.All(t => t.Id != pageType.Id))
                {
                    orphans.Add(pageType);
                }
            }

            // Delete all orphans.
            foreach (var pageType in orphans)
            {
                api.PageTypes.Delete(pageType);
            }
            return this;
        }

        #region Private methods
        /// <summary>
        /// Gets the possible page type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The page type</returns>
        protected override PageType GetContentType(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttribute<PageTypeAttribute>();

            if (attr != null)
            {
                if (string.IsNullOrWhiteSpace(attr.Id))
                    attr.Id = type.Name;

                if (string.IsNullOrEmpty(attr.Id) || string.IsNullOrEmpty(attr.Title))
                {
                    return null;
                }

                var pageType = new PageType
                {
                    Id = attr.Id,
                    Title = attr.Title,
                    ContentTypeId = App.ContentTypes.GetId(type)
                };

                var routes = type.GetTypeInfo().GetCustomAttributes(typeof(PageTypeRouteAttribute));
                foreach (var attribute in routes)
                {
                    var route = (PageTypeRouteAttribute) attribute;
                    if (!string.IsNullOrWhiteSpace(route.Title) && !string.IsNullOrWhiteSpace(route.Route))
                        pageType.Routes.Add(new ContentTypeRoute
                        {
                            Title = route.Title,
                            Route = route.Route
                        });
                }

                var regionTypes = new List<Tuple<int?, RegionType>>();

                foreach (var prop in type.GetProperties(App.PropertyBindings))
                {
                    var regionType = GetRegionType(prop);

                    if (regionType != null)
                    {
                        regionTypes.Add(regionType);
                    }
                }
                regionTypes = regionTypes.OrderBy(t => t.Item1).ToList();

                // First add sorted regions
                foreach (var regionType in regionTypes.Where(t => t.Item1.HasValue))
                    pageType.Regions.Add(regionType.Item2);
                // Then add the unsorted regions
                foreach (var regionType in regionTypes.Where(t => !t.Item1.HasValue))
                    pageType.Regions.Add(regionType.Item2);

                return pageType;
            }

            throw new ArgumentException($"Title is mandatory in PageTypeAttribute. No title provided for {type.Name}");
        }
        #endregion
    }
}
