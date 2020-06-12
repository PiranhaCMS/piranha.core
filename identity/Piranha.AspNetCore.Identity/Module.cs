/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using Piranha.Extend;
using Piranha.Manager;
using Piranha.Security;

namespace Piranha.AspNetCore.Identity
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class Module : IModule
    {
        private readonly List<PermissionItem> _permissions = new List<PermissionItem>
        {
            new PermissionItem { Name = Permissions.Roles, Title = "List Roles", Category = "Roles", IsInternal = true },
            new PermissionItem { Name = Permissions.RolesAdd, Title = "Add Roles", Category = "Roles", IsInternal = true },
            new PermissionItem { Name = Permissions.RolesDelete, Title = "Delete Roles", Category = "Roles", IsInternal = true },
            new PermissionItem { Name = Permissions.RolesEdit, Title = "Edit Roles", Category = "Roles", IsInternal = true },
            new PermissionItem { Name = Permissions.RolesSave, Title = "Save Roles", Category = "Roles", IsInternal = true },
            new PermissionItem { Name = Permissions.Users, Title = "List Users", Category = "Users", IsInternal = true },
            new PermissionItem { Name = Permissions.UsersAdd, Title = "Add Users", Category = "Users", IsInternal = true },
            new PermissionItem { Name = Permissions.UsersDelete, Title = "Delete Users", Category = "Users", IsInternal = true },
            new PermissionItem { Name = Permissions.UsersEdit, Title = "Edit Users", Category = "Users", IsInternal = true },
            new PermissionItem { Name = Permissions.UsersSave, Title = "Save Users", Category = "Users", IsInternal = true }
        };

        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha.AspNetCore.Identity";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Security module for Piranha CMS using AspNetCore Identity.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/Piranha.AspNetCore.Identity";

        /// <summary>
        /// Gets the icon url.
        /// </summary>
        public string IconUrl => "http://piranhacms.org/assets/twitter-shield.png";

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init()
        {
            // Register permissions
            foreach (var permission in _permissions)
            {
                App.Permissions["Manager"].Add(permission);
            }

            // Add manager menu items
            Menu.Items["System"].Items.Insert(0, new MenuItem
            {
                InternalId = "Users",
                Name = "Users",
                Route = "~/manager/users",
                Policy = Permissions.Users,
                Css = "fas fa-users"
            });
            Menu.Items["System"].Items.Insert(1, new MenuItem
            {
                InternalId = "Roles",
                Name = "Roles",
                Route = "~/manager/roles",
                Policy = Permissions.Roles,
                Css = "fas fa-eye-slash"
            });
        }
    }
}