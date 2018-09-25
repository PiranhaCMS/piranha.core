/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using AutoMapper;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Security;

namespace Piranha.Manager
{
    public class Module : Extend.IModule
    {
        private readonly List<PermissionItem> _permissions = new List<PermissionItem>() {
            new PermissionItem() { Name = Permission.Admin, Title = "Admin"},
            new PermissionItem() { Name = Permission.Aliases, Title = "List Aliases", Category = "Aliases" },
            new PermissionItem() { Name = Permission.AliasesDelete, Title = "Delete Aliases", Category = "Aliases" },
            new PermissionItem() { Name = Permission.AliasesEdit, Title = "Edit Aliases", Category = "Aliases" },
            new PermissionItem() { Name = Permission.Config, Title = "View Config", Category = "Config" },
            new PermissionItem() { Name = Permission.ConfigEdit, Title = "Edit Config", Category = "Config" },
            new PermissionItem() { Name = Permission.Media, Title = "List Media", Category = "Media" },
            new PermissionItem() { Name = Permission.MediaAdd, Title = "Add Media", Category = "Media" },
            new PermissionItem() { Name = Permission.MediaAddFolder, Title = "Add Media Folders", Category = "Media" },
            new PermissionItem() { Name = Permission.MediaDelete, Title = "Delete Media", Category = "Media" },
            new PermissionItem() { Name = Permission.MediaDeleteFolder, Title = "Delete Media Folders", Category = "Media" },
            new PermissionItem() { Name = Permission.MediaEdit, Title = "Edit Media", Category = "Media" },
            new PermissionItem() { Name = Permission.Pages, Title = "List Pages", Category = "Pages" },
            new PermissionItem() { Name = Permission.PagesAdd, Title = "Add Pages", Category = "Pages" },
            new PermissionItem() { Name = Permission.PagesDelete, Title = "Delete Pages", Category = "Pages" },
            new PermissionItem() { Name = Permission.PagesEdit, Title = "Edit Pages", Category = "Pages" },
            new PermissionItem() { Name = Permission.PagesPublish, Title = "Publish Pages", Category = "Pages" },
            new PermissionItem() { Name = Permission.PagesSave, Title = "Pages - Save", Category = "Pages" },
            new PermissionItem() { Name = Permission.Posts, Title = "List Posts", Category = "Posts" },
            new PermissionItem() { Name = Permission.PostsAdd, Title = "Add Posts", Category = "Posts" },
            new PermissionItem() { Name = Permission.PostsDelete, Title = "Delete Posts", Category = "Posts" },
            new PermissionItem() { Name = Permission.PostsEdit, Title = "Edit Posts", Category = "Posts" },
            new PermissionItem() { Name = Permission.PostsPublish, Title = "Publish Posts", Category = "Posts" },
            new PermissionItem() { Name = Permission.PostsSave, Title = "Save Posts", Category = "Posts" },
            new PermissionItem() { Name = Permission.Sites, Title = "List Sites", Category = "Sites" },
            new PermissionItem() { Name = Permission.SitesAdd, Title = "Add Sites", Category = "Sites" },
            new PermissionItem() { Name = Permission.SitesDelete, Title = "Delete Sites", Category = "Sites" },
            new PermissionItem() { Name = Permission.SitesEdit, Title = "Edit Sites", Category = "Sites" },
            new PermissionItem() { Name = Permission.SitesSave, Title = "Save Sites", Category = "Sites" }
        };

        /// <summary>
        /// The currently registered custom scripts.
        /// </summary>
        public List<string> Scripts { get; private set; }

        /// <summary>
        /// The currently registered custom styles.
        /// </summary>
        public List<string> Styles { get; private set; }

        /// <summary>
        /// The currently registrered partial views.
        /// </summary>
        public List<string> Partials { get; private set; }

        /// <summary>
        /// The currently registered preview sizes.
        /// </summary>
        public List<PreviewSize> PreviewSizes { get; private set; } = new List<PreviewSize>() {
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
        /// Gets the release date
        /// </summary>
        public string ReleaseDate => "2018-05-30";

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Manager panel for Piranha CMS for AspNetCore.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageURL => "https://www.nuget.org/packages/Piranha.Manager";

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// The assembly.
        /// </summary>
        internal static Assembly Assembly;

        /// <summary>
        /// Last modification date of the assembly.
        /// </summary>
        internal static DateTime LastModified;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Module() {
            Partials = new List<string>();
            Scripts = new List<string>();
            Styles = new List<string>();
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.PageBase, Areas.Manager.Models.PageEditModel>()
                    .ForMember(m => m.PageType, o => o.Ignore())
                    .ForMember(m => m.Blocks, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.PageContentType, o => o.Ignore());
                cfg.CreateMap<Areas.Manager.Models.PageEditModel, Models.PageBase>()
                    .ForMember(m => m.Blocks, o => o.Ignore())
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
                cfg.CreateMap<Models.PostBase, Areas.Manager.Models.PostEditModel>()
                    .ForMember(m => m.PostType, o => o.Ignore())
                    .ForMember(m => m.Blocks, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.AllCategories, o => o.Ignore())
                    .ForMember(m => m.AllTags, o => o.Ignore())
                    .ForMember(m => m.SelectedCategory, o => o.Ignore())
                    .ForMember(m => m.SelectedTags, o => o.Ignore())
                    .ForMember(m => m.BlogSlug, o => o.Ignore());
                cfg.CreateMap<Areas.Manager.Models.PostEditModel, Models.PostBase>()
                    .ForMember(m => m.Blocks, o => o.Ignore())
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
                cfg.CreateMap<Models.SiteContentBase, Areas.Manager.Models.SiteContentEditModel>()
                    .ForMember(s => s.SiteType, o => o.Ignore())
                    .ForMember(s => s.Regions, o => o.Ignore());
                cfg.CreateMap<Areas.Manager.Models.SiteContentEditModel, Models.SiteContentBase>()
                    .ForMember(s => s.TypeId, o => o.Ignore())
                    .ForMember(s => s.Created, o => o.Ignore())
                    .ForMember(s => s.LastModified, o => o.Ignore());
            });

            config.AssertConfigurationIsValid();
            Mapper = config.CreateMapper();

            // Register permissions
            foreach (var permission in _permissions)
            {
                App.Permissions["Manager"].Add(permission);
            }

            // Get assembly information
            Assembly = this.GetType().GetTypeInfo().Assembly;
            LastModified = new FileInfo(Assembly.Location).LastWriteTime;
        }
    }
}
