using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Xunit;

namespace Aero.Identity.Tests;

public class RavenUserStoreTests : RavenTestBase
{
    [Fact]
    public async Task CanCreateUser()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "testuser", Email = "test@example.com" };

        // Act
        var result = await userStore.CreateAsync(user, CancellationToken.None);
        await session.SaveChangesAsync();

        // Assert
        Assert.True(result.Succeeded);
        
        using var assertSession = store.OpenAsyncSession();
        var dbUser = await assertSession.LoadAsync<RavenUser>(user.Id);
        Assert.NotNull(dbUser);
        Assert.Equal("testuser", dbUser.UserName);
    }

    [Fact]
    public async Task CanFindUserById()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user = new RavenUser { UserName = "testuser" };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var dbUser = await userStore.FindByIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(dbUser);
        Assert.Equal("testuser", dbUser.UserName);
    }

    [Fact]
    public async Task CanFindUserByName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user = new RavenUser { UserName = "testuser", NormalizedUserName = "TESTUSER" };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var dbUser = await userStore.FindByNameAsync("TESTUSER", CancellationToken.None);

        // Assert
        Assert.NotNull(dbUser);
        Assert.Equal(user.Id, dbUser.Id);
    }

    [Fact]
    public async Task CanSetPasswordHash()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var hash = "hashedpassword";

        // Act
        await userStore.SetPasswordHashAsync(user, hash, CancellationToken.None);

        // Assert
        Assert.Equal(hash, user.PasswordHash);
    }

    [Fact]
    public async Task CanGetPasswordHash()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var hash = "hashedpassword";
        var user = new RavenUser { PasswordHash = hash };

        // Act
        var result = await userStore.GetPasswordHashAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(hash, result);
    }

    [Fact]
    public async Task CanSetSecurityStamp()
    {
        // ... (previous content)
    }

    [Fact]
    public async Task GetUserIdAsync_ReturnsUserId()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { Id = "users/1" };

        // Act
        var result = await userStore.GetUserIdAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal("users/1", result);
    }

    [Fact]
    public async Task GetUserNameAsync_ReturnsUserName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test" };

        // Act
        var result = await userStore.GetUserNameAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal("test", result);
    }

    [Fact]
    public async Task SetUserNameAsync_SetsUserName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        await userStore.SetUserNameAsync(user, "test", CancellationToken.None);

        // Assert
        Assert.Equal("test", user.UserName);
    }

    [Fact]
    public async Task GetNormalizedUserNameAsync_ReturnsNormalizedUserName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { NormalizedUserName = "TEST" };

        // Act
        var result = await userStore.GetNormalizedUserNameAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal("TEST", result);
    }

    [Fact]
    public async Task SetNormalizedUserNameAsync_SetsNormalizedUserName()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        await userStore.SetNormalizedUserNameAsync(user, "TEST", CancellationToken.None);

        // Assert
        Assert.Equal("TEST", user.NormalizedUserName);
    }

    [Fact]
    public async Task HasPasswordAsync_ReturnsTrueIfPasswordSet()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { PasswordHash = "hash" };

        // Act
        var result = await userStore.HasPasswordAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasPasswordAsync_ReturnsFalseIfPasswordNotSet()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        var result = await userStore.HasPasswordAsync(user, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetSecurityStampAsync_ReturnsSecurityStamp()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { SecurityStamp = "stamp" };

        // Act
        var result = await userStore.GetSecurityStampAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal("stamp", result);
    }

    [Fact]
    public async Task CanDeleteUser()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user = new RavenUser { UserName = "delete-me" };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var result = await userStore.DeleteAsync(user, CancellationToken.None);
        await session.SaveChangesAsync();

        // Assert
        Assert.True(result.Succeeded);
        using var assertSession = store.OpenAsyncSession();
        var dbUser = await assertSession.LoadAsync<RavenUser>(user.Id);
        Assert.Null(dbUser);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test" };

        // Act
        var result = await userStore.UpdateAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task CreateAsync_ThrowsOnNullUser()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => userStore.CreateAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsOnNullUser()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => userStore.UpdateAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsOnNullUser()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => userStore.DeleteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ThrowsOnCancelledToken()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => userStore.CreateAsync(user, cts.Token));
    }

    [Fact]
    public async Task Concurrency_Update_Fails_When_ETag_Mismatch()
    {
        // Arrange
        using var store = CreateStore();
        
        // Create user
        string userId;
        using (var session = store.OpenAsyncSession())
        {
            var user = new RavenUser { UserName = "concurrent" };
            await session.StoreAsync(user);
            await session.SaveChangesAsync();
            userId = user.Id;
        }

        // Load in two different sessions
        using var session1 = store.OpenAsyncSession();
        using var session2 = store.OpenAsyncSession();
        session1.Advanced.UseOptimisticConcurrency = true;
        session2.Advanced.UseOptimisticConcurrency = true;

        var user1 = await session1.LoadAsync<RavenUser>(userId);
        var user2 = await session2.LoadAsync<RavenUser>(userId);

        // Update in session 1
        var userStore1 = new RavenUserStore<RavenUser>(session1);
        user1.UserName = "updated1";
        await userStore1.UpdateAsync(user1, CancellationToken.None);
        await session1.SaveChangesAsync();

        // Update in session 2 (should fail on SaveChangesAsync, not UpdateAsync)
        var userStore2 = new RavenUserStore<RavenUser>(session2);
        user2.UserName = "updated2";
        await userStore2.UpdateAsync(user2, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<Raven.Client.Exceptions.ConcurrencyException>(() => session2.SaveChangesAsync());
    }
}
