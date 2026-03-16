using Aero.Cms;
using Aero.Cms.Manager;
using Xunit;

namespace Aero.Identity.Tests;

public class RavenIdentityModuleTests
{
    [Fact]
    public void Init_RegistersPermissions()
    {
        // Arrange
        var module = new RavenIdentityModule();

        // Act
        module.Init();

        // Assert
        Assert.Contains(App.Permissions["Manager"], p => p.Name == Permissions.Users);
        Assert.Contains(App.Permissions["Manager"], p => p.Name == Permissions.Roles);
    }

    [Fact]
    public void Init_AddsMenuItems()
    {
        // Arrange
        var module = new RavenIdentityModule();

        // Act
        module.Init();

        // Assert
        var systemMenu = Menu.Items["System"];
        Assert.Contains(systemMenu.Items, i => i.InternalId == "Users");
        Assert.Contains(systemMenu.Items, i => i.InternalId == "Roles");
    }
}
