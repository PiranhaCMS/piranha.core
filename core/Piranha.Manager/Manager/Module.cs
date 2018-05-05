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

namespace Piranha.Manager
{
    public class Module : Extend.IModule
    {
        private readonly Dictionary<string, string> _permissionNames = new Dictionary<string, string>()
        {
            { Permission.Admin, "Admin"},
            { Permission.Aliases, "Aliases"},
            { Permission.AliasesDelete, "Aliases - Delete"},
            { Permission.AliasesEdit, "Aliases - Edit"},
            { Permission.Config, "Config"},
            { Permission.ConfigEdit, "Config - Edit"},
            { Permission.Media, "Media"},
            { Permission.MediaAdd, "Media - Add"},
            { Permission.MediaAddFolder, "Media - Add folder"},
            { Permission.MediaDelete, "Media - Delete"},
            { Permission.MediaDeleteFolder, "Media - Delete folder"},
            { Permission.MediaEdit, "Media - Edit"},
            { Permission.Pages, "Pages"},
            { Permission.PagesAdd, "Pages - Add"},
            { Permission.PagesDelete, "Pages - Delete"},
            { Permission.PagesEdit, "Pages - Edit"},
            { Permission.PagesPublish, "Pages - Publish"},
            { Permission.PagesSave, "Pages - Save"},
            { Permission.Posts, "Posts"},
            { Permission.PostsAdd, "Posts - Add"},
            { Permission.PostsDelete, "Posts - Delete"},
            { Permission.PostsEdit, "Posts - Edit"},
            { Permission.PostsPublish, "Posts - Publish"},
            { Permission.PostsSave, "Posts - Save"},
            { Permission.Sites, "Sites"},
            { Permission.SitesAdd, "Sites - Add"},
            { Permission.SitesDelete, "Sites - Delete"},
            { Permission.SitesEdit, " Sites - Edit"},
            { Permission.SitesSave, "Sites - Save"}
        };

        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha Manager";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);

        /// <summary>
        /// Gets the release date
        /// </summary>
        public string ReleaseDate => "2018-03-25";

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
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
                cfg.CreateMap<Models.PostBase, Areas.Manager.Models.PostEditModel>()
                    .ForMember(m => m.PostType, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.AllCategories, o => o.Ignore())
                    .ForMember(m => m.AllTags, o => o.Ignore())
                    .ForMember(m => m.SelectedCategory, o => o.Ignore())
                    .ForMember(m => m.SelectedTags, o => o.Ignore())
                    .ForMember(m => m.BlogSlug, o => o.Ignore());
                cfg.CreateMap<Areas.Manager.Models.PostEditModel, Models.PostBase>()
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
            });

            config.AssertConfigurationIsValid();
            Mapper = config.CreateMapper();

            // Register permissions
            foreach (var permission in _permissionNames.Keys)
            {
                App.Permissions["Manager"].Add(new Security.PermissionItem
                {
                    Name = permission,
                    Title = _permissionNames[permission]
                });
            }

            // Get assembly information
            Assembly = this.GetType().GetTypeInfo().Assembly;
            LastModified = new FileInfo(Assembly.Location).LastWriteTime;
        }
    }
}
