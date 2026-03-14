using Microsoft.Extensions.DependencyInjection;
using Aero.Cms.ImageSharp;
using Aero.Cms.Data;
using Aero.Cms.Data.Extensions;
using Aero.Cms.Data.Repositories;
using Aero.Cms.Data.Services.Internal;
using Aero.Cms.Services;
using Aero.Local;
using Marten.Services;


namespace Aero.Cms.Tests;

/// <summary>
/// Base class for using the api.
/// </summary>
public abstract class AsyncTestBase(MartenFixture fixture) : AeroMartenTest(fixture), IClassFixture<MartenFixture>
{
    protected IStorage storage = new FileStorage("uploads/", "~/uploads/");
    protected IImageProcessor processor = new ImageSharpProcessor();
    protected IServiceProvider services;
    protected Cache.ICache cache
    {
        get => _cache;
        set
        {
            _cache = value;
            api = CreateApi();
        }
    }
    private Cache.ICache _cache;
    protected IApi api;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        services = CreateServiceCollection(store).BuildServiceProvider();

        api = CreateApi();
        Aero.Cms.App.Init(api);

        Init();
    }


    protected static IServiceCollection CreateServiceCollection(
        IDocumentStore store,
        Func<IServiceCollection, IServiceCollection> register = null)
    {
        if(store is null)
            ArgumentNullException.ThrowIfNull(store, nameof(IDocumentStore));

        var sc = new ServiceCollection()
            //.AddSingleton(CreateFakeConfiguration())
            .AddScoped(_ => store.LightweightSession())
            .AddAeroStore(isTesting: true) // don't need this when using AeroDbTestDriver
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
        //sc.AddScoped<IDocumentSession>(sc => session);

        return sc;
    }

    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected IDb GetDb()
    {
        return new AeroDb(session, store);
    }

    /// <summary>
    /// Creates a new api.
    /// </summary>
    protected virtual IApi CreateApi()
    {
        var factory = new ContentFactory(services);
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
            cache: cache,
            storage: storage,
            processor: processor
        );

        return api;
    }

    /// <summary>
    /// Sets up & initializes the tests.
    /// </summary>
    protected virtual void Init()
    {
    }

    /// <summary>
    /// Cleans up any possible data and resources
    /// created by the test.
    /// </summary>
    protected virtual void Cleanup()
    {
    }

    /// <summary>
    /// Disposes the test class.
    /// </summary>
    public void Dispose()
    {
        session.Dispose();
        Cleanup();
    }
}