

using Xunit;

namespace Aero.Cms.Tests;

public class Permissions
{
    private readonly Aero.Cms.Security.PermissionManager mgr;

    public Permissions() {
        mgr = new Aero.Cms.Security.PermissionManager();

        mgr["Module1"].Add(new Aero.Cms.Security.PermissionItem {
            Name = "Module1Permission1", Title = "Module1Permission1"
        });
        mgr["Module1"].Add(new Aero.Cms.Security.PermissionItem {
            Name = "Module1Permission2", Title = "Module1Permission2"
        });

        mgr["Module2"].Add(new Aero.Cms.Security.PermissionItem {
            Name = "Module2Permission1", Title = "Module2Permission1"
        });
        mgr["Module2"].Add(new Aero.Cms.Security.PermissionItem {
            Name = "Module2Permission2", Title = "Module2Permission2"
        });
        mgr["Module2"].Add(new Aero.Cms.Security.PermissionItem {
            Name = "Module2Permission3", Title = "Module2Permission3"
        });
    }

    [Fact]
    public void PermissionCount() {
        var permissions = Aero.Cms.Security.Permission.All();

        Assert.Equal(2, permissions.Length);
    }

    [Fact]
    public void AllHasPagePreview() {
        var permissions = Aero.Cms.Security.Permission.All();

        Assert.Contains(permissions, s => s == Aero.Cms.Security.Permission.PagePreview);
    }

    [Fact]
    public void AllHasPostPreview() {
        var permissions = Aero.Cms.Security.Permission.All();

        Assert.Contains(permissions, s => s == Aero.Cms.Security.Permission.PostPreview);
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
