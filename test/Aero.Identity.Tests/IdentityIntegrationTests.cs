using Aero.Identity.Models;
using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


using Xunit;

namespace Aero.Identity.Tests;

public class IdentityIntegrationTests : AeroDbTestDriver
{
    [Fact]
    public async Task UserManager_CanCreateAndFindUser()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().LightweightSession());
        services.AddLogging(builder => builder.AddConsole());

        services.AddIdentityCore<AeroUser>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AeroUser>>();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var user = new AeroUser { UserName = "integrated", Email = "integrated@example.com" };

        // Act
        var createResult = await userManager.CreateAsync(user, "Password123!");
        await session.SaveChangesAsync();

        // Wait for index
        using (var waitSession = store.LightweightSession())
        {
            await waitSession.Query<AeroUser>()
                
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
        
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().LightweightSession());

        services.AddIdentityCore<AeroUser>()
            .AddRoles<RavenRole>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AeroUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RavenRole>>();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var user = new AeroUser { UserName = "roleuser" };
        var role = new RavenRole { Name = "Tester" };

        // Act
        await userManager.CreateAsync(user);
        await roleManager.CreateAsync(role);
        await session.SaveChangesAsync();

        // Wait for index
        using (var waitSession = store.LightweightSession())
        {
            await waitSession.Query<RavenRole>()
                
                .FirstOrDefaultAsync(r => r.Name == "Tester");
        }

        await userManager.AddToRoleAsync(user, "Tester");
        await session.SaveChangesAsync();

        // Wait for index
        using (var waitSession = store.LightweightSession())
        {
            await waitSession.Query<AeroUser>()
                
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
        
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().LightweightSession());

        services.AddIdentityCore<AeroUser>()
            .AddRoles<RavenRole>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AeroUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RavenRole>>();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        // Create a role
        await roleManager.CreateAsync(new RavenRole { Name = "PowerUser" });



        // Bulk insert 1000 users
        //await using (var bulk = store.BulkInsert())
        var docs = new List<AeroUser>();
        {
            for (int i = 0; i < 1000; i++)
            {
                docs.Add(new AeroUser
                {
                    UserName = $"user{i}",
                    Email = $"user{i}@example.com",
                    NormalizedUserName = $"USER{i}",
                    NormalizedEmail = $"USER{i}@EXAMPLE.COM",
                    Roles = new List<string> { i % 10 == 0 ? "POWERUSER" : "USER" }
                });
            }

            await store.BulkInsertDocumentsAsync(docs);
        }

        // Wait for indexing
        using (var waitSession = store.LightweightSession())
        {
            await waitSession.Query<AeroUser>()
                
                .Take(0)
                .ToListAsync();
        }

        // Add 10 extra users via UserManager
        for (int i = 1000; i < 1010; i++)
        {
            var user = new AeroUser { UserName = $"extra{i}", Email = $"extra{i}@example.com" };
            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, "PowerUser");
        }

        await session.SaveChangesAsync();

        // Wait for indexing again
        using (var waitSession = store.LightweightSession())
        {
            await waitSession.Query<AeroUser>()
                
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

    [Fact]
    public async Task UserManager_CanFindUsersInMultipleRoles()
    {
        // Arrange
        
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().LightweightSession());

        services.AddIdentityCore<AeroUser>()
            .AddRoles<RavenRole>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AeroUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RavenRole>>();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var roleNames = new[] { "RoleA", "RoleB", "RoleC" };
        foreach (var roleName in roleNames)
        {
            await roleManager.CreateAsync(new RavenRole { Name = roleName });
        }

        // Create 25 users. 
        // RoleA gets 15 users (0-14)
        // RoleB gets 15 users (5-19)
        // RoleC gets 15 users (10-24)
        // Users 10-14 will be in all three roles.
        var expectedUsersInAllRoles = new List<string>();

        for (int i = 0; i < 25; i++)
        {
            var user = new AeroUser { UserName = $"intersection_user_{i}", Email = $"intersection{i}@example.com" };
            await userManager.CreateAsync(user, "Password123!");

            if (i >= 0 && i < 15) await userManager.AddToRoleAsync(user, "RoleA");
            if (i >= 5 && i < 20) await userManager.AddToRoleAsync(user, "RoleB");
            if (i >= 10 && i < 25) await userManager.AddToRoleAsync(user, "RoleC");

            if (i >= 10 && i < 15)
            {
                expectedUsersInAllRoles.Add(user.UserName);
            }
        }

        await session.SaveChangesAsync();

        // Wait for indexing
        using (var waitSession = store.LightweightSession())
        {
            await waitSession.Query<AeroUser>()
                
                .FirstOrDefaultAsync(u => u.UserName == "intersection_user_0");
        }

        // Act
        // Find users who belong to all three roles
        var query = session.Query<AeroUser>()
            
            .Where(u => u.Roles.Contains("ROLEA") && u.Roles.Contains("ROLEB") && u.Roles.Contains("ROLEC"));

        var usersInAllRoles = await query.ToListAsync();

        // Assert
        Assert.Equal(5, usersInAllRoles.Count);
        foreach (var expectedUserName in expectedUsersInAllRoles)
        {
            Assert.Contains(usersInAllRoles, u => u.UserName == expectedUserName);
        }
    }
}
