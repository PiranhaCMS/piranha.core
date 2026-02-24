namespace Aero.Identity;

/// <summary>
/// The available identity permissions.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Permission to list roles.
    /// </summary>
    public const string Roles = "PiranhaRoles";

    /// <summary>
    /// Permission to add roles.
    /// </summary>
    public const string RolesAdd = "PiranhaRolesAdd";

    /// <summary>
    /// Permission to delete roles.
    /// </summary>
    public const string RolesDelete = "PiranhaRolesDelete";

    /// <summary>
    /// Permission to edit roles.
    /// </summary>
    public const string RolesEdit = "PiranhaRolesEdit";

    /// <summary>
    /// Permission to save roles.
    /// </summary>
    public const string RolesSave = "PiranhaRolesSave";

    /// <summary>
    /// Permission to list users.
    /// </summary>
    public const string Users = "PiranhaUsers";

    /// <summary>
    /// Permission to add users.
    /// </summary>
    public const string UsersAdd = "PiranhaUsersAdd";

    /// <summary>
    /// Permission to delete users.
    /// </summary>
    public const string UsersDelete = "PiranhaUsersDelete";

    /// <summary>
    /// Permission to edit users.
    /// </summary>
    public const string UsersEdit = "PiranhaUsersEdit";

    /// <summary>
    /// Permission to save users.
    /// </summary>
    public const string UsersSave = "PiranhaUsersSave";

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
