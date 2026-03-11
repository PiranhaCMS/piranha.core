using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System.Security.Cryptography.X509Certificates;
using Aero.Cms.Data.Repositories;
using Aero.Cms.Data.Services;
using Aero.Cms.Data.Services.Internal;

namespace Aero.Cms.Data.Extensions;

public static class AeroDataExtensions
{
    /// <summary>
    /// Adds the DbContext and the default services needed to run
    /// Aero.Cms over Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional lifetime</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddAeroStore(this IServiceCollection services,
            ServiceLifetime scope = ServiceLifetime.Scoped, bool isTesting = false)
        //where T : IDb
    {
        services
            .AddAeroPersistence(scope, isTesting)
            .RegisterServices(scope);
        return services;
    }


    // todo - give user option to run embedded during startup
    private static IServiceCollection AddAeroStoreEmbedded(this IServiceCollection services, string path = ".")
    {
        services.AddSingleton<IDocumentStore>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var database = config["RavenDb:Database"] ?? "aero-cms";

            var store = Raven.Embedded.EmbeddedServer.Instance.GetDocumentStore(path);
            store.Conventions.MaxNumberOfRequestsPerSession = 500;
            store.Database = database;
            store.Initialize();

            IndexCreator.CreateIndexes(store);
            return store;
        });
        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
        return services.RegisterServices();
    }

    private static IServiceCollection AddAeroPersistence(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Scoped, bool isTesting = false)
    {
        // Configure RavenDB
        //var ravenUrl = builder.Configuration["RAVENDB_URL"];
        //var c = builder.Configuration["RAVENDB_CERT"];
        if (isTesting)
        {
            // tests will wire up IDocumentStore, etc
            return services;
        }

        services.AddSingleton<IDocumentStore>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var ravenUrl = config["RAVENDB_URL"] ?? config["RavenDb:Urls"] ?? "https://localhost:8080/";
            var database = config["RAVENDB_DATABASE"] ?? config["RavenDb:Database"] ?? "AeroCMS";
            var certPath = config["RAVENDB_CERT"] ?? config["RavenDb:CertificatePath"];

            X509Certificate2 cert = null;
            if (!string.IsNullOrEmpty(certPath) && File.Exists(certPath))
            {
                try
                {
                    cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RavenDB] Failed to load certificate from {certPath}: {ex.Message}");
                }
            }

            //    var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, null);
            //    //ravenStore.Certificate = cert;
            //}
            var store = new DocumentStore
            {
                Urls = [ravenUrl],
                Database = database,
                Certificate = cert
            };

            store.Initialize();

            // Ensure database exists
            try
            {
                var databaseNames = store.Maintenance.Server.Send(new GetDatabaseNamesOperation(0, 100));
                if (!databaseNames.Contains(database, StringComparer.OrdinalIgnoreCase))
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RavenDB] Failed to ensure database '{database}' exists: {ex.Message}");
                // We continue as Initialize succeeded, but operations might fail later if the database is truly missing
            }

            IndexCreator.CreateIndexes(store);

            return store;
        });

        services.AddScoped<IAsyncDocumentSession>(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());

        return services;
    }

    /// <summary>
    /// Adds the default services needed to run Aero.Cms over
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
        App.Modules.Register<Module>();

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