using Aero.Identity.Models;
using Xunit;

namespace Aero.Identity.Tests.Models;

public class RavenRoleTests
{
    [Fact]
    public void CanInitializeRavenRole()
    {
        // Arrange & Act
        var role = new RavenRole();

        // Assert
        Assert.NotNull(role);
    }

    [Fact]
    public void CanSetRoleProperties()
    {
        // Arrange
        var role = new RavenRole();
        var roleId = "roles/1";
        var roleName = "Admin";
        var normalizedName = "ADMIN";

        // Act
        role.Id = roleId;
        role.Name = roleName;
        role.NormalizedName = normalizedName;

        // Assert
        Assert.Equal(roleId, role.Id);
        Assert.Equal(roleName, role.Name);
        Assert.Equal(normalizedName, role.NormalizedName);
    }
}
