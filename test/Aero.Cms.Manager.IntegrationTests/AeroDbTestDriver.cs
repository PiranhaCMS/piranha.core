using JasperFx;
using Marten;

namespace Aero.Cms.Manager.IntegrationTests;

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
            var connString = "Host=localhost;Port=5432;Database=aero-test;Username=postgres;Password=*strongPassword1;";
            opts.Connection(connString!);
            opts.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
        });
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