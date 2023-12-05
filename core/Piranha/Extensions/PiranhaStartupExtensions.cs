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
using Piranha;
using Piranha.Models;
using Piranha.Services;

public static class PiranhaStartupExtensions
{
    public static IServiceCollection AddPiranha(this IServiceCollection services,
        Action<PiranhaServiceBuilder> options = null, ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        var serviceBuilder = new PiranhaServiceBuilder(services);

        options?.Invoke(serviceBuilder);

        services.AddSingleton<IContentFactory, ContentFactory>();

        services.AddScoped<IAliasService, AliasService>();
        services.AddScoped<IArchiveService, ArchiveService>();
        services.AddScoped<IContentGroupService, ContentGroupService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IContentTypeService, ContentTypeService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IPageTypeService, PageTypeService>();
        services.AddScoped<IParamService, ParamService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostTypeService, PostTypeService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<ISiteTypeService, SiteTypeService>();

        services.AddScoped<IApi, Api>();
        services.AddScoped<Config>();

        return services;
    }
}