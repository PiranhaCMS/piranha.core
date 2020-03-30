/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Xunit;

namespace Piranha.Tests
{
    public class Permissions
    {
        private readonly Piranha.Security.PermissionManager mgr;

        public Permissions() {
            mgr = new Piranha.Security.PermissionManager();

            mgr["Module1"].Add(new Piranha.Security.PermissionItem {
                Name = "Module1Permission1", Title = "Module1Permission1"
            });
            mgr["Module1"].Add(new Piranha.Security.PermissionItem {
                Name = "Module1Permission2", Title = "Module1Permission2"
            });

            mgr["Module2"].Add(new Piranha.Security.PermissionItem {
                Name = "Module2Permission1", Title = "Module2Permission1"
            });
            mgr["Module2"].Add(new Piranha.Security.PermissionItem {
                Name = "Module2Permission2", Title = "Module2Permission2"
            });
            mgr["Module2"].Add(new Piranha.Security.PermissionItem {
                Name = "Module2Permission3", Title = "Module2Permission3"
            });
        }

        [Fact]
        public void PermissionCount() {
            var permissions = Piranha.Security.Permission.All();

            Assert.Equal(2, permissions.Length);
        }

        [Fact]
        public void AllHasPagePreview() {
            var permissions = Piranha.Security.Permission.All();

            Assert.Contains(permissions, s => s == Piranha.Security.Permission.PagePreview);
        }

        [Fact]
        public void AllHasPostPreview() {
            var permissions = Piranha.Security.Permission.All();

            Assert.Contains(permissions, s => s == Piranha.Security.Permission.PostPreview);
        }

        [Fact]
        public void GetPermissionManagerModuleCount() {
            Assert.Equal(2, mgr.GetModules().Count);
        }

        [Fact]
        public void GetPermissionManagerCount() {
            Assert.Equal(2, mgr.GetPermissions("Module1").Count);
            Assert.Equal(3, mgr.GetPermissions("Module2").Count);
            Assert.Equal(5, mgr.GetPermissions().Count);
        }
    }
}