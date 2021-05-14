/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Piranha.Extend;
using Piranha.Manager.Models;
using Piranha.Models;
using Piranha.Security;

namespace Piranha.Manager
{
    public sealed class Module : IModule
    {
        /// <summary>
        /// Gets the mapper instance for this module.
        /// </summary>
        public static IMapper Mapper { get; private set; }

        private readonly List<PermissionItem> _permissions = new List<PermissionItem>
        {
            new PermissionItem { Name = Permission.Admin, Title = "Admin" },
            new PermissionItem { Name = Permission.Aliases, Title = "List Aliases", Category = "Aliases" },
            new PermissionItem { Name = Permission.AliasesDelete, Title = "Delete Aliases", Category = "Aliases" },
            new PermissionItem { Name = Permission.AliasesEdit, Title = "Edit Aliases", Category = "Aliases" },
            new PermissionItem { Name = Permission.Comments, Title = "List Comments", Category = "Comments" },
            new PermissionItem { Name = Permission.CommentsApprove, Title = "Approve Comments", Category = "Comments" },
            new PermissionItem { Name = Permission.CommentsDelete, Title = "Delete Comments", Category = "Comments" },
            new PermissionItem { Name = Permission.Config, Title = "View Config", Category = "Config" },
            new PermissionItem { Name = Permission.ConfigEdit, Title = "Edit Config", Category = "Config" },
            new PermissionItem { Name = Permission.Content, Title = "List content", Category = "Content" },
            new PermissionItem { Name = Permission.ContentAdd, Title = "Add content", Category = "Content" },
            new PermissionItem { Name = Permission.ContentDelete, Title = "Delete content", Category = "Content" },
            new PermissionItem { Name = Permission.ContentEdit, Title = "Edit content", Category = "Content" },
            new PermissionItem { Name = Permission.ContentSave, Title = "Save content", Category = "Content" },
            new PermissionItem { Name = Permission.Language, Title = "List Languages", Category = "Languages" },
            new PermissionItem { Name = Permission.LanguageAdd, Title = "Add Languages", Category = "Languages" },
            new PermissionItem { Name = Permission.LanguageDelete, Title = "Delete Languages", Category = "Languages" },
            new PermissionItem { Name = Permission.LanguageEdit, Title = "Edit Languages", Category = "Languages" },
            new PermissionItem { Name = Permission.Media, Title = "List Media", Category = "Media" },
            new PermissionItem { Name = Permission.MediaAdd, Title = "Add Media", Category = "Media" },
            new PermissionItem { Name = Permission.MediaAddFolder, Title = "Add Media Folders", Category = "Media" },
            new PermissionItem { Name = Permission.MediaDelete, Title = "Delete Media", Category = "Media" },
            new PermissionItem { Name = Permission.MediaDeleteFolder, Title = "Delete Media Folders", Category = "Media" },
            new PermissionItem { Name = Permission.MediaEdit, Title = "Edit Media", Category = "Media" },
            new PermissionItem { Name = Permission.Modules, Title = "List Modules", Category = "Modules" },
            new PermissionItem { Name = Permission.Pages, Title = "List Pages", Category = "Pages" },
            new PermissionItem { Name = Permission.PagesAdd, Title = "Add Pages", Category = "Pages" },
            new PermissionItem { Name = Permission.PagesDelete, Title = "Delete Pages", Category = "Pages" },
            new PermissionItem { Name = Permission.PagesEdit, Title = "Edit Pages", Category = "Pages" },
            new PermissionItem { Name = Permission.PagesPublish, Title = "Publish Pages", Category = "Pages" },
            new PermissionItem { Name = Permission.PagesSave, Title = "Pages - Save", Category = "Pages" },
            new PermissionItem { Name = Permission.Posts, Title = "List Posts", Category = "Posts" },
            new PermissionItem { Name = Permission.PostsAdd, Title = "Add Posts", Category = "Posts" },
            new PermissionItem { Name = Permission.PostsDelete, Title = "Delete Posts", Category = "Posts" },
            new PermissionItem { Name = Permission.PostsEdit, Title = "Edit Posts", Category = "Posts" },
            new PermissionItem { Name = Permission.PostsPublish, Title = "Publish Posts", Category = "Posts" },
            new PermissionItem { Name = Permission.PostsSave, Title = "Save Posts", Category = "Posts" },
            new PermissionItem { Name = Permission.Sites, Title = "List Sites", Category = "Sites" },
            new PermissionItem { Name = Permission.SitesAdd, Title = "Add Sites", Category = "Sites" },
            new PermissionItem { Name = Permission.SitesDelete, Title = "Delete Sites", Category = "Sites" },
            new PermissionItem { Name = Permission.SitesEdit, Title = "Edit Sites", Category = "Sites" },
            new PermissionItem { Name = Permission.SitesSave, Title = "Save Sites", Category = "Sites" }
        };

        /// <summary>
        /// The currently registered custom scripts.
        /// </summary>
        public List<ManagerScriptDefinition> Scripts { get; private set; }

        /// <summary>
        /// The currently registered custom styles.
        /// </summary>
        public List<string> Styles { get; private set; }

        /// <summary>
        /// The currently registrered partial views.
        /// </summary>
        public List<string> Partials { get; private set; }

        /// <summary>
        /// Gets/sets the url that should be used to sign out
        /// of the manager.
        /// </summary>
        public string LogoutUrl { get; set; }

        /// <summary>
        /// Gets/sets the url to the currently registered editor init script.
        /// </summary>
        public static string EditorInitScriptUrl { get; set; }

        /// <summary>
        /// The currently registered preview sizes.
        /// </summary>
        public List<PreviewSize> PreviewSizes { get; private set; } = new List<PreviewSize> {
            new PreviewSize { Title = "Desktop", Width = "100%", IconCss = "fas fa-desktop" },
            new PreviewSize { Title = "Laptop", Width = "1200px", IconCss = "fas fa-laptop" },
            new PreviewSize { Title = "Tablet", Width = "768px", IconCss = "fas fa-tablet-alt" },
            new PreviewSize { Title = "Mobile", Width = "320px", IconCss = "fas fa-mobile-alt" }
        };

        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha.Manager";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Manager panel for Piranha CMS for AspNetCore.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/Piranha.Manager";

        /// <summary>
        /// Gets the icon url.
        /// </summary>
        public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

        /// <summary>
        /// The assembly.
        /// </summary>
        internal static readonly Assembly Assembly;

        /// <summary>
        /// Last modification date of the assembly.
        /// </summary>
        internal static readonly DateTime LastModified;

        /// <summary>
        /// Static initialization.
        /// </summary>
        static Module()
        {
            // Get assembly information
            Assembly = typeof(Module).GetTypeInfo().Assembly;
            LastModified = new FileInfo(Assembly.Location).LastWriteTime;

            // Create mapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GenericContent, ContentModel>()
                    .ForMember(m => m.Permissions, o => o.Ignore())
                    .ForMember(m => m.Status, o => o.Ignore())
                    .ForMember(m => m.IsReadOnly, o => o.Ignore())
                    .ForMember(m => m.IsScheduled, o => o.Ignore())
                    .ForMember(m => m.LanguageId, o => o.Ignore())
                    .ForMember(m => m.LanguageTitle, o => o.Ignore())
                    .ForMember(m => m.ParentId, o => o.Ignore())
                    .ForMember(m => m.Published, o => o.Ignore())
                    .ForMember(m => m.PublishedTime, o => o.Ignore())
                    .ForMember(m => m.Comments, o => o.Ignore())
                    .ForMember(m => m.Meta, o => o.Ignore())
                    .ForMember(m => m.Routes, o => o.Ignore())
                    .ForMember(m => m.Taxonomies, o => o.Ignore())
                    .ForMember(m => m.TypeTitle, o => o.Ignore())
                    .ForMember(m => m.GroupId, o => o.Ignore())
                    .ForMember(m => m.GroupTitle, o => o.Ignore())
                    .ForMember(m => m.Slug, o => o.Ignore())
                    .ForMember(m => m.Features, o => o.Ignore())
                    .ForMember(m => m.Position, o => o.Ignore())
                    .ForMember(m => m.AltTitle, o => o.Ignore())
                    .ForMember(m => m.State, o => o.Ignore())
                    .ForMember(m => m.Editors, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.Languages, o => o.Ignore())
                    .ForMember(m => m.Sections, o => o.Ignore());
                cfg.CreateMap<PageBase, ContentModel>()
                    .ForMember(m => m.AltTitle, o => o.MapFrom(p => p.NavigationTitle))
                    .ForMember(m => m.IsScheduled, o => o.MapFrom(p => p.Published.HasValue && p.Published.Value > DateTime.Now))
                    .ForMember(m => m.Published, o => o.MapFrom(p => p.Published.HasValue ? p.Published.Value.ToString("yyyy-MM-dd") : null))
                    .ForMember(m => m.PublishedTime, o => o.MapFrom(p => p.Published.HasValue ? p.Published.Value.ToString("HH:mm") : null))
                    .ForMember(m => m.Comments, o => o.MapFrom(p => new ContentComments
                    {
                        CloseCommentsAfterDays = p.CloseCommentsAfterDays,
                        CommentCount = p.CommentCount
                    }))
                    .ForMember(m => m.Meta, o => o.MapFrom(p => new ContentMeta
                    {
                        MetaTitle = p.MetaTitle,
                        MetaKeywords = p.MetaKeywords,
                        MetaDescription = p.MetaDescription,
                        MetaFollow = p.MetaFollow,
                        MetaIndex = p.MetaIndex,
                        MetaPriority = p.MetaPriority,
                        OgTitle = p.OgTitle,
                        OgImage = p.OgImage,
                        OgDescription = p.OgDescription
                    }))
                    .ForMember(m => m.Permissions, o => o.MapFrom(p => new ContentPermissions
                    {
                        SelectedPermissions = p.Permissions,
                        Permissions = App.Permissions
                            .GetPublicPermissions()
                            .Select(p => new KeyValuePair<string, string>(p.Name, p.Title))
                            .ToList()
                    }))
                    .ForMember(m => m.Position, o => o.MapFrom(p => new ContentPosition
                    {
                        IsHidden = p.IsHidden,
                        SiteId = p.SiteId,
                        SortOrder = p.SortOrder
                    }))
                    .ForMember(m => m.Routes, o => o.MapFrom(p => new ContentRoutes
                    {
                        SelectedRoute = new RouteModel { Route = p.Route },
                        RedirectUrl = p.RedirectUrl,
                        RedirectType = p.RedirectType.ToString()
                    }))
                    .ForMember(m => m.Status, o => o.Ignore())
                    .ForMember(m => m.IsReadOnly, o => o.Ignore())
                    .ForMember(m => m.LanguageId, o => o.Ignore())
                    .ForMember(m => m.LanguageTitle, o => o.Ignore())
                    .ForMember(m => m.TypeTitle, o => o.Ignore())
                    .ForMember(m => m.GroupId, o => o.Ignore())
                    .ForMember(m => m.GroupTitle, o => o.Ignore())
                    .ForMember(m => m.Features, o => o.Ignore())
                    .ForMember(m => m.Taxonomies, o => o.Ignore())
                    .ForMember(m => m.State, o => o.Ignore())
                    .ForMember(m => m.Editors, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.Languages, o => o.Ignore())
                    .ForMember(m => m.Sections, o => o.Ignore());
                cfg.CreateMap<PostBase, ContentModel>()
                    .ForMember(m => m.ParentId, o => o.MapFrom(p => p.BlogId))
                    .ForMember(m => m.IsScheduled, o => o.MapFrom(p => p.Published.HasValue && p.Published.Value > DateTime.Now))
                    .ForMember(m => m.Published, o => o.MapFrom(p => p.Published.HasValue ? p.Published.Value.ToString("yyyy-MM-dd") : null))
                    .ForMember(m => m.PublishedTime, o => o.MapFrom(p => p.Published.HasValue ? p.Published.Value.ToString("HH:mm") : null))
                    .ForMember(m => m.Comments, o => o.MapFrom(p => new ContentComments
                    {
                        CloseCommentsAfterDays = p.CloseCommentsAfterDays,
                        CommentCount = p.CommentCount
                    }))
                    .ForMember(m => m.Meta, o => o.MapFrom(p => new ContentMeta
                    {
                        MetaTitle = p.MetaTitle,
                        MetaKeywords = p.MetaKeywords,
                        MetaDescription = p.MetaDescription,
                        MetaFollow = p.MetaFollow,
                        MetaIndex = p.MetaIndex,
                        MetaPriority = p.MetaPriority,
                        OgTitle = p.OgTitle,
                        OgImage = p.OgImage,
                        OgDescription = p.OgDescription
                    }))
                    .ForMember(m => m.Permissions, o => o.MapFrom(p => new ContentPermissions
                    {
                        SelectedPermissions = p.Permissions,
                        Permissions = App.Permissions
                            .GetPublicPermissions()
                            .Select(p => new KeyValuePair<string, string>(p.Name, p.Title))
                            .ToList()
                    }))
                    .ForMember(m => m.Routes, o => o.MapFrom(p => new ContentRoutes
                    {
                        SelectedRoute = new RouteModel { Route = p.Route },
                        RedirectUrl = p.RedirectUrl,
                        RedirectType = p.RedirectType.ToString()
                    }))
                    .ForMember(m => m.Taxonomies, o => o.MapFrom(p => new ContentTaxonomies
                    {
                        SelectedCategory = p.Category.Title,
                        SelectedTags = p.Tags.Select(t => t.Title).ToList()
                    }))
                    .ForMember(m => m.Status, o => o.Ignore())
                    .ForMember(m => m.IsReadOnly, o => o.Ignore())
                    .ForMember(m => m.LanguageId, o => o.Ignore())
                    .ForMember(m => m.LanguageTitle, o => o.Ignore())
                    .ForMember(m => m.TypeTitle, o => o.Ignore())
                    .ForMember(m => m.GroupId, o => o.Ignore())
                    .ForMember(m => m.GroupTitle, o => o.Ignore())
                    .ForMember(m => m.Features, o => o.Ignore())
                    .ForMember(m => m.Position, o => o.Ignore())
                    .ForMember(m => m.AltTitle, o => o.Ignore())
                    .ForMember(m => m.State, o => o.Ignore())
                    .ForMember(m => m.Editors, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.Languages, o => o.Ignore())
                    .ForMember(m => m.Sections, o => o.Ignore());

                cfg.CreateMap<ContentModel, GenericContent>()
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.Created, o => o.Ignore())
                    .ForMember(p => p.LastModified, o => o.Ignore());

                cfg.CreateMap<ContentModel, PageBase>()
                    .ForMember(p => p.SiteId, o => o.MapFrom(m => m.Position.SiteId))
                    .ForMember(p => p.SortOrder, o => o.MapFrom(m => m.Position.SortOrder))
                    .ForMember(p => p.IsHidden, o => o.MapFrom(m => m.Position.IsHidden))
                    .ForMember(p => p.MetaTitle, o => o.MapFrom(m => m.Meta.MetaTitle))
                    .ForMember(p => p.MetaKeywords, o => o.MapFrom(m => m.Meta.MetaKeywords))
                    .ForMember(p => p.MetaDescription, o => o.MapFrom(m => m.Meta.MetaDescription))
                    .ForMember(p => p.MetaFollow, o => o.MapFrom(m => m.Meta.MetaFollow))
                    .ForMember(p => p.MetaIndex, o => o.MapFrom(m => m.Meta.MetaIndex))
                    .ForMember(p => p.MetaPriority, o => o.MapFrom(m => m.Meta.MetaPriority))
                    .ForMember(p => p.OgTitle, o => o.MapFrom(m => m.Meta.OgTitle))
                    .ForMember(p => p.OgImage, o => o.MapFrom(m => m.Meta.OgImage))
                    .ForMember(p => p.OgDescription, o => o.MapFrom(m => m.Meta.OgDescription))
                    .ForMember(p => p.NavigationTitle, o => o.MapFrom(m => m.AltTitle))
                    .ForMember(p => p.EnableComments, o => o.MapFrom(m => m.Comments.EnableComments))
                    .ForMember(p => p.CloseCommentsAfterDays, o => o.MapFrom(m => m.Comments.CloseCommentsAfterDays))
                    .ForMember(p => p.CommentCount, o => o.MapFrom(m => m.Comments.CommentCount))
                    .ForMember(p => p.RedirectUrl, o => o.MapFrom(m => m.Routes.RedirectUrl))
                    .ForMember(p => p.RedirectType, o => o.MapFrom(m => m.Routes.RedirectType))
                    .ForMember(p => p.Route, o => o.MapFrom(m => m.Routes.SelectedRoute.Route))
                    .ForMember(p => p.Blocks, o => o.Ignore())
                    .ForMember(p => p.OriginalPageId, o => o.Ignore())
                    .ForMember(p => p.Permalink, o => o.Ignore())
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.Created, o => o.Ignore())
                    .ForMember(p => p.LastModified, o => o.Ignore());

                cfg.CreateMap<ContentModel, PostBase>()
                    .ForMember(p => p.BlogId, o => o.MapFrom(m => m.ParentId))
                    .ForMember(p => p.MetaTitle, o => o.MapFrom(m => m.Meta.MetaTitle))
                    .ForMember(p => p.MetaKeywords, o => o.MapFrom(m => m.Meta.MetaKeywords))
                    .ForMember(p => p.MetaDescription, o => o.MapFrom(m => m.Meta.MetaDescription))
                    .ForMember(p => p.MetaFollow, o => o.MapFrom(m => m.Meta.MetaFollow))
                    .ForMember(p => p.MetaIndex, o => o.MapFrom(m => m.Meta.MetaIndex))
                    .ForMember(p => p.MetaPriority, o => o.MapFrom(m => m.Meta.MetaPriority))
                    .ForMember(p => p.OgTitle, o => o.MapFrom(m => m.Meta.OgTitle))
                    .ForMember(p => p.OgImage, o => o.MapFrom(m => m.Meta.OgImage))
                    .ForMember(p => p.OgDescription, o => o.MapFrom(m => m.Meta.OgDescription))
                    .ForMember(p => p.EnableComments, o => o.MapFrom(m => m.Comments.EnableComments))
                    .ForMember(p => p.CloseCommentsAfterDays, o => o.MapFrom(m => m.Comments.CloseCommentsAfterDays))
                    .ForMember(p => p.CommentCount, o => o.MapFrom(m => m.Comments.CommentCount))
                    .ForMember(p => p.RedirectUrl, o => o.MapFrom(m => m.Routes.RedirectUrl))
                    .ForMember(p => p.RedirectType, o => o.MapFrom(m => m.Routes.RedirectType))
                    .ForMember(p => p.Route, o => o.MapFrom(m => m.Routes.SelectedRoute.Route))
                    .ForMember(p => p.Category, o => o.Ignore())
                    .ForMember(p => p.Tags, o => o.Ignore())
                    .ForMember(p => p.Blocks, o => o.Ignore())
                    .ForMember(p => p.Permalink, o => o.Ignore())
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.Created, o => o.Ignore())
                    .ForMember(p => p.LastModified, o => o.Ignore());
            });

            mapperConfig.AssertConfigurationIsValid();
            Mapper = mapperConfig.CreateMapper();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Module()
        {
            Partials = new List<string>();
            Scripts = new List<ManagerScriptDefinition>();
            Styles = new List<string>();
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init()
        {
            // Register permissions
            foreach (var permission in _permissions)
            {
                // Set the permission to internal
                permission.IsInternal = true;

                // Add to the global permission collection
                App.Permissions["Manager"].Add(permission);
            }
        }
    }
}
