using Piranha;
using Piranha.Extend;
using Piranha.Manager;
using Piranha.Security;

namespace Aero.Identity;

/// <summary>
/// The RavenDB identity module for Piranha CMS.
/// </summary>
public class RavenIdentityModule : IModule
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

    /// <inheritdoc />
    public string Author => "Aero";

    /// <inheritdoc />
    public string Name => "Aero.Identity";

    /// <inheritdoc />
    public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

    /// <inheritdoc />
    public string Description => "Security module for Piranha CMS using RavenDB Identity.";

    /// <inheritdoc />
    public string PackageUrl => "";

    /// <inheritdoc />
    public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

    /// <inheritdoc />
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
