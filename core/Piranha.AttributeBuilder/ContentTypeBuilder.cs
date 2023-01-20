/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections;
using System.Reflection;
using Piranha.Extend;
using Piranha.Models;

namespace Piranha.AttributeBuilder;

/// <summary>
/// Class for simple access to the different content type
/// builders available.
/// </summary>
public class ContentTypeBuilder
{
    private class BuilderItem<T> where T : ContentTypeBase
    {
        public Type Type { get; set; }
        public T ContentType { get; set; }
    }

    private readonly IApi _api;
    private readonly IList<Type> _contentGroups = new List<Type>();
    private readonly IList<BuilderItem<ContentType>> _contentTypes = new List<BuilderItem<ContentType>>();
    private readonly IList<BuilderItem<PageType>> _pageTypes = new List<BuilderItem<PageType>>();
    private readonly IList<BuilderItem<PostType>> _postTypes = new List<BuilderItem<PostType>>();
    private readonly IList<BuilderItem<SiteType>> _siteTypes = new List<BuilderItem<SiteType>>();

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public ContentTypeBuilder(IApi api)
    {
        _api = api;
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
            if (type.IsClass)
            {
                AddType(type);
            }
        }
        return this;
    }

    /// <summary>
    /// Adds a new type to build content types from.
    /// </summary>
    /// <param name="type">The type</param>
    /// <returns>The builder</returns>
    public ContentTypeBuilder AddType(Type type)
    {
        // Make sure the type is a class
        if (!type.IsClass) return this;

        if (!type.IsAbstract)
        {
            // Type is a non abstract class, check if it is a content type
            if (typeof(GenericContent).IsAssignableFrom(type))
            {
                if (type.GetCustomAttribute<ContentTypeAttribute>() != null)
                {
                    _contentTypes.Add(new BuilderItem<ContentType>
                    {
                        Type = type
                    });

                    // Make sure we add the content group for this type as well
                    var groupType = GetContentGroupType(type);
                    if (groupType != null)
                    {
                        if (!_contentGroups.Contains(groupType))
                        {
                            _contentGroups.Add(groupType);
                        }
                    }
                }
            }
            else if (typeof(PageBase).IsAssignableFrom(type))
            {
                if (type.GetCustomAttribute<PageTypeAttribute>() != null)
                {
                    _pageTypes.Add(new BuilderItem<PageType>
                    {
                        Type = type
                    });
                }
            }
            else if (typeof(PostBase).IsAssignableFrom(type))
            {
                if (type.GetCustomAttribute<PostTypeAttribute>() != null)
                {
                    _postTypes.Add(new BuilderItem<PostType>
                    {
                        Type = type
                    });
                }
            }
            else if (typeof(SiteContentBase).IsAssignableFrom(type))
            {
                if (type.GetCustomAttribute<SiteTypeAttribute>() != null)
                {
                    _siteTypes.Add(new BuilderItem<SiteType>
                    {
                        Type = type
                    });
                }
            }
        }
        return this;
    }

    /// <summary>
    /// Builds all of the content types and saves them to the
    /// database.
    /// </summary>
    /// <returns>The builder</returns>
    public ContentTypeBuilder Build()
    {
        return BuildAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Builds all of the content types and saves them to the
    /// database.
    /// </summary>
    /// <returns>The builder</returns>
    public async Task<ContentTypeBuilder> BuildAsync()
    {
        // Build content groups
        foreach (var t in _contentGroups)
        {
            var group = GetContentGroup(t);
            if (group != null)
            {
                await _api.ContentGroups.SaveAsync(group);
            }
        }

        // Build content types
        foreach (var t in _contentTypes)
        {
            var type = GetContentType(t.Type);
            if (type != null)
            {
                type.Ensure();
                t.ContentType = type;
                await _api.ContentTypes.SaveAsync(type);
            }
        }

        // Build page types
        foreach (var t in _pageTypes)
        {
            var type = GetPageType(t.Type);
            if (type != null)
            {
                type.Ensure();
                t.ContentType = type;
                await _api.PageTypes.SaveAsync(type);
            }
        }

        // Build post types
        foreach (var t in _postTypes)
        {
            var type = GetPostType(t.Type);
            if (type != null)
            {
                type.Ensure();
                t.ContentType = type;
                await _api.PostTypes.SaveAsync(type);
            }
        }

        // Build site types
        foreach (var t in _siteTypes)
        {
            var type = GetSiteType(t.Type);
            if (type != null)
            {
                type.Ensure();
                t.ContentType = type;
                await _api.SiteTypes.SaveAsync(type);
            }
        }

        return this;
    }

    /// <summary>
    /// Deletes all content types that does not currently exist in
    /// the builder.
    /// </summary>
    /// <returns>The builder</returns>
    public ContentTypeBuilder DeleteOrphans()
    {
        return DeleteOrphansAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Deletes all content types that does not currently exist in
    /// the builder.
    /// </summary>
    /// <returns>The builder</returns>
    public async Task<ContentTypeBuilder> DeleteOrphansAsync()
    {
        var orphanedPageTypes = new List<PageType>();
        foreach (var pt in await _api.PageTypes.GetAllAsync())
        {
            if (!_pageTypes.Any(t => t.ContentType.Id == pt.Id))
            {
                orphanedPageTypes.Add(pt);
            }
        }
        await _api.PageTypes.DeleteAsync(orphanedPageTypes);

        var orphanedPostTypes = new List<PostType>();
        foreach (var pt in await _api.PostTypes.GetAllAsync())
        {
            if (!_postTypes.Any(t => t.ContentType.Id == pt.Id))
            {
                orphanedPostTypes.Add(pt);
            }
        }
        await _api.PostTypes.DeleteAsync(orphanedPostTypes);

        var orphanedSiteTypes = new List<SiteType>();
        foreach (var st in await _api.SiteTypes.GetAllAsync())
        {
            if (!_siteTypes.Any(t => t.ContentType.Id == st.Id))
            {
                orphanedSiteTypes.Add(st);
            }
        }
        await _api.SiteTypes.DeleteAsync(orphanedSiteTypes);

        return this;
    }

    private Type GetContentGroupType(Type type)
    {
        if (type.GetCustomAttribute<ContentGroupAttribute>(false) != null)
        {
            return type;
        }

        if (type.BaseType != null)
        {
            return GetContentGroupType(type.BaseType);
        }
        return null;
    }

    private ContentGroup GetContentGroup(Type type)
    {
        var attr = type.GetCustomAttribute<ContentGroupAttribute>(false);

        if (attr != null)
        {
            // Create the content group
            return new ContentGroup
            {
                Id = attr.Id,
                Title = attr.Title,
                CLRType = type.Name, // type.AssemblyQualifiedName,
                Icon = attr.Icon,
                IsHidden = attr.IsHidden
            };
        }
        return null;
    }

    private ContentType GetContentType(Type type)
    {
        var group = type.GetCustomAttribute<ContentGroupAttribute>();
        if (group == null)
        {
            throw new ArgumentException($"Content Group is missing for the Content Type { type.Name }");
        }

        var attr = type.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();

        if (attr != null)
        {
            // Set default id if not specified
            if (string.IsNullOrWhiteSpace(attr.Id))
            {
                attr.Id = type.Name;
            }

            // Make sure both id and title are set
            if (string.IsNullOrEmpty(attr.Id) || string.IsNullOrEmpty(attr.Title))
            {
                throw new ArgumentException($"[{ type.Name }] Id and Title is mandatory for content types.");
            }

            return new ContentType
            {
                Id = attr.Id,
                CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                Title = attr.Title,
                Group = group.Id,
                UseExcerpt = attr.UseExcerpt,
                UsePrimaryImage = attr.UsePrimaryImage,
                UseBlocks = typeof(IBlockContent).IsAssignableFrom(type),
                UseCategory = typeof(ICategorizedContent).IsAssignableFrom(type),
                UseTags = typeof(ITaggedContent).IsAssignableFrom(type),
                CustomEditors = GetEditors(type),
                Regions = GetRegions(type)
            };
        }
        return null;
    }

    private PageType GetPageType(Type type)
    {
        var attr = type.GetTypeInfo().GetCustomAttribute<PageTypeAttribute>();

        if (attr != null)
        {
            // Set default id if not specified
            if (string.IsNullOrWhiteSpace(attr.Id))
            {
                attr.Id = type.Name;
            }

            // Make sure both id and title are set
            if (string.IsNullOrEmpty(attr.Id) || string.IsNullOrEmpty(attr.Title))
            {
                throw new ArgumentException($"[{ type.Name }] Id and Title is mandatory for content types.");
            }

            // Create page type
            var pageType = new PageType
            {
                Id = attr.Id,
                CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                Title = attr.Title,
                Description = attr.Description,
                UseBlocks = attr.UseBlocks,
                UsePrimaryImage = attr.UsePrimaryImage,
                UseExcerpt = attr.UseExcerpt,
                IsArchive = attr.IsArchive,
                Routes = GetRoutes(type),
                CustomEditors = GetEditors(type),
                Regions = GetRegions(type)
            };

            // Add default archive editor
            if (pageType.IsArchive)
            {
                pageType.CustomEditors.Insert(0, new ContentTypeEditor
                {
                    Component = "post-archive",
                    Icon = "fas fa-book",
                    Title = "Archive"
                });
            }

            // Add archive items
            if (pageType.IsArchive)
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

            // Add block types
            var blockTypes = type.GetCustomAttributes<BlockItemTypeAttribute>();
            foreach (var blockType in blockTypes)
            {
                pageType.BlockItemTypes.Add(blockType.Type.FullName);
            }

            return pageType;
        }
        return null;
    }

    private PostType GetPostType(Type type)
    {
        var attr = type.GetTypeInfo().GetCustomAttribute<PostTypeAttribute>();

        if (attr != null)
        {
            // Set default id if not specified
            if (string.IsNullOrWhiteSpace(attr.Id))
            {
                attr.Id = type.Name;
            }

            // Make sure both id and title are set
            if (string.IsNullOrEmpty(attr.Id) || string.IsNullOrEmpty(attr.Title))
            {
                throw new ArgumentException($"[{ type.Name }] Id and Title is mandatory for content types.");
            }

            // Create post type
            var postType = new PostType
            {
                Id = attr.Id,
                CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                Title = attr.Title,
                UseBlocks = attr.UseBlocks,
                UsePrimaryImage = attr.UsePrimaryImage,
                UseExcerpt = attr.UseExcerpt,
                Routes = GetRoutes(type),
                CustomEditors = GetEditors(type),
                Regions = GetRegions(type)
            };

            // Add block types
            var blockTypes = type.GetCustomAttributes<BlockItemTypeAttribute>();
            foreach (var blockType in blockTypes)
            {
                postType.BlockItemTypes.Add(blockType.Type.FullName);
            }

            return postType;
        }
        return null;
    }

    private SiteType GetSiteType(Type type)
    {
        var attr = type.GetTypeInfo().GetCustomAttribute<SiteTypeAttribute>();

        if (attr != null)
        {
            // Set default id if not specified
            if (string.IsNullOrWhiteSpace(attr.Id))
            {
                attr.Id = type.Name;
            }

            // Make sure both id and title are set
            if (string.IsNullOrEmpty(attr.Id) || string.IsNullOrEmpty(attr.Title))
            {
                throw new ArgumentException($"[{ type.Name }] Id and Title is mandatory for content types.");
            }

            // Create post type
            return new SiteType
            {
                Id = attr.Id,
                CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                Title = attr.Title,
                Regions = GetRegions(type)
            };
        }
        return null;
    }

    private IList<ContentTypeRoute> GetRoutes(Type type)
    {
        var routes = new List<ContentTypeRoute>();

        var attrs = type.GetTypeInfo().GetCustomAttributes(typeof(ContentTypeRouteAttribute));
        foreach (ContentTypeRouteAttribute attr in attrs)
        {
            if (!string.IsNullOrWhiteSpace(attr.Route))
            {
                var contentRoute = new ContentTypeRoute
                {
                    Title = attr.Title ?? attr.Route,
                    Route = attr.Route
                };

                // Make sure the route starts with a forward slash
                if (!contentRoute.Route.StartsWith("/"))
                {
                    contentRoute.Route = $"/{ contentRoute.Route }";
                }
                routes.Add(contentRoute);
            }
        }
        return routes;
    }

    private IList<ContentTypeEditor> GetEditors(Type type)
    {
        var editors = new List<ContentTypeEditor>();

        var attrs = type.GetTypeInfo().GetCustomAttributes(typeof(ContentTypeEditorAttribute));
        foreach (ContentTypeEditorAttribute attr in attrs)
        {
            if (!string.IsNullOrWhiteSpace(attr.Component) && !string.IsNullOrWhiteSpace(attr.Title))
            {
                // Check if we already have an editor registered with this name
                var current = editors.FirstOrDefault(e => e.Title == attr.Title);

                if (current != null)
                {
                    // Replace current editor
                    current.Component = attr.Component;
                    current.Icon = attr.Icon;
                    current.Title = attr.Title;
                }
                else
                {
                    // Add new editor
                    editors.Add(new ContentTypeEditor
                    {
                        Component = attr.Component,
                        Icon = attr.Icon,
                        Title = attr.Title
                    });
                }
            }
        }
        return editors;
    }

    private IList<ContentTypeRegion> GetRegions(Type type)
    {
        var regions = new List<ContentTypeRegion>();

        // Get regions
        var sortedRegions = new List<Tuple<int?, ContentTypeRegion>>();
        foreach (var prop in type.GetProperties(App.PropertyBindings))
        {
            var regionType = GetRegionType(prop);

            if (regionType != null)
            {
                sortedRegions.Add(regionType);
            }
        }
        sortedRegions = sortedRegions.OrderBy(t => t.Item1).ToList();

        // First add sorted regions
        foreach (var regionType in sortedRegions.Where(t => t.Item1.HasValue))
            regions.Add(regionType.Item2);
        // Then add the unsorted regions
        foreach (var regionType in sortedRegions.Where(t => !t.Item1.HasValue))
            regions.Add(regionType.Item2);

        return regions;
    }

    private Tuple<int?, ContentTypeRegion> GetRegionType(PropertyInfo prop)
    {
        var attr = prop.GetCustomAttribute<RegionAttribute>();

        if (attr != null)
        {
            var isCollection = typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);

            var regionType = new ContentTypeRegion
            {
                Id = prop.Name,
                Title = attr.Title,
                Description = attr.Description,
                Collection = isCollection,
                ListTitleField = attr.ListTitle,
                ListTitlePlaceholder = attr.ListPlaceholder,
                ListExpand = attr.ListExpand,
                Icon = attr.Icon,
                Display = attr.Display,
                Width = attr.Width
            };
            int? sortOrder = attr.SortOrder != Int32.MaxValue ? attr.SortOrder : (int?)null;

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

                regionType.Fields.Add(new ContentTypeField
                {
                    Id = "Default",
                    Type = appFieldType.TypeName
                });
            }
            else
            {
                var sortedFields = new List<Tuple<int?, ContentTypeField>>();
                foreach (var fieldProp in type.GetProperties(App.PropertyBindings))
                {
                    var fieldType = GetFieldType(fieldProp);

                    if (fieldType != null)
                    {
                        sortedFields.Add(fieldType);
                    }
                }

                // First add sorted fields
                foreach (var fieldType in sortedFields.Where(t => t.Item1.HasValue))
                    regionType.Fields.Add(fieldType.Item2);
                // Then add the unsorted fields
                foreach (var fieldType in sortedFields.Where(t => !t.Item1.HasValue))
                    regionType.Fields.Add(fieldType.Item2);

                // Skip regions without fields.
                if (regionType.Fields.Count == 0)
                {
                    return null;
                }
            }
            return new Tuple<int?, ContentTypeRegion>(sortOrder, regionType);
        }
        return null;
    }

    private Tuple<int?, ContentTypeField> GetFieldType(PropertyInfo prop)
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
                var fieldType = new ContentTypeField
                {
                    Id = prop.Name,
                    Title = attr.Title,
                    Description = attr.Description,
                    Type = appFieldType.TypeName,
                    Options = attr.Options,
                    Placeholder = attr.Placeholder
                };
                int? sortOrder = attr.SortOrder != Int32.MaxValue ? attr.SortOrder : (int?)null;

                // Get optional settings
                fieldType.Settings = Utils.GetFieldSettings(prop);

                return new Tuple<int?, ContentTypeField>(sortOrder, fieldType);
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
                else if (typeof(Extend.Fields.DataSelectFieldBase).IsAssignableFrom(type))
                {
                    var method = typeof(Runtime.AppFieldList).GetMethod("RegisterDataSelect");
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
