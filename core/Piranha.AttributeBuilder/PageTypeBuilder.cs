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
        public PageTypeBuilder(Api api) : base(api) { }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public override void Build() {
            foreach (var type in types) {
                var pageType = GetContentType(type);

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
        protected override PageType GetContentType(Type type) {
            var attr = type.GetTypeInfo().GetCustomAttribute<PageTypeAttribute>();

            if (attr != null) {
                if (string.IsNullOrWhiteSpace(attr.Id))
                    attr.Id = type.Name;

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title)) {
                    var pageType = new PageType() {
                        Id = attr.Id,
                        Title = attr.Title,
                        Route = attr.Route
                    };

                    foreach (var prop in type.GetProperties(App.PropertyBindings)) {
                        var regionType = GetRegionType(prop);

                        if (regionType != null)
                            pageType.Regions.Add(regionType);
                    }
                    return pageType;
                }
            } else {
                throw new ArgumentException($"Title is mandatory in PageTypeAttribute. No title provided for {type.Name}");
            }
            return null;
        }
        #endregion
    }
}
