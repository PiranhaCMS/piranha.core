using Raven.Client.Documents;
using Raven.Embedded;
using Raven.TestDriver;
using System.Runtime.CompilerServices;

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

    public IDocumentStore CreateStore([CallerMemberName] string database = "")
    {
        return GetDocumentStore(database: database);
    }
}
