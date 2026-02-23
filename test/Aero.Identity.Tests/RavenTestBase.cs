using Raven.Client.Documents;
using Raven.Embedded;
using Raven.TestDriver;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aero.Identity.Tests;

public class RavenTestBase : RavenTestDriver
{
    static RavenTestBase()
    {
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
