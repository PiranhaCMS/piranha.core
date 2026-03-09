namespace Aero.Identity;

/// <summary>
/// The available identity permissions.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Permission to list roles.
    /// </summary>
    public const string Roles = "AeroRoles";

    /// <summary>
    /// Permission to add roles.
    /// </summary>
    public const string RolesAdd = "AeroRolesAdd";

    /// <summary>
    /// Permission to delete roles.
    /// </summary>
    public const string RolesDelete = "AeroRolesDelete";

    /// <summary>
    /// Permission to edit roles.
    /// </summary>
    public const string RolesEdit = "AeroRolesEdit";

    /// <summary>
    /// Permission to save roles.
    /// </summary>
    public const string RolesSave = "AeroRolesSave";

    /// <summary>
    /// Permission to list users.
    /// </summary>
    public const string Users = "AeroUsers";

    /// <summary>
    /// Permission to add users.
    /// </summary>
    public const string UsersAdd = "AeroUsersAdd";

    /// <summary>
    /// Permission to delete users.
    /// </summary>
    public const string UsersDelete = "AeroUsersDelete";

    /// <summary>
    /// Permission to edit users.
    /// </summary>
    public const string UsersEdit = "AeroUsersEdit";

    /// <summary>
    /// Permission to save users.
    /// </summary>
    public const string UsersSave = "AeroUsersSave";

    /// <summary>
    /// Gets all available identity permissions.
    /// </summary>
    /// <returns>The permissions.</returns>
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
