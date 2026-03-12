using JasperFx;
using Marten;

namespace Aero.Cms.Tests;

public abstract class AeroDbTestDriver : IDisposable
{
    protected IDocumentStore store
    {
        get => field ?? GetDocumentStore();
        init;
    }

    protected bool IsDisposed { get; private set; }

    protected AeroDbTestDriver()
    {
        store = DocumentStore.For(opts =>
        {
            var connString =
                "Host=localhost;Port=5432;Database=aero-test;Username=postgres;Password=*strongPassword1;";
            opts.Connection(connString!);

            opts.CreateDatabasesForTenants(config =>
            {
                // Maintenance database is used to connect and run the CREATE DATABASE script.
                // If not specified, it defaults to 'postgres' on the same server.
                config.MaintenanceDatabase(
                    "Host=localhost;Database=postgres;Username=postgres;Password=*strongPassword1;");
                config.ForTenant()
                    .CheckAgainstPgDatabase() // Verify existence using pg_database table
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1);
            });

            // CRITICAL: This ensures the database creation and schema migrations run on startup
            //.ApplyAllDatabaseChangesOnStartup();
            ;
            opts.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
        });

        store.Storage.ApplyAllConfiguredChangesToDatabaseAsync()
            .GetAwaiter()
            .GetResult();
    }

    protected internal IDocumentStore GetDocumentStore(string database = null)
    {
        return this.store;
    }

    protected virtual void PreInitialize(IDocumentStore store)
    {

    }

    protected virtual void PreConfigureDatabase(IDocumentStore store)
    {

    }

    protected virtual void SetupDatabase(IDocumentStore store)
    {

    }

    public void Dispose()
    { 
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        if (disposing)
        {
            store.Dispose();
        }
        IsDisposed = true;
    }
}