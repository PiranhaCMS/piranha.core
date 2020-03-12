using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Extend
{
    /// <summary>
    /// Class for building content types from models.
    /// </summary>
    public sealed class ContentTypeBuilder
    {
        private IApi _api;
        private readonly List<Type> _types = new List<Type>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ContentTypeBuilder(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Adds a new type to build content types from
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The builder</returns>
        public ContentTypeBuilder AddType(Type type)
        {
            _types.Add(type);

            return this;
        }

        /// <summary>
        /// Builds the content types.
        /// </summary>
        public ContentTypeBuilder Build()
        {
            return BuildAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Builds the content types.
        /// </summary>
        public async Task<ContentTypeBuilder> BuildAsync()
        {
            foreach (var type in _types)
            {
                var contentType = GetContentType(type);

                if (contentType != null)
                {
                    await _api.ContentTypes.SaveAsync(contentType);
                }
            }
            return this;

        }

        /// <summary>
        /// Gets the possible content type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The content type</returns>
        private ContentType GetContentType(Type type)
        {
            var group = type.GetCustomAttribute<ContentGroupAttribute>(true);
            if (group == null)
            {
                throw new ArgumentException($"Content Group is missing for the Content Type { type.Name }");
            }

            var attr = type.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();

            if (attr != null)
            {
                if (string.IsNullOrWhiteSpace(attr.Id))
                    attr.Id = type.Name;

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title))
                {
                    var contentType = new ContentType
                    {
                        Id = attr.Id,
                        TypeName = type.FullName,
                        AssemblyName = type.Assembly.GetName().Name,
                        Title = attr.Title,
                        Group = group.Title,
                        IsPrimaryContent = group.IsPrimaryContent
                    };

                    // Get all page routes
                    var routes = type.GetTypeInfo().GetCustomAttributes(typeof(ContentTypeRouteAttribute));
                    foreach (ContentTypeRouteAttribute route in routes)
                    {
                        if (!string.IsNullOrWhiteSpace(route.Title) && !string.IsNullOrWhiteSpace(route.Route))
                            contentType.Routes.Add(new ContentTypeRoute
                            {
                                Title = route.Title,
                                Route = route.Route
                            });
                    }

                    // Add default route if there were no defined ones
                    if (contentType.Routes.Count == 0)
                    {
                        contentType.Routes.Add(new ContentTypeRoute
                        {
                            Title = "Default",
                            Route = group.DefaultRoute
                        });
                    }

                    // Get all custom editors
                    var editors = type.GetTypeInfo().GetCustomAttributes(typeof(ContentTypeEditorAttribute));
                    foreach (ContentTypeEditorAttribute editor in editors)
                    {
                        if (!string.IsNullOrWhiteSpace(editor.Component) && !string.IsNullOrWhiteSpace(editor.Title))
                        {
                            // Check if we already have an editor registered with this name
                            var current = contentType.CustomEditors.FirstOrDefault(e => e.Title == editor.Title);

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
                                contentType.CustomEditors.Add(new ContentTypeEditor
                                {
                                    Component = editor.Component,
                                    Icon = editor.Icon,
                                    Title = editor.Title
                                });
                            }
                        }
                    }

                    // Get all regions
                    var regionTypes = new List<Tuple<int?, ContentTypeRegion>>();
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
                        contentType.Regions.Add(regionType.Item2);
                    // Then add the unsorted regions
                    foreach (var regionType in regionTypes.Where(t => !t.Item1.HasValue))
                        contentType.Regions.Add(regionType.Item2);

                    return contentType;
                }
            }
            else
            {
                throw new ArgumentException($"Title is mandatory in ContentTypeAttribute. No title provided for { type.Name }");
            }
            return null;
        }

        /// <summary>
        /// Gets the possible region type for the given property.
        /// </summary>
        /// <param name="prop">The property info</param>
        /// <returns>The region type</returns>
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

                    var fieldType = new ContentTypeField
                    {
                        Id = "Default"
                    };
                    SetTypeInfo(fieldType, appFieldType);

                    regionType.Fields.Add(fieldType);
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
                return new Tuple<int?, ContentTypeRegion>(sortOrder, regionType);
            }
            return null;
        }

        /// <summary>
        /// Gets the possible field type for the given property.
        /// </summary>
        /// <param name="prop">The property</param>
        /// <returns>The field type</returns>
        private ContentTypeField GetFieldType(PropertyInfo prop)
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
                        Options = attr.Options,
                        Placeholder = attr.Placeholder
                    };

                    SetTypeInfo(fieldType, appFieldType);

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

        private void SetTypeInfo(ContentTypeField field, Runtime.AppField fieldType)
        {
            // Set basic info
            field.Type.TypeName = fieldType.TypeName;
            field.Type.AssemblyName = fieldType.AssemblyName;

            // Handle generic fields
            if (fieldType.Type.IsGenericType)
            {
                field.Type.IsGeneric = true;
                field.Type.TypeName = $"{ fieldType.Type.Namespace }.{ fieldType.Type.Name }";

                foreach (var arg in fieldType.Type.GenericTypeArguments)
                {
                    field.Type.TypeArguments.Add(new ContentTypeFieldInfoBase
                    {
                        TypeName = arg.FullName,
                        AssemblyName = arg.Assembly.GetName().Name
                    });
                }
            }
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