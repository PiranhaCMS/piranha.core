/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

namespace Piranha.AspNetCore.Identity
{
    /// <summary>
    /// The available identity permissions.
    /// </summary>
    public static class Permissions
    {
        public const string Roles = "PiranhaRoles";
        public const string RolesAdd = "PiranhaRolesAdd";
        public const string RolesDelete = "PiranhaRolesDelete";
        public const string RolesEdit = "PiranhaRolesEdit";
        public const string RolesSave = "PiranhaRolesSave";
        public const string Users = "PiranhaUsers";
        public const string UsersAdd = "PiranhaUsersAdd";
        public const string UsersDelete = "PiranhaUsersDelete";
        public const string UsersEdit = "PiranhaUsersEdit";
        public const string UsersSave = "PiranhaUsersSave";

        public static string[] All()
        {
            return new[]
            {
                Roles,
                RolesAdd,
                RolesDelete,
                RolesEdit,
                RolesSave,
                Users,
                UsersAdd,
                UsersDelete,
                UsersEdit,
                UsersSave
            };
        }
    }
}