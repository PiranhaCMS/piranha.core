using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Piranha.Data.RavenDb.Repositories;
using Piranha.Data.RavenDb.Services;
using Piranha.Data.RavenDb.Services.Internal;
using Piranha.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Security.Cryptography.X509Certificates;

namespace Piranha.Data.RavenDb.Extensions;

public static class AeroDataExtensions
{
    /// <summary>
    /// Adds the DbContext and the default services needed to run
    /// Piranha over Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional lifetime</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddAeroStore(this IServiceCollection services, ServiceLifetime scope = ServiceLifetime.Scoped)
        //where T : IDb
    {
        return services
            .AddAeroPersistence(scope)
            .RegisterServices(scope);
    }


    // todo - give user option to run embedded during startup
    private static IServiceCollection AddAeroStoreEmbedded(this IServiceCollection services, string path = ".")
    {
        services.AddSingleton<IDocumentStore>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var database = config["RavenDb:Database"] ?? "aero-cms";

            var store = Raven.Embedded.EmbeddedServer.Instance.GetDocumentStore(path);
            store.Initialize();
            
            IndexCreator.CreateIndexes(store);
            return store;
        } );
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        return services.RegisterServices();
    }

    private static IServiceCollection AddAeroPersistence(this IServiceCollection services, ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        // Configure RavenDB
        //var ravenUrl = builder.Configuration["RAVENDB_URL"];
        //var c = builder.Configuration["RAVENDB_CERT"];

        services.AddSingleton<IDocumentStore>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var ravenUrl = config["RAVENDB_URL"] ?? throw new InvalidOperationException("RavenDb:Url configuration is required.");
            var database = config["RavenDb:Database"] ?? "aero-cms";
            var certPath = config["RAVENDB_CERT"];

            //if (Environment.GetEnvironmentVariable("RAVENDB_CERT") is { Length: > 0 } certPath)
            //{
                var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, null);
                //ravenStore.Certificate = cert;
            //}
            var store = new DocumentStore
            {
                Urls = [ravenUrl],
                Database = config["RavenDb:Database"] ?? "aero-cms",
                Certificate = certPath != null ? X509CertificateLoader.LoadPkcs12FromFile(certPath, null) : null
            };

            store.Initialize();
            
            IndexCreator.CreateIndexes(store);

            return store;
        } );

        services.AddScoped<IAsyncDocumentSession>(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());

        return services;
    }

    /// <summary>
    /// Adds the default services needed to run Piranha over
    /// Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional lifetime</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The updated service collection</returns>
    private static IServiceCollection RegisterServices(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        // Add the identity module
        App.Modules.Register<Piranha.Data.RavenDb.Module>();

        // Register repositories
        services.AddScoped<IAliasRepository, AliasRepository>();
        services.AddScoped<IArchiveRepository, ArchiveRepository>();
        services.AddScoped<IContentRepository, ContentRepository>();
        services.AddScoped<IContentGroupRepository, ContentGroupRepository>();
        services.AddScoped<IContentTypeRepository, ContentTypeRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IPageRepository, PageRepository>();
        services.AddScoped<IPageTypeRepository, PageTypeRepository>();
        services.AddScoped<IParamRepository, ParamRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostTypeRepository, PostTypeRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<ISiteTypeRepository, SiteTypeRepository>();

        // Register services
        services.AddSingleton<IContentServiceFactory, ContentServiceFactory>();
        services.AddScoped<IDb, DbRaven>();

        return services;
    }
}