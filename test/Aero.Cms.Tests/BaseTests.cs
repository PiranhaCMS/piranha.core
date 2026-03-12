


using Aero.Cms.Data;
using Microsoft.Extensions.DependencyInjection;
using Aero.Local;


namespace Aero.Cms.Tests;

/// <summary>
/// Base class for using the api.
/// </summary>
public abstract class BaseTests : BaseTestsAsync, IDisposable
{
    protected IStorage storage = new FileStorage("uploads/", "~/uploads/");
    protected IServiceProvider services = new ServiceCollection()
        .BuildServiceProvider();

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected BaseTests() {
        Init();
    }

    /// <summary>
    /// Disposes the test class.
    /// </summary>
    public new void Dispose() {
        session.Dispose();
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
}
