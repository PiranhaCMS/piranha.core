using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using Moq;
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
}
