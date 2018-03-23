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
        private readonly Dictionary<string, string> _permissionNames = new Dictionary<string, string>()
        {
            { Permissions.Roles, "Roles"},
            { Permissions.RolesAdd, "Roles - Add"},
            { Permissions.RolesDelete, "Roles - Delete"},
            { Permissions.RolesEdit, "Roles - Edit"},
            { Permissions.RolesSave, "Roles - Save"},
            { Permissions.Users, "Users"},
            { Permissions.UsersAdd, "Users - Add"},
            { Permissions.UsersDelete, "Users - Delete"},
            { Permissions.UsersEdit, "Users - Edit"},
            { Permissions.UsersSave, "Users - Save"}
        };

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init()
        {
            // Register permissions
            foreach (var permission in _permissionNames.Keys)
            {
                App.Permissions["Manager"].Add(new PermissionItem
                {
                    Name = permission,
                    Title = _permissionNames[permission]
                });
            }

            // Add manager menu items
            Piranha.Manager.Menu.Items["System"].Items.Add(new Manager.Menu.MenuItem()
            {
                InternalId = "Users",
                Name = "Users",
                Controller = "User",
                Action = "List",
                Policy = Permissions.Users,
                Css = "glyphicon glyphicon-user"
            });
            Piranha.Manager.Menu.Items["System"].Items.Add(new Manager.Menu.MenuItem()
            {
                InternalId = "Roles",
                Name = "Roles",
                Controller = "Role",
                Action = "List",
                Policy = Permissions.Roles,
                Css = "glyphicon glyphicon-eye-close"
            });
        }
    }
}