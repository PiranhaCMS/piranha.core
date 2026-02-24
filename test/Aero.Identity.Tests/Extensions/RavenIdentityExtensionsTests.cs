using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using Moq;
using Piranha;
using Piranha.Manager.LocalAuth;
using Xunit;

namespace Aero.Identity.Tests.Extensions;

public class RavenIdentityExtensionsTests
{
    [Fact]
    public void AddRavenDbIdentity_RegistersStores()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockSession = new Mock<IAsyncDocumentSession>();
        services.AddScoped(_ => mockSession.Object);

        // Act
        services.AddIdentityCore<RavenUser>()
            .AddRoles<RavenRole>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userStore = serviceProvider.GetService<IUserStore<RavenUser>>();
        var roleStore = serviceProvider.GetService<IRoleStore<RavenRole>>();

        Assert.NotNull(userStore);
        Assert.IsType<RavenUserStore<RavenUser>>(userStore);
        Assert.NotNull(roleStore);
        Assert.IsType<RavenRoleStore<RavenRole>>(roleStore);
    }

    [Fact]
    public void AddRavenDbIdentity_WithoutRoles_RegistersUserStoreOnly()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockSession = new Mock<IAsyncDocumentSession>();
        services.AddScoped(_ => mockSession.Object);

        // Act
        services.AddIdentityCore<RavenUser>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userStore = serviceProvider.GetService<IUserStore<RavenUser>>();
        var roleStore = serviceProvider.GetService<IRoleStore<RavenRole>>();

        Assert.NotNull(userStore);
        Assert.IsType<RavenUserStore<RavenUser>>(userStore);
        Assert.Null(roleStore);
    }

    [Fact]
    public void AddPiranhaRavenDbIdentity_RegistersEverything()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockStore = new Mock<Raven.Client.Documents.IDocumentStore>();
        var mockSession = new Mock<IAsyncDocumentSession>();
        services.AddSingleton(mockStore.Object);
        services.AddScoped(_ => mockSession.Object);

        // Act
        services.AddPiranhaRavenDbIdentity();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userStore = serviceProvider.GetService<IUserStore<RavenUser>>();
        var security = serviceProvider.GetService<ISecurity>();

        Assert.NotNull(userStore);
        Assert.IsType<RavenUserStore<RavenUser>>(userStore);
        Assert.NotNull(security);
        Assert.IsType<RavenIdentitySecurity>(security);

        // Verify module registration
        Assert.True(App.Modules.Any(m => m.Instance is RavenIdentityModule));
    }
}
