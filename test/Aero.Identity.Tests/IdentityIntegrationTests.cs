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

    [Fact]
    public async Task UserManager_ScaleAndSearchTest()
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

        // Create a role
        await roleManager.CreateAsync(new RavenRole { Name = "PowerUser" });

        // Bulk insert 1000 users
        await using var bulk = store.BulkInsert();
        for (int i = 0; i < 1000; i++)
        {
            await bulk.StoreAsync(new RavenUser 
            { 
                UserName = $"user{i}", 
                Email = $"user{i}@example.com",
                NormalizedUserName = $"USER{i}",
                NormalizedEmail = $"USER{i}@EXAMPLE.COM",
                Roles = new List<string> { i % 10 == 0 ? "POWERUSER" : "USER" }
            });
        }

        // Wait for indexing
        using (var waitSession = store.OpenAsyncSession())
        {
            await waitSession.Query<RavenUser>()
                .Customize(x => x.WaitForNonStaleResults())
                .Take(0)
                .ToListAsync();
        }

        // Add 10 extra users via UserManager
        for (int i = 1000; i < 1010; i++)
        {
            var user = new RavenUser { UserName = $"extra{i}", Email = $"extra{i}@example.com" };
            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, "PowerUser");
        }
        await session.SaveChangesAsync();

        // Wait for indexing again
        using (var waitSession = store.OpenAsyncSession())
        {
            await waitSession.Query<RavenUser>()
                .Customize(x => x.WaitForNonStaleResults())
                .Take(0)
                .ToListAsync();
        }

        // Assert Scale
        var totalUsers = await userManager.Users.CountAsync();
        Assert.Equal(1010, totalUsers);

        // Assert Search/Query
        var powerUsers = await userManager.GetUsersInRoleAsync("PowerUser");
        // 1000/10 = 100 from bulk + 10 extra = 110
        Assert.Equal(110, powerUsers.Count);

        // Update a user
        var userToUpdate = await userManager.FindByNameAsync("user500");
        Assert.NotNull(userToUpdate);
        userToUpdate.Email = "updated500@example.com";
        var updateResult = await userManager.UpdateAsync(userToUpdate);
        await session.SaveChangesAsync();
        Assert.True(updateResult.Succeeded);

        // Delete a user
        var userToDelete = await userManager.FindByNameAsync("user999");
        Assert.NotNull(userToDelete);
        var deleteResult = await userManager.DeleteAsync(userToDelete);
        await session.SaveChangesAsync();
        Assert.True(deleteResult.Succeeded);

        // Final Search check
        var deletedUser = await userManager.FindByNameAsync("user999");
        Assert.Null(deletedUser);

        var updatedUser = await userManager.FindByEmailAsync("updated500@example.com");
        Assert.NotNull(updatedUser);
        Assert.Equal("user500", updatedUser.UserName);
    }
}
