/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Repositories;
using Piranha.Services;

public static class PiranhaEFExtensions
{
    /// <summary>
    /// Adds the DbContext and the default services needed to run
    /// Piranha over Entity Framework Core.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="dboptions">The DbContext options builder</param>
    /// <param name="poolSize">The optional connection pool size. Default value is 128</param>
    /// <param name="scope">The optional lifetime</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The updated service collection</returns>
    public static PiranhaServiceBuilder UseEF<T>(this PiranhaServiceBuilder serviceBuilder,
        Action<DbContextOptionsBuilder> dboptions, int poolSize = 128,
        ServiceLifetime scope = ServiceLifetime.Scoped) where T : DbContext, IDb
    {
        serviceBuilder.Services.AddPiranhaEF<T>(dboptions, poolSize, scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the DbContext and the default services needed to run
    /// Piranha over Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="dboptions">The DbContext options builder</param>
    /// <param name="poolSize">The optional connection pool size. Default value is 128</param>
    /// <param name="scope">The optional lifetime</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddPiranhaEF<T>(this IServiceCollection services,
        Action<DbContextOptionsBuilder> dboptions, int poolSize = 128,
        ServiceLifetime scope = ServiceLifetime.Scoped) where T : DbContext, IDb
    {
        services.AddDbContextPool<T>(dboptions, poolSize);

        return RegisterServices<T>(services, scope);
    }

    /// <summary>
    /// Adds the default services needed to run Piranha over
    /// Entity Framework Core.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional lifetime</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The updated service collection</returns>
    private static IServiceCollection RegisterServices<T>(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Scoped) where T : DbContext, IDb
    {
        // Add the identity module
        App.Modules.Register<Piranha.Data.EF.Module>();

        // Register repositories
        services.Add(new ServiceDescriptor(typeof(IAliasRepository), typeof(AliasRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IArchiveRepository), typeof(ArchiveRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IContentRepository), typeof(ContentRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IContentGroupRepository), typeof(ContentGroupRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IContentTypeRepository), typeof(ContentTypeRepository), scope));
        services.Add(new ServiceDescriptor(typeof(ILanguageRepository), typeof(LanguageRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IMediaRepository), typeof(MediaRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IPageRepository), typeof(PageRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IPageTypeRepository), typeof(PageTypeRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IParamRepository), typeof(ParamRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IPostRepository), typeof(PostRepository), scope));
        services.Add(new ServiceDescriptor(typeof(IPostTypeRepository), typeof(PostTypeRepository), scope));
        services.Add(new ServiceDescriptor(typeof(ISiteRepository), typeof(SiteRepository), scope));
        services.Add(new ServiceDescriptor(typeof(ISiteTypeRepository), typeof(SiteTypeRepository), scope));

        // Register services
        services.Add(new ServiceDescriptor(typeof(IContentServiceFactory), typeof(ContentServiceFactory), ServiceLifetime.Singleton));
        services.Add(new ServiceDescriptor(typeof(IDb), typeof(T), scope));

        return services;
    }
}