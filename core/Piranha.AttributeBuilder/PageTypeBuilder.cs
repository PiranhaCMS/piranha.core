/*
 * Copyright (c) 2016-2019 Håkan Edling
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
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.AttributeBuilder
{
    public class PageTypeBuilder : ContentTypeBuilder<PageTypeBuilder, PageType>
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageTypeBuilder(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public override async Task<PageTypeBuilder> BuildAsync()
        {
            foreach (var type in _types)
            {
                var pageType = GetContentType(type);

                if (pageType != null)
                {
                    pageType.Ensure();
                    await _api.PageTypes.SaveAsync(pageType);
                }
            }
            return this;
        }

        /// <summary>
        /// Deletes all page types in the database that doesn't
        /// exist in the database,
        /// </summary>
        /// <returns>The builder</returns>
        public PageTypeBuilder DeleteOrphans()
        {
            return DeleteOrphansAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes all page types in the database that doesn't
        /// exist in the database,
        /// </summary>
        /// <returns>The builder</returns>
        public async Task<PageTypeBuilder> DeleteOrphansAsync()
        {
            var orphans = new List<PageType>();
            var importTypes = new List<PageType>();

            // Get all page types added for import.
            foreach (var type in _types)
            {
                var importType = GetContentType(type);

                if (importType != null)
                    importTypes.Add(importType);
            }

            // Get all previously imported page types.
            foreach (var pageType in await _api.PageTypes.GetAllAsync())
            {
                if (!importTypes.Any(t => t.Id == pageType.Id))
                    orphans.Add(pageType);
            }

            // Delete all orphans.
            foreach (var pageType in orphans)
            {
                await _api.PageTypes.DeleteAsync(pageType);
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

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title))
                {
                    var pageType = new PageType
                    {
                        Id = attr.Id,
                        CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                        Title = attr.Title,
                        UseBlocks = attr.UseBlocks,
                        IsArchive = typeof(IArchivePage).IsAssignableFrom(type)
                    };

                    // Get all page routes
                    var routes = type.GetTypeInfo().GetCustomAttributes(typeof(PageTypeRouteAttribute));
                    foreach (PageTypeRouteAttribute route in routes)
                    {
                        if (!string.IsNullOrWhiteSpace(route.Title) && !string.IsNullOrWhiteSpace(route.Route))
                            pageType.Routes.Add(new ContentTypeRoute
                            {
                                Title = route.Title,
                                Route = route.Route
                            });
                    }

                    // Add default custom editors
                    if (typeof(IArchivePage).IsAssignableFrom(type))
                    {
                        pageType.CustomEditors.Add(new ContentTypeEditor
                        {
                            Component = "post-archive",
                            Icon = "fas fa-book",
                            Title = "Archive"
                        });
                    }

                    // Get all custom editors
                    var editors = type.GetTypeInfo().GetCustomAttributes(typeof(PageTypeEditorAttribute));
                    foreach (PageTypeEditorAttribute editor in editors)
                    {
                        if (!string.IsNullOrWhiteSpace(editor.Component) && !string.IsNullOrWhiteSpace(editor.Title))
                        {
                            // Check if we already have an editor registered with this name
                            var current = pageType.CustomEditors.FirstOrDefault(e => e.Title == editor.Title);

                            if (current != null)
                            {
                                // Replace current editor
                                current.Component = editor.Component;
                                current.Icon = editor.Icon;
                                current.Title = editor.Title;
                            }
                            else
                            {
                                // Add new editor
                                pageType.CustomEditors.Add(new ContentTypeEditor
                                {
                                    Component = editor.Component,
                                    Icon = editor.Icon,
                                    Title = editor.Title
                                });
                            }
                        }
                    }

                    // Get all allowed archive items, if this is an archive page
                    if (typeof(IArchivePage).IsAssignableFrom(type))
                    {
                        var itemTypes = type.GetCustomAttributes(typeof(PageTypeArchiveItemAttribute));
                        foreach (PageTypeArchiveItemAttribute itemType in itemTypes)
                        {
                            var postAttr = itemType.PostType.GetCustomAttribute<PostTypeAttribute>();
                            if (postAttr != null)
                            {
                                var typeId = postAttr.Id;
                                if (string.IsNullOrWhiteSpace(typeId))
                                {
                                    typeId = itemType.PostType.Name;
                                }
                                pageType.ArchiveItemTypes.Add(typeId);
                            }
                        }
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
            }
            else
            {
                throw new ArgumentException($"Title is mandatory in PageTypeAttribute. No title provided for {type.Name}");
            }
            return null;
        }
        #endregion
    }
}
