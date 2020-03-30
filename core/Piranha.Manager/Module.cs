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
using System.Reflection;
using Piranha.Extend;
using Piranha.Security;

namespace Piranha.Manager
{
    public sealed class Module : IModule
    {
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
        public string IconUrl => "http://piranhacms.org/assets/twitter-shield.png";

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
