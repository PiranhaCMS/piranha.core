using Raven.Client.Documents;
using Raven.Embedded;
using Raven.TestDriver;
using System.Runtime.CompilerServices;
using Aero.Cms.RavenDb;

namespace Aero.Cms.Tests;

public class RavenTestBase : RavenTestDriver
{
    static RavenTestBase()
    {
        ConfigureServer(new TestServerOptions
        {
            ServerUrl = "http://localhost:5150",
            Licensing = new ServerOptions.LicensingOptions
            {
                ThrowOnInvalidOrMissingLicense = false
            }
        });
    }

    protected override void PreInitialize(IDocumentStore store)
    {
        store.Conventions.MaxNumberOfRequestsPerSession = 1000;
    }

    protected IDocumentStore CreateStore([CallerMemberName] string database = null)
    {
        if (database == null)
            database = $"aero-cms-test"; // todo - figure out how to not set a db name (index creation relies on it)
        
        var store = GetDocumentStore();
        
        // remove old database
        //store.Maintenance.Server.Send(
        //    new DeleteDatabasesOperation(
        //        databaseName: database,
        //        hardDelete: true // permanently remove files
        //    ));

        //store.Maintenance.Server.Send(
        //    new CreateDatabaseOperation(new DatabaseRecord(database)));

        IndexCreator.CreateIndexes(store);


        return store;
    }
    
    protected override void SetupDatabase(IDocumentStore documentStore)
    {
        base.SetupDatabase(documentStore);   
    }
    
    public override void Dispose()
    {
        
        base.Dispose();
    }
}
