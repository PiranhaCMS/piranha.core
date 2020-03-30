/*
 * Copyright (c) .NET Foundation and Contributors
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

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Class for building and importing post types.
    /// </summary>
    public class PostTypeBuilder : ContentTypeBuilder<PostTypeBuilder, PostType>
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PostTypeBuilder(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public override async Task<PostTypeBuilder> BuildAsync()
        {
            foreach (var type in _types)
            {
                var postType = GetContentType(type);

                if (postType != null)
                {
                    postType.Ensure();
                    await _api.PostTypes.SaveAsync(postType);
                }
            }
            return this;
        }

        /// <summary>
        /// Deletes all post types in the database that doesn't
        /// exist in the database,
        /// </summary>
        /// <returns>The builder</returns>
        public PostTypeBuilder DeleteOrphans()
        {
            return DeleteOrphansAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes all page types in the database that doesn't
        /// exist in the database,
        /// </summary>
        /// <returns>The builder</returns>
        public async Task<PostTypeBuilder> DeleteOrphansAsync()
        {
            var orphans = new List<PostType>();
            var importTypes = new List<PostType>();

            // Get all page types added for import.
            foreach (var type in _types)
            {
                var importType = GetContentType(type);

                if (importType != null)
                    importTypes.Add(importType);
            }

            // Get all previously imported page types.
            foreach (var postType in await _api.PostTypes.GetAllAsync())
            {
                if (!importTypes.Any(t => t.Id == postType.Id))
                    orphans.Add(postType);
            }

            // Delete all orphans.
            foreach (var postType in orphans)
            {
                await _api.PostTypes.DeleteAsync(postType);
            }
            return this;
        }

        #region Private methods
        /// <summary>
        /// Gets the possible post type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The page type</returns>
        protected override PostType GetContentType(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttribute<PostTypeAttribute>();

            if (attr != null)
            {
                if (string.IsNullOrWhiteSpace(attr.Id))
                    attr.Id = type.Name;

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title))
                {
                    var postType = new PostType
                    {
                        Id = attr.Id,
                        CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                        Title = attr.Title,
                        UseBlocks = attr.UseBlocks
                    };

                    // Get all post routes
                    var routes = type.GetTypeInfo().GetCustomAttributes(typeof(PostTypeRouteAttribute));
                    foreach (PostTypeRouteAttribute route in routes)
                    {
                        if (!string.IsNullOrWhiteSpace(route.Title) && !string.IsNullOrWhiteSpace(route.Route))
                        {
                            postType.Routes.Add(new ContentTypeRoute
                            {
                                Title = route.Title,
                                Route = route.Route
                            });
                        }
                    }

                    // Get all custom editors
                    var editors = type.GetTypeInfo().GetCustomAttributes(typeof(PostTypeEditorAttribute));
                    foreach (PostTypeEditorAttribute editor in editors)
                    {
                        if (!string.IsNullOrWhiteSpace(editor.Component) && !string.IsNullOrWhiteSpace(editor.Title))
                        {
                            // Check if we already have an editor registered with this name
                            var current = postType.CustomEditors.FirstOrDefault(e => e.Title == editor.Title);

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
                                postType.CustomEditors.Add(new ContentTypeEditor
                                {
                                    Component = editor.Component,
                                    Icon = editor.Icon,
                                    Title = editor.Title
                                });
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
                        postType.Regions.Add(regionType.Item2);
                    // Then add the unsorted regions
                    foreach (var regionType in regionTypes.Where(t => !t.Item1.HasValue))
                        postType.Regions.Add(regionType.Item2);

                    return postType;
                }
            }
            else
            {
                throw new ArgumentException($"Title is mandatory in PostTypeAttribute. No title provided for {type.Name}");
            }
            return null;
        }
        #endregion
    }
}
