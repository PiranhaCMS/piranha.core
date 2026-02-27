/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */


using Microsoft.Extensions.DependencyInjection;
using Piranha.Data.RavenDb;
using Piranha.Data.RavenDb.Extensions;
using Piranha.Data.RavenDb.Repositories;
using Piranha.Data.RavenDb.Services.Internal;
using Xunit;
using Piranha.ImageSharp;
using Piranha.Repositories;
using Piranha.Services;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Piranha.Tests;

/// <summary>
/// Base class for using the api.
/// </summary>
public abstract class BaseTestsAsync : RavenTestBase, IAsyncLifetime
{
    protected IStorage _storage = new Local.FileStorage("uploads/", "~/uploads/");
    protected IImageProcessor _processor = new ImageSharpProcessor();
    protected IServiceProvider _services;
    protected Cache.ICache _cache;

    protected IDocumentStore _store;
    protected IAsyncDocumentSession _session;

    public virtual async Task InitializeAsync()
    {
        _store = CreateStore();
        _session = _store.OpenAsyncSession();
        _services = CreateServiceCollection(_session).BuildServiceProvider();

        var api = CreateApi();
        Piranha.App.Init(api);
    }

    public virtual async Task DisposeAsync()
    {
        _session.Dispose();
        _store.Dispose();
    }

    protected static IServiceCollection CreateServiceCollection(IAsyncDocumentSession session)
    {
        return new ServiceCollection()
            .AddScoped(_ => session)
            .AddAeroStore()
            .AddPiranha()
            .AddMemoryCache()
            .AddPiranhaMemoryCache()
            .AddDistributedMemoryCache()
            .AddPiranhaFileStorage()
            .AddPiranhaImageSharp();
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