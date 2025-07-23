/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager;

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

