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
using Piranha.Security;

namespace Piranha.WebApi
{
    public sealed class Module : Extend.IModule
    {
        private readonly List<PermissionItem> _permissions = new List<PermissionItem>
        {
            new PermissionItem { Name = Permissions.Media, Title = "Media Api" },
            new PermissionItem { Name = Permissions.Pages, Title = "Page Api" },
            new PermissionItem { Name = Permissions.Posts, Title = "Post Api" },
            new PermissionItem { Name = Permissions.Sitemap, Title = "Sitemap Api" }
        };

        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha.WebApi";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Web Api module for Piranha.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/Piranha.WebApi";

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
        /// Initializes the module.
        /// </summary>
        public void Init()
        {
            // Register permissions
            foreach (var permission in _permissions)
            {
                App.Permissions["Api"].Add(permission);
            }
        }
    }
}
