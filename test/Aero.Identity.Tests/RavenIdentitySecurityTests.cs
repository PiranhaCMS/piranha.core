using Aero.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Piranha.Manager.LocalAuth;
using Xunit;

namespace Aero.Identity.Tests;

public class RavenIdentitySecurityTests
{
    [Fact]
    public async Task SignIn_ReturnsSucceeded_WhenSignInSucceeds()
    {
        // Arrange
        var mockUserStore = new Mock<IUserStore<RavenUser>>();
        var mockSignInManager = new Mock<SignInManager<RavenUser>>(
            new UserManager<RavenUser>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<RavenUser>>().Object,
            null!, null!, null!, null!);

        mockSignInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var options = Options.Create(new IdentityOptions());
        var security = new RavenIdentitySecurity(mockSignInManager.Object, options);

        // Act
        var result = await security.SignIn(null!, "user", "pass");

        // Assert
        Assert.Equal(LoginResult.Succeeded, result);
    }

    [Fact]
    public async Task SignIn_ReturnsLocked_WhenLockedOut()
    {
        // Arrange
        var mockUserStore = new Mock<IUserStore<RavenUser>>();
        var mockSignInManager = new Mock<SignInManager<RavenUser>>(
            new UserManager<RavenUser>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<RavenUser>>().Object,
            null!, null!, null!, null!);

        mockSignInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        var options = Options.Create(new IdentityOptions());
        var security = new RavenIdentitySecurity(mockSignInManager.Object, options);

        // Act
        var result = await security.SignIn(null!, "user", "pass");

        // Assert
        Assert.Equal(LoginResult.Locked, result);
    }

    [Fact]
    public async Task SignIn_ReturnsFailed_WhenSignInFails()
    {
        // Arrange
        var mockUserStore = new Mock<IUserStore<RavenUser>>();
        var mockSignInManager = new Mock<SignInManager<RavenUser>>(
            new UserManager<RavenUser>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<RavenUser>>().Object,
            null!, null!, null!, null!);

        mockSignInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var options = Options.Create(new IdentityOptions());
        var security = new RavenIdentitySecurity(mockSignInManager.Object, options);

        // Act
        var result = await security.SignIn(null!, "user", "pass");

        // Assert
        Assert.Equal(LoginResult.Failed, result);
    }

    [Fact]
    public async Task SignOut_CallsSignInManagerSignOut()
    {
        // Arrange
        var mockUserStore = new Mock<IUserStore<RavenUser>>();
        var mockSignInManager = new Mock<SignInManager<RavenUser>>(
            new UserManager<RavenUser>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<RavenUser>>().Object,
            null!, null!, null!, null!);

        mockSignInManager.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

        var options = Options.Create(new IdentityOptions());
        var security = new RavenIdentitySecurity(mockSignInManager.Object, options);

        // Act
        await security.SignOut(null!);

        // Assert
        mockSignInManager.Verify(s => s.SignOutAsync(), Times.Once);
    }
}
