using System;

namespace Piranha.Manager
{
    public record PermissionsStructure
    {
        public PermissionsStructure(string permissionName, PermissionsStructure[] childPermissions)
            : this(permissionName)
        {
            ChildPermissions = childPermissions;
        }

        public PermissionsStructure(string domainName)
        {
            PermissionName = domainName;
        }

        public string PermissionName { get; }
        public PermissionsStructure[] ChildPermissions { get; } = Array.Empty<PermissionsStructure>();
    }
}
