

using Aero.Cms.Extend;
using Aero.Cms.Manager;
using Aero.Cms.Security;

namespace Aero.Cms.AspNetCore.Identity;

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
    public string Author => "Aero.Cms";

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => "Aero.Cms.AspNetCore.Identity";

    /// <summary>
    /// Gets the Version
    /// </summary>
    public string Version => Aero.Cms.Utils.GetAssemblyVersion(GetType().Assembly);

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description => "Security module for Aero Cms using AspNetCore Identity.";

    /// <summary>
    /// Gets the package url.
    /// </summary>
    public string PackageUrl => "https://www.nuget.org/packages/Aero.Cms.AspNetCore.Identity";

    /// <summary>
    /// Gets the icon url.
    /// </summary>
    public string IconUrl => "https://Aerocms.org/assets/twitter-shield.png";

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
