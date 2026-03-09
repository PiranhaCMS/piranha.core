

namespace Aero.Cms.Security;

/// <summary>
/// The permission manager.
/// </summary>
public class PermissionManager
{
    private readonly Dictionary<string, List<PermissionItem>> _modules = new Dictionary<string, List<PermissionItem>>();

    /// <summary>
    /// Gets the permission items for the given module.
    /// </summary>
    public List<PermissionItem> this[string module]
    {
        get
        {
            if (!_modules.TryGetValue(module, out var items))
            {
                _modules[module] = items = new List<PermissionItem>();
            }
            return items;
        }
    }

    /// <summary>
    /// Gets the registered permission modules.
    /// </summary>
    /// <returns>The module names</returns>
    public List<string> GetModules()
    {
        return _modules.Keys.OrderBy(k => k).ToList();
    }

    /// <summary>
    /// Gets the permissions for the given module.
    /// </summary>
    /// <param name="module">The module name</param>
    /// <returns>The available permissions</returns>
    public List<PermissionItem> GetPermissions(string module)
    {
        return this[module].OrderBy(p => p.Name).ToList();
    }

    /// <summary>
    /// Gets all of the available permissions.
    /// </summary>
    /// <returns>The available permissions</returns>
    public List<PermissionItem> GetPermissions()
    {
        var all = new Dictionary<string, PermissionItem>();

        foreach (var module in GetModules())
        {
            foreach (var permission in GetPermissions(module))
            {
                all[permission.Name] = permission;
            }
        }
        return all.Values.OrderBy(k => k.Name).ToList();
    }

    /// <summary>
    /// Gets all of available permissions that is not internal.
    /// </summary>
    /// <returns>The available permissions</returns>
    public List<PermissionItem> GetPublicPermissions()
    {
        var all = new Dictionary<string, PermissionItem>();

        foreach (var module in GetModules())
        {
            foreach (var permission in GetPermissions(module).Where(p => !p.IsInternal))
            {
                all[permission.Name] = permission;
            }
        }
        return all.Values.OrderBy(k => k.Name).ToList();
    }
}
