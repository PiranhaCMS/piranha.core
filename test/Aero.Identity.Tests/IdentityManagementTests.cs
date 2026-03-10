using Aero.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Aero.Cms.AspNetCore.Identity.Data;
using Xunit;

namespace Aero.Identity.Tests;

public class IdentityManagementTests : RavenTestBase
{
    private async Task<(UserManager<User>, RoleManager<Role>, IAsyncDocumentSession, IServiceProvider)> SetupIdentityAsync(IDocumentStore store)
    {
        var services = new ServiceCollection();
        services.AddSingleton(store);
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        services.AddLogging(builder => builder.AddConsole());

        services.AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddRavenDbStores();

        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var session = scope.ServiceProvider.GetRequiredService<IAsyncDocumentSession>();

        return (userManager, roleManager, session, serviceProvider);
    }

    [Fact]
    public async Task UserLifecycle_Create_Read_Update_Delete()
    {
        // Arrange
        using var store = CreateStore();
        var (userManager, _, session, _) = await SetupIdentityAsync(store);

        var userName = "lifecycleuser";
        var email = "lifecycle@example.com";
        var user = new User { UserName = userName, Email = email };

        // 1. Create - Should persist and wait for index due to store fixes
        var createResult = await userManager.CreateAsync(user, "Password123!");
        Assert.True(createResult.Succeeded);

        // 2. Read (Find by Name) - Should work now without manual wait
        var dbUser = await userManager.FindByNameAsync(userName);
        Assert.NotNull(dbUser);
        Assert.Equal(email, dbUser.Email);

        // 3. Update (Email)
        dbUser.Email = "updated@example.com";
        var updateResult = await userManager.UpdateAsync(dbUser);
        Assert.True(updateResult.Succeeded);

        var updatedUser = await userManager.FindByNameAsync(userName);
        Assert.Equal("updated@example.com", updatedUser!.Email);

        // 4. Delete
        var deleteResult = await userManager.DeleteAsync(updatedUser);
        Assert.True(deleteResult.Succeeded);

        var deletedUser = await userManager.FindByNameAsync(userName);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task RoleAndClaimManagement_Lifecycle()
    {
        // Arrange
        using var store = CreateStore();
        var (userManager, roleManager, session, _) = await SetupIdentityAsync(store);

        var roleName = "TestRole";
        var role = new Role { Name = roleName };

        // 1. Create Role
        var roleResult = await roleManager.CreateAsync(role);
        Assert.True(roleResult.Succeeded);

        // 2. Add Claim to Role
        var claim = new System.Security.Claims.Claim("Permission", "ViewDashboard");
        var claimResult = await roleManager.AddClaimAsync(role, claim);
        Assert.True(claimResult.Succeeded);

        // 3. Verify Role and Claim
        var dbRole = await roleManager.FindByNameAsync(roleName);
        Assert.NotNull(dbRole);
        var claims = await roleManager.GetClaimsAsync(dbRole);
        Assert.Contains(claims, c => c.Type == "Permission" && c.Value == "ViewDashboard");

        // 4. Create User and Assign Role
        var user = new User { UserName = "claimuser" };
        await userManager.CreateAsync(user, "Password123!");
        await userManager.AddToRoleAsync(user, roleName);

        // 5. Verify User Role
        var dbUser = await userManager.FindByNameAsync("claimuser");
        Assert.NotNull(dbUser);
        var roles = await userManager.GetRolesAsync(dbUser);
        Assert.Contains(roleName.ToUpperInvariant(), roles);
        
        // Verify claims sync (fixed via generic RavenUserStore)
        Assert.NotEmpty(dbUser.Claims);
        Assert.Contains(dbUser.Claims, c => c.ClaimType == "Permission" && c.ClaimValue == "ViewDashboard");
    }

    [Fact]
    public async Task UserClaims_CanBeAddedAndRemovedDirectly()
    {
        // Arrange
        using var store = CreateStore();
        var (userManager, _, session, _) = await SetupIdentityAsync(store);

        var user = new User { UserName = "directclaimuser" };
        await userManager.CreateAsync(user, "Password123!");

        // 1. Add Claim
        var claim = new System.Security.Claims.Claim("Personal", "Data");
        await userManager.AddClaimAsync(user, claim);

        // 2. Verify
        var dbUser = await userManager.FindByNameAsync("directclaimuser");
        Assert.NotNull(dbUser);
        var claims = await userManager.GetClaimsAsync(dbUser!);
        Assert.Contains(claims, c => c.Type == "Personal" && c.Value == "Data");

        // 3. Remove Claim
        await userManager.RemoveClaimAsync(dbUser!, claim);

        // 4. Verify
        var finalUser = await userManager.FindByNameAsync("directclaimuser");
        var finalClaims = await userManager.GetClaimsAsync(finalUser!);
        Assert.DoesNotContain(finalClaims, c => c.Type == "Personal");
    }
}
