using Alba;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.TestDriver;
using Aero.Cms.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Aero.Cms.RavenDb.Extensions;

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
            builder.ConfigureServices(services =>
            {
                // Register the test store before AddAeroStore so it can be used
                services.AddSingleton<IDocumentStore>(store);
                services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
                
                // Add Aero Store in testing mode to skip its own IDocumentStore registration
                services.AddAeroStore(isTesting: true);
            });
        });

        // Ensure the database is seeded
        using var scope = Host.Services.CreateScope();
        var identityDb = scope.ServiceProvider.GetRequiredService<IIdentityDb>();
        var seed = scope.ServiceProvider.GetService<IIdentitySeed>();
        
        // IdentityDb constructor already calls SeedAsync().GetAwaiter().GetResult()
        // but we might need to call the IIdentitySeed to create the admin user.
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
}
