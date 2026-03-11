


using Aero.Cms.Data;
using Microsoft.Extensions.DependencyInjection;
using Aero.Local;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Aero.Cms.Tests;

/// <summary>
/// Base class for using the api.
/// </summary>
public abstract class BaseTests : RavenTestBase, IDisposable
{
    protected IStorage storage = new FileStorage("uploads/", "~/uploads/");
    protected IServiceProvider services = new ServiceCollection()
        .BuildServiceProvider();

    protected IDocumentStore _store;
    protected IAsyncDocumentSession _session;

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected BaseTests() {
        _store = CreateStore();
        _session = _store.OpenAsyncSession();
        Init();
    }

    /// <summary>
    /// Disposes the test class.
    /// </summary>
    public new void Dispose() {
        _session.Dispose();
        _store.Dispose();
        Cleanup();
    }

    /// <summary>
    /// Sets up & initializes the tests.
    /// </summary>
    protected abstract void Init();

    /// <summary>
    /// Cleans up any possible data and resources
    /// created by the test.
    /// </summary>
    protected abstract void Cleanup();

    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected IDb GetDb() {
        return new DbRaven(_session, _store);
    }
}
