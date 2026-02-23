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
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var stamp = Guid.NewGuid().ToString();

        // Act
        await userStore.SetSecurityStampAsync(user, stamp, CancellationToken.None);

        // Assert
        Assert.Equal(stamp, user.SecurityStamp);
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
    public async Task CanSetEmail()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var email = "test@example.com";

        // Act
        await userStore.SetEmailAsync(user, email, CancellationToken.None);

        // Assert
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public async Task CanGetEmail()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var email = "test@example.com";
        var user = new RavenUser { Email = email };

        // Act
        var result = await userStore.GetEmailAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(email, result);
    }

    [Fact]
    public async Task CanSetEmailConfirmed()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        await userStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);

        // Assert
        Assert.True(user.EmailConfirmed);
    }

    [Fact]
    public async Task CanGetEmailConfirmed()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { EmailConfirmed = true };

        // Act
        var result = await userStore.GetEmailConfirmedAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanFindByEmail()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user = new RavenUser { UserName = "test", Email = "test@example.com", NormalizedEmail = "TEST@EXAMPLE.COM" };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var dbUser = await userStore.FindByEmailAsync("TEST@EXAMPLE.COM", CancellationToken.None);

        // Assert
        Assert.NotNull(dbUser);
        Assert.Equal(user.Id, dbUser.Id);
    }

    [Fact]
    public async Task CanSetPhoneNumber()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var phone = "123456789";

        // Act
        await userStore.SetPhoneNumberAsync(user, phone, CancellationToken.None);

        // Assert
        Assert.Equal(phone, user.PhoneNumber);
    }

    [Fact]
    public async Task CanGetPhoneNumber()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var phone = "123456789";
        var user = new RavenUser { PhoneNumber = phone };

        // Act
        var result = await userStore.GetPhoneNumberAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(phone, result);
    }

    [Fact]
    public async Task CanSetPhoneNumberConfirmed()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        await userStore.SetPhoneNumberConfirmedAsync(user, true, CancellationToken.None);

        // Assert
        Assert.True(user.PhoneNumberConfirmed);
    }

    [Fact]
    public async Task CanGetPhoneNumberConfirmed()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { PhoneNumberConfirmed = true };

        // Act
        var result = await userStore.GetPhoneNumberConfirmedAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanAddToRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test" };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        // Act
        await userStore.AddToRoleAsync(user, "Admin", CancellationToken.None);

        // Assert
        Assert.Contains("Admin", user.Roles);
    }

    [Fact]
    public async Task CanRemoveFromRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Roles = new List<string> { "Admin" } };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        // Act
        await userStore.RemoveFromRoleAsync(user, "Admin", CancellationToken.None);

        // Assert
        Assert.DoesNotContain("Admin", user.Roles);
    }

    [Fact]
    public async Task CanGetRoles()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var roles = new List<string> { "Admin", "User" };
        var user = new RavenUser { UserName = "test", Roles = roles };

        // Act
        var result = await userStore.GetRolesAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(roles, result);
    }

    [Fact]
    public async Task IsInRole_ReturnsTrueIfInRole()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Roles = new List<string> { "Admin" } };

        // Act
        var result = await userStore.IsInRoleAsync(user, "Admin", CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetUsersInRoleAsync_ReturnsUsers()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user1 = new RavenUser { UserName = "user1", Roles = new List<string> { "Admin" } };
        var user2 = new RavenUser { UserName = "user2", Roles = new List<string> { "User" } };
        await session.StoreAsync(user1);
        await session.StoreAsync(user2);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var result = await userStore.GetUsersInRoleAsync("Admin", CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("user1", result[0].UserName);
    }

    [Fact]
    public async Task CanAddLogin()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test" };
        var login = new UserLoginInfo("Google", "key1", "Google Display");

        // Act
        await userStore.AddLoginAsync(user, login, CancellationToken.None);

        // Assert
        Assert.Single(user.Logins);
        Assert.Equal("Google", user.Logins[0].LoginProvider);
        Assert.Equal("key1", user.Logins[0].ProviderKey);
    }

    [Fact]
    public async Task CanRemoveLogin()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Logins = new List<RavenUserLogin> { new RavenUserLogin { LoginProvider = "Google", ProviderKey = "key1" } } };

        // Act
        await userStore.RemoveLoginAsync(user, "Google", "key1", CancellationToken.None);

        // Assert
        Assert.Empty(user.Logins);
    }

    [Fact]
    public async Task CanGetLogins()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Logins = new List<RavenUserLogin> { new RavenUserLogin { LoginProvider = "Google", ProviderKey = "key1" } } };

        // Act
        var result = await userStore.GetLoginsAsync(user, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Google", result[0].LoginProvider);
    }

    [Fact]
    public async Task CanFindByLogin()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user = new RavenUser { UserName = "test", Logins = new List<RavenUserLogin> { new RavenUserLogin { LoginProvider = "Google", ProviderKey = "key1" } } };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var dbUser = await userStore.FindByLoginAsync("Google", "key1", CancellationToken.None);

        // Assert
        Assert.NotNull(dbUser);
        Assert.Equal(user.Id, dbUser.Id);
    }

    [Fact]
    public async Task CanAddClaims()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test" };
        var claims = new List<System.Security.Claims.Claim> { new System.Security.Claims.Claim("type1", "value1") };

        // Act
        await userStore.AddClaimsAsync(user, claims, CancellationToken.None);

        // Assert
        Assert.Single(user.Claims);
        Assert.Equal("type1", user.Claims[0].ClaimType);
        Assert.Equal("value1", user.Claims[0].ClaimValue);
    }

    [Fact]
    public async Task CanGetClaims()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Claims = new List<RavenUserClaim> { new RavenUserClaim { ClaimType = "type1", ClaimValue = "value1" } } };

        // Act
        var result = await userStore.GetClaimsAsync(user, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("type1", result[0].Type);
        Assert.Equal("value1", result[0].Value);
    }

    [Fact]
    public async Task CanRemoveClaims()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Claims = new List<RavenUserClaim> { new RavenUserClaim { ClaimType = "type1", ClaimValue = "value1" }, new RavenUserClaim { ClaimType = "type2", ClaimValue = "value2" } } };
        var claimsToRemove = new List<System.Security.Claims.Claim> { new System.Security.Claims.Claim("type1", "value1") };

        // Act
        await userStore.RemoveClaimsAsync(user, claimsToRemove, CancellationToken.None);

        // Assert
        Assert.Single(user.Claims);
        Assert.Equal("type2", user.Claims[0].ClaimType);
    }

    [Fact]
    public async Task ReplaceClaim_UpdatesClaim()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { UserName = "test", Claims = new List<RavenUserClaim> { new RavenUserClaim { ClaimType = "type1", ClaimValue = "value1" } } };
        var oldClaim = new System.Security.Claims.Claim("type1", "value1");
        var newClaim = new System.Security.Claims.Claim("type1", "updated");

        // Act
        await userStore.ReplaceClaimAsync(user, oldClaim, newClaim, CancellationToken.None);

        // Assert
        Assert.Single(user.Claims);
        Assert.Equal("updated", user.Claims[0].ClaimValue);
    }

    [Fact]
    public async Task GetUsersForClaimAsync_ReturnsUsers()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user1 = new RavenUser { UserName = "user1", Claims = new List<RavenUserClaim> { new RavenUserClaim { ClaimType = "type1", ClaimValue = "value1" } } };
        var user2 = new RavenUser { UserName = "user2", Claims = new List<RavenUserClaim> { new RavenUserClaim { ClaimType = "type2", ClaimValue = "value2" } } };
        await session.StoreAsync(user1);
        await session.StoreAsync(user2);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var result = await userStore.GetUsersForClaimAsync(new System.Security.Claims.Claim("type1", "value1"), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("user1", result[0].UserName);
    }

    [Fact]
    public async Task CanSetTwoFactorEnabled()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        await userStore.SetTwoFactorEnabledAsync(user, true, CancellationToken.None);

        // Assert
        Assert.True(user.TwoFactorEnabled);
    }

    [Fact]
    public async Task CanGetTwoFactorEnabled()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { TwoFactorEnabled = true };

        // Act
        var result = await userStore.GetTwoFactorEnabledAsync(user, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanSetAuthenticatorKey()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var key = "authkey";

        // Act
        await userStore.SetAuthenticatorKeyAsync(user, key, CancellationToken.None);

        // Assert
        Assert.Equal(key, user.AuthenticatorKey);
    }

    [Fact]
    public async Task CanGetAuthenticatorKey()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var key = "authkey";
        var user = new RavenUser { AuthenticatorKey = key };

        // Act
        var result = await userStore.GetAuthenticatorKeyAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(key, result);
    }

    [Fact]
    public async Task CanReplaceCodes()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var codes = new List<string> { "code1", "code2" };

        // Act
        await userStore.ReplaceCodesAsync(user, codes, CancellationToken.None);

        // Assert
        Assert.Equal("code1;code2", user.RecoveryCodes);
    }

    [Fact]
    public async Task CanRedeemCode()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { RecoveryCodes = "code1;code2" };

        // Act
        var result = await userStore.RedeemCodeAsync(user, "code1", CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal("code2", user.RecoveryCodes);
    }

    [Fact]
    public async Task CanCountCodes()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { RecoveryCodes = "code1;code2" };

        // Act
        var result = await userStore.CountCodesAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CanAddPasskey()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();
        var passkey = new UserPasskeyInfo(new byte[] { 1 }, new byte[] { 2 }, DateTimeOffset.UtcNow, 1, null, false, false, false, new byte[] { 3 }, new byte[] { 4 });

        // Act
        await userStore.AddOrUpdatePasskeyAsync(user, passkey, CancellationToken.None);

        // Assert
        Assert.Single(user.Passkeys);
        Assert.Equal(new byte[] { 1 }, user.Passkeys[0].CredentialId);
    }

    [Fact]
    public async Task CanGetPasskeys()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { Passkeys = new List<PasskeyCredential> { new PasskeyCredential { CredentialId = new byte[] { 1 }, CreatedAt = DateTimeOffset.UtcNow, ClientDataJson = new byte[] { 3 }, AttestationObject = new byte[] { 4 } } } };

        // Act
        var result = await userStore.GetPasskeysAsync(user, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(new byte[] { 1 }, result[0].CredentialId);
    }

    [Fact]
    public async Task CanRemovePasskey()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { Passkeys = new List<PasskeyCredential> { new PasskeyCredential { CredentialId = new byte[] { 1 } } } };

        // Act
        await userStore.RemovePasskeyAsync(user, new byte[] { 1 }, CancellationToken.None);

        // Assert
        Assert.Empty(user.Passkeys);
    }

    [Fact]
    public async Task CanFindByPasskeyId()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var user = new RavenUser { UserName = "test", Passkeys = new List<PasskeyCredential> { new PasskeyCredential { CredentialId = new byte[] { 1 }, CreatedAt = DateTimeOffset.UtcNow, ClientDataJson = new byte[] { 3 }, AttestationObject = new byte[] { 4 } } } };
        await session.StoreAsync(user);
        await session.SaveChangesAsync();

        var userStore = new RavenUserStore<RavenUser>(session);

        // Act
        var dbUser = await userStore.FindByPasskeyIdAsync(new byte[] { 1 }, CancellationToken.None);

        // Assert
        Assert.NotNull(dbUser);
        Assert.Equal(user.Id, dbUser.Id);
    }

    [Fact]
    public async Task CanFindPasskey()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser { Passkeys = new List<PasskeyCredential> { new PasskeyCredential { CredentialId = new byte[] { 1 }, CreatedAt = DateTimeOffset.UtcNow, ClientDataJson = new byte[] { 3 }, AttestationObject = new byte[] { 4 } } } };

        // Act
        var result = await userStore.FindPasskeyAsync(user, new byte[] { 1 }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new byte[] { 1 }, result.CredentialId);
    }

    [Fact]
    public async Task FindPasskey_ReturnsNullIfNotFound()
    {
        // Arrange
        using var store = CreateStore();
        using var session = store.OpenAsyncSession();
        var userStore = new RavenUserStore<RavenUser>(session);
        var user = new RavenUser();

        // Act
        var result = await userStore.FindPasskeyAsync(user, new byte[] { 1 }, CancellationToken.None);

        // Assert
        Assert.Null(result);
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
