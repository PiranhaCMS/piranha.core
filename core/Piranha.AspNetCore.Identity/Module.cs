/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Security;
using System.Collections.Generic;

namespace Piranha.AspNetCore.Identity
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class Module : Extend.IModule
    {
        private readonly List<PermissionItem> _permissions = new List<PermissionItem>() {
            new PermissionItem { Name = Permissions.Roles, Title = "List Roles", Category = "Roles"},
            new PermissionItem { Name = Permissions.RolesAdd, Title = "Add Roles", Category = "Roles" },
            new PermissionItem { Name = Permissions.RolesDelete, Title = "Delete Roles", Category = "Roles" },
            new PermissionItem { Name = Permissions.RolesEdit, Title = "Edit Roles", Category = "Roles" },
            new PermissionItem { Name = Permissions.RolesSave, Title = "Save Roles", Category = "Roles" },
            new PermissionItem { Name = Permissions.Users, Title = "List Users", Category = "Users" },
            new PermissionItem { Name = Permissions.UsersAdd, Title = "Add Users", Category = "Users" },
            new PermissionItem { Name = Permissions.UsersDelete, Title = "Delete Users", Category = "Users" },
            new PermissionItem { Name = Permissions.UsersEdit, Title = "Edit Users", Category = "Users" },
            new PermissionItem { Name = Permissions.UsersSave, Title = "Save Users", Category = "Users" }
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
        public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);

        /// <summary>
        /// Gets the release date
        /// </summary>
        public string ReleaseDate => "2018-05-30";

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Security module for Piranha CMS using AspNetCore Identity.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageURL => "https://www.nuget.org/packages/Piranha.AspNetCore.Identity";

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
            Manager.Menu.Items["System"].Items.Insert(0, new Manager.Menu.MenuItem()
            {
                InternalId = "Users",
                Name = "Users",
                Controller = "User",
                Action = "List",
                Policy = Permissions.Users,
                Css = "fas fa-users"
            });
            Manager.Menu.Items["System"].Items.Insert(1, new Manager.Menu.MenuItem()
            {
                InternalId = "Roles",
                Name = "Roles",
                Controller = "Role",
                Action = "List",
                Policy = Permissions.Roles,
                Css = "fas fa-eye-slash"
            });
        }
    }
}