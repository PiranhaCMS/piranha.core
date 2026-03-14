namespace Aero.Cms.Tests;



[CollectionDefinition("Marten")]
public class MartenCollection : ICollectionFixture<MartenFixture> { }


/// <summary>
/// Abstract base for all Marten integration tests.
/// Subclasses receive a fully initialized IDocumentStore via IClassFixture injection.
/// Each test method gets a fresh IDocumentSession; the store itself is shared per class.
/// </summary>
public abstract class AeroMartenTest(MartenFixture fixture) : IAsyncLifetime
{
    protected readonly IDocumentStore store = fixture.Store;
    protected IDocumentSession session { get; private set; } = null!;

    /// <summary>
    /// Opens a fresh lightweight session before each test method.
    /// Override and call base to add additional per-test setup.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        // Reset application state
        Aero.Cms.App.Reset();

        // Clean the database documents before each test method
        await store.Advanced.Clean.DeleteAllDocumentsAsync();

        // Reset initialization flag so seeding runs for the next test
        Aero.Cms.Data.AeroDbBase.IsInitialized = false;
        
        session = store.DirtyTrackedSession();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Disposes the session after each test method.
    /// Override and call base to add additional per-test teardown.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        if (session is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else
            session?.Dispose();
    }
}