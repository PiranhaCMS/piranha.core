using Aero.Identity.Models;
using Xunit;

namespace Aero.Identity.Tests;

public class RavenRoleStoreTests : RavenTestBase
{
    [Fact]
    public async Task CanCreateRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole { Name = "Admin", NormalizedName = "ADMIN" };

        // Act
        var result = await roleStore.CreateAsync(role, CancellationToken.None);
        await session.SaveChangesAsync();

        // Assert
        Assert.True(result.Succeeded);
        
        using var assertSession = store.OpenAsyncSession();
        var dbRole = await assertSession.LoadAsync<RavenRole>(role.Id);
        Assert.NotNull(dbRole);
        Assert.Equal("Admin", dbRole.Name);
    }

    [Fact]
    public async Task CanFindRoleById()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var role = new RavenRole { Name = "Admin" };
        await session.StoreAsync(role);
        await session.SaveChangesAsync();

        var roleStore = new RavenRoleStore<RavenRole>(session);

        // Act
        var dbRole = await roleStore.FindByIdAsync(role.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(dbRole);
        Assert.Equal("Admin", dbRole.Name);
    }

    [Fact]
    public async Task CanFindRoleByName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var role = new RavenRole { Name = "Admin", NormalizedName = "ADMIN" };
        await session.StoreAsync(role);
        await session.SaveChangesAsync();

        var roleStore = new RavenRoleStore<RavenRole>(session);

        // Act
        var dbRole = await roleStore.FindByNameAsync("ADMIN", CancellationToken.None);

        // Assert
        Assert.NotNull(dbRole);
        Assert.Equal(role.Id, dbRole.Id);
    }

    [Fact]
    public async Task CanDeleteRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var role = new RavenRole { Name = "DeleteMe" };
        await session.StoreAsync(role);
        await session.SaveChangesAsync();

        var roleStore = new RavenRoleStore<RavenRole>(session);

        // Act
        var result = await roleStore.DeleteAsync(role, CancellationToken.None);
        await session.SaveChangesAsync();

        // Assert
        Assert.True(result.Succeeded);
        using var assertSession = store.OpenAsyncSession();
        var dbRole = await assertSession.LoadAsync<RavenRole>(role.Id);
        Assert.Null(dbRole);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole { Name = "Admin" };

        // Act
        var result = await roleStore.UpdateAsync(role, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateAsync_ThrowsOnNullRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => roleStore.CreateAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsOnNullRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => roleStore.UpdateAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsOnNullRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => roleStore.DeleteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetRoleIdAsync_ReturnsRoleId()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole { Id = "roles/1" };

        // Act
        var result = await roleStore.GetRoleIdAsync(role, CancellationToken.None);

        // Assert
        Assert.Equal("roles/1", result);
    }

    [Fact]
    public async Task GetRoleNameAsync_ReturnsRoleName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole { Name = "Admin" };

        // Act
        var result = await roleStore.GetRoleNameAsync(role, CancellationToken.None);

        // Assert
        Assert.Equal("Admin", result);
    }

    [Fact]
    public async Task SetRoleNameAsync_SetsRoleName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole();

        // Act
        await roleStore.SetRoleNameAsync(role, "Admin", CancellationToken.None);

        // Assert
        Assert.Equal("Admin", role.Name);
    }

    [Fact]
    public async Task GetNormalizedRoleNameAsync_ReturnsNormalizedRoleName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole { NormalizedName = "ADMIN" };

        // Act
        var result = await roleStore.GetNormalizedRoleNameAsync(role, CancellationToken.None);

        // Assert
        Assert.Equal("ADMIN", result);
    }

    [Fact]
    public async Task SetNormalizedRoleNameAsync_SetsNormalizedRoleName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var roleStore = new RavenRoleStore<RavenRole>(session);
        var role = new RavenRole();

        // Act
        await roleStore.SetNormalizedRoleNameAsync(role, "ADMIN", CancellationToken.None);

        // Assert
        Assert.Equal("ADMIN", role.NormalizedName);
    }
}
