using Raven.Client.Documents;
using Raven.Embedded;
using Raven.TestDriver;
using System.Runtime.CompilerServices;
using Raven.Client.Documents.Operations.Revisions;

namespace Piranha.Tests;

public class RavenTestBase : RavenTestDriver
{
    static RavenTestBase()
    {
        try {
            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                Licensing = new ServerOptions.LicensingOptions
                {
                    ThrowOnInvalidOrMissingLicense = false
                }
            });
        } catch (InvalidOperationException) { }

        ConfigureServer(new TestServerOptions
        {
            Licensing = new ServerOptions.LicensingOptions
            {
                ThrowOnInvalidOrMissingLicense = false
            }
        });
    }

    protected override void PreInitialize(IDocumentStore store)
    {
        store.Conventions.MaxNumberOfRequestsPerSession = 100;
    }

    protected IDocumentStore CreateStore([CallerMemberName] string database = "aero-test")
    {
        return GetDocumentStore(database: database);
    }
    
    protected override void SetupDatabase(IDocumentStore documentStore)
    {
        documentStore.Maintenance.Send(new ConfigureRevisionsOperation(new RevisionsConfiguration
        {
            Default = new RevisionsCollectionConfiguration
            {
                Disabled = false,
                PurgeOnDelete = true,
                MinimumRevisionsToKeep = 1,
                MinimumRevisionAgeToKeep = TimeSpan.FromDays(14),
            }
        }));
    }
}
