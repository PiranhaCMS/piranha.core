using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Xunit;

namespace Aero.Identity.Tests;

public class IdentityIntegrationTests : RavenTestBase
{
    [Fact]
    public async Task UserManager_CanCreateAndFindUser()
    {
        // Arrange
        using var store = CreateStore();
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        services.AddLogging(builder => builder.AddConsole());
        
        services.AddIdentityCore<RavenUser>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RavenUser>>();
        var session = scope.ServiceProvider.GetRequiredService<IAsyncDocumentSession>();

        var user = new RavenUser { UserName = "integrated", Email = "integrated@example.com" };

        // Act
        var createResult = await userManager.CreateAsync(user, "Password123!");
        await session.SaveChangesAsync();

        // Wait for index
        using (var waitSession = store.OpenAsyncSession())
        {
            await waitSession.Query<RavenUser>()
                .Customize(x => x.WaitForNonStaleResults())
                .FirstOrDefaultAsync(u => u.UserName == "integrated");
        }

        // Assert
        Assert.True(createResult.Succeeded);
        
        var dbUser = await userManager.FindByNameAsync("integrated");
        Assert.NotNull(dbUser);
        Assert.Equal("integrated@example.com", dbUser.Email);
    }

    [Fact]
    public async Task UserManager_CanHandleRoles()
    {
        // Arrange
        using var store = CreateStore();
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        
        services.AddIdentityCore<RavenUser>()
            .AddRoles<RavenRole>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<RavenUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RavenRole>>();
        var session = scope.ServiceProvider.GetRequiredService<IAsyncDocumentSession>();

        var user = new RavenUser { UserName = "roleuser" };
        var role = new RavenRole { Name = "Tester" };

        // Act
        await userManager.CreateAsync(user);
        await roleManager.CreateAsync(role);
        await session.SaveChangesAsync();

        // Wait for index
        using (var waitSession = store.OpenAsyncSession())
        {
            await waitSession.Query<RavenRole>()
                .Customize(x => x.WaitForNonStaleResults())
                .FirstOrDefaultAsync(r => r.Name == "Tester");
        }

        await userManager.AddToRoleAsync(user, "Tester");
        await session.SaveChangesAsync();

        // Wait for index
        using (var waitSession = store.OpenAsyncSession())
        {
            await waitSession.Query<RavenUser>()
                .Customize(x => x.WaitForNonStaleResults())
                .FirstOrDefaultAsync(u => u.UserName == "roleuser");
        }

        // Assert
        var isInRole = await userManager.IsInRoleAsync(user, "Tester");
        Assert.True(isInRole);
        
        var roles = await userManager.GetRolesAsync(user);
        Assert.Contains("TESTER", roles); // UserManager normalizes to uppercase
    }
}
