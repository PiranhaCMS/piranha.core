using Alba;
using Aero.Cms.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Aero.Cms.Data.Extensions;
using Marten;
using Xunit;
using Microsoft.AspNetCore.Antiforgery;

namespace Aero.Cms.Manager.IntegrationTests;

public class ManagerTestBase : AeroDbTestDriver, IAsyncLifetime
{
    protected IAlbaHost Host { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var store = GetDocumentStore();

        Host = await AlbaHost.For<Program>(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Register the test store before AddAeroStore so it can be used
                services.AddSingleton<IDocumentStore>(store);
                services.AddScoped(s => s.GetRequiredService<IDocumentStore>().LightweightSession());
                
                // Add Aero Store in testing mode to skip its own IDocumentStore registration
                services.AddAeroStore(isTesting: true);

                // Disable Antiforgery for tests by providing a mock/no-op implementation
                services.AddSingleton<IAntiforgery, NoOpAntiforgery>();
            });
        });

        // Ensure the database is seeded with identity data
        using var scope = Host.Services.CreateScope();
        var identityDb = scope.ServiceProvider.GetRequiredService<IIdentityDb>();
        var seed = scope.ServiceProvider.GetService<IIdentitySeed>();
        var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Aero.Cms.AspNetCore.Identity.Data.Role>>();
        
        // Reset initialization flag so seeding runs for the next test
        Aero.Cms.Data.AeroDbBase.IsInitialized = false;

        // Ensure roles exist
        if (!await roleManager.RoleExistsAsync("SysAdmin"))
        {
            await roleManager.CreateAsync(new Aero.Cms.AspNetCore.Identity.Data.Role { Name = "SysAdmin" });
        }
        if (!await roleManager.RoleExistsAsync("AeroAdmin"))
        {
            await roleManager.CreateAsync(new Aero.Cms.AspNetCore.Identity.Data.Role { Name = "AeroAdmin" });
        }

        // IIdentitySeed creates the admin user
        if (seed != null)
        {
            await seed.CreateAsync();
        }
        
        await identityDb.SaveChangesAsync();
        
        // Wait for indexing
        using var session = store.LightweightSession();
        await session.Query<User>().ToListAsync();
        await session.Query<Role>().ToListAsync();
    }

    public async Task DisposeAsync()
    {
        if (Host != null)
        {
            await Host.DisposeAsync();
        }
        Dispose();
    }

    private class NoOpAntiforgery : IAntiforgery
    {
        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext) => new AntiforgeryTokenSet("token", "cookie", "form", "header");
        public AntiforgeryTokenSet GetTokens(HttpContext httpContext) => new AntiforgeryTokenSet("token", "cookie", "form", "header");
        public Task<bool> IsRequestValidAsync(HttpContext httpContext) => Task.FromResult(true);
        public void SetCookieTokenAndHeader(HttpContext httpContext) { }
        public Task ValidateRequestAsync(HttpContext httpContext) => Task.CompletedTask;
    }
}
