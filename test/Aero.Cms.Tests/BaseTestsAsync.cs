


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Aero.Cms.ImageSharp;
using Aero.Cms.RavenDb;
using Aero.Cms.RavenDb.Extensions;
using Aero.Cms.RavenDb.Repositories;
using Aero.Cms.RavenDb.Services.Internal;
using Aero.Cms.Services;
using Aero.Local;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Aero.Cms.Tests;

/// <summary>
/// Base class for using the api.
/// </summary>
public abstract class BaseTestsAsync : RavenTestBase, IAsyncLifetime
{
    protected IStorage _storage = new FileStorage("uploads/", "~/uploads/");
    protected IImageProcessor _processor = new ImageSharpProcessor();
    protected IServiceProvider _services;
    protected Cache.ICache _cache;
    protected IApi _api;
    protected IDocumentStore _store;
    protected IAsyncDocumentSession _session;

    public virtual async Task InitializeAsync()
    {
        _store = CreateStore();
        _session = _store.OpenAsyncSession();
        _services = CreateServiceCollection(_store, _session).BuildServiceProvider();

        _api = CreateApi();
        Aero.Cms.App.Init(_api);
    }

    public virtual async Task DisposeAsync()
    {
        _session.Dispose();
        _store.Dispose();
    }

    /// <summary>
    /// Creates a fake IConfiguration for test use via NSubstitute.
    /// Provides default values for the RavenDB config keys that
    /// AeroDataExtensions expects when resolving IDocumentStore.
    /// </summary>
    protected static IConfiguration CreateFakeConfiguration()
    {
        var config = Substitute.For<IConfiguration>();
        config["RAVENDB_URL"].Returns("http://localhost:8080");
        config["RavenDb:Database"].Returns("aero-cms-test");
        config["RAVENDB_CERT"].Returns((string)null);
        return config;
    }

    protected static IServiceCollection CreateServiceCollection(
        IDocumentStore store,
        IAsyncDocumentSession session,
        Func<IServiceCollection, IServiceCollection> register = null)
    {
        var sc = new ServiceCollection()
            //.AddSingleton(CreateFakeConfiguration())
            .AddScoped(_ => session)
            .AddAeroStore(isTesting: true) // don't need this when using RavenTestBase
            .AddAero()
            .AddMemoryCache()
            .AddAeroMemoryCache()
            .AddDistributedMemoryCache()
            .AddAeroFileStorage()
            .AddAeroImageSharp();

        // todo - figure out where to put this registration to avoid nullref issues
        sc.AddSingleton<IMyService, MyService>(); // for dynamic region/field testing only
        // if (register is not null)
        //     register.Invoke(sc);
        sc.AddSingleton<IDocumentStore>(store);
        sc.AddScoped<IAsyncDocumentSession>(sc => session);

        return sc;
    }

    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected IDb GetDb()
    {
        return new DbRaven(_session, _store);
    }

    /// <summary>
    /// Creates a new api.
    /// </summary>
    protected virtual IApi CreateApi()
    {
        var factory = new ContentFactory(_services);
        var serviceFactory = new ContentServiceFactory(factory);

        var db = GetDb();

        var api = new Api(
            factory,
            new AliasRepository(db),
            new ArchiveRepository(db),
            new ContentRepository(db, serviceFactory),
            new ContentGroupRepository(db),
            new ContentTypeRepository(db),
            new LanguageRepository(db),
            new MediaRepository(db),
            new PageRepository(db, serviceFactory),
            new PageTypeRepository(db),
            new ParamRepository(db),
            new PostRepository(db, serviceFactory),
            new PostTypeRepository(db),
            new SiteRepository(db, serviceFactory),
            new SiteTypeRepository(db),
            cache: _cache,
            storage: _storage,
            processor: _processor
        );

        return api;
    }
}