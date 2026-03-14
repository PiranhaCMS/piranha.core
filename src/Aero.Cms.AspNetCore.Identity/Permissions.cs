

namespace Aero.Cms.AspNetCore.Identity;

/// <summary>
/// The available identity permissions.
/// </summary>
public static class Permissions
{
    public const string Roles = "AeroRoles";
    public const string RolesAdd = "AeroRolesAdd";
    public const string RolesDelete = "AeroRolesDelete";
    public const string RolesEdit = "AeroRolesEdit";
    public const string RolesSave = "AeroRolesSave";
    public const string Users = "AeroUsers";
    public const string UsersAdd = "AeroUsersAdd";
    public const string UsersDelete = "AeroUsersDelete";
    public const string UsersEdit = "AeroUsersEdit";
    public const string UsersSave = "AeroUsersSave";

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
