using Aero.Cms.Data.Repositories;
using Aero.Cms.Data.Services;
using Aero.Cms.Data.Services.Internal;
using Aero.Cms.Repositories;
using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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


    private static IServiceCollection AddAeroPersistence(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Scoped, bool isTesting = false)
    {
        // Configure RavenDB
        //var ravenUrl = builder.Configuration["RAVENDB_URL"];
        //var c = builder.Configuration["RAVENDB_CERT"];
        if (isTesting)
        {
            // todo - verify we still need to check this now that we move away from ravendb
            // tests will wire up IDocumentStore, etc
            return services;
        }

        services.AddSingleton<IDocumentStore>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var connString = config.GetConnectionString("pg");

            var store = DocumentStore.For(opts =>
            {
                opts.Connection(connString!);
                opts.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
            });

            return store;
        });

        services.AddScoped(s => s.GetRequiredService<IDocumentStore>().LightweightSession());

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
        services.AddScoped<IDb, AeroDb>();

        return services;
    }
}