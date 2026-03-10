using Alba;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.TestDriver;
using Raven.Embedded;
using Aero.Cms.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Aero.Cms.RavenDb.Extensions;
using Xunit;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Aero.Cms.Manager.IntegrationTests;

public class ManagerTestBase : RavenTestDriver, IAsyncLifetime
{
    protected IAlbaHost Host { get; private set; } = null!;

    static ManagerTestBase()
    {
        ConfigureServer(new TestServerOptions
        {
            Licensing = new ServerOptions.LicensingOptions
            {
                ThrowOnInvalidOrMissingLicense = false
            }
        });
    }

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
                services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
                
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
        
        // IIdentitySeed creates the admin user
        if (seed != null)
        {
            await seed.CreateAsync();
        }
        
        await identityDb.SaveChangesAsync();
        
        // Wait for indexing
        using var session = store.OpenAsyncSession();
        await session.Query<User>().Customize(x => x.WaitForNonStaleResults()).ToListAsync();
        await session.Query<Role>().Customize(x => x.WaitForNonStaleResults()).ToListAsync();
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
