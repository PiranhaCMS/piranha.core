/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha.Areas.Manager.Hubs;
using Piranha.Areas.Manager.Services;
using Piranha.Manager;

public static class ManagerModuleExtensions
{
    /// <summary>
    /// Adds the Piranha manager module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaManager(this IServiceCollection services) {
        var assembly = typeof(ManagerModuleExtensions).GetTypeInfo().Assembly;
        var provider = new EmbeddedFileProvider(assembly, "Piranha");

        // Add the file provider to the Razor view engine
        services.Configure<RazorViewEngineOptions>(options => {
            options.FileProviders.Add(provider);
        });

        // Add the manager module
        Piranha.App.Modules.Register<Piranha.Manager.Module>();

        // Add the manager services
        services.AddScoped<PageEditService, PageEditService>();
        services.AddScoped<PostEditService, PostEditService>();
        services.AddScoped<SiteContentEditService, SiteContentEditService>();

        // Add session support
        services.AddSession();

        // Add SignalR
        services.AddSignalR();

        // Setup authorization policies
        services.AddAuthorization(o => {
            // Alias policies
            o.AddPolicy(Permission.Aliases, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Aliases, Permission.Aliases);
            });
            o.AddPolicy(Permission.AliasesDelete, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Aliases, Permission.Aliases);
                policy.RequireClaim(Permission.AliasesDelete, Permission.AliasesDelete);
            });
            o.AddPolicy(Permission.AliasesEdit, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Aliases, Permission.Aliases);
                policy.RequireClaim(Permission.AliasesEdit, Permission.AliasesEdit);
            });

            // Config policies
            o.AddPolicy(Permission.Config, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Config, Permission.Config);
            });
            o.AddPolicy(Permission.ConfigEdit, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Config, Permission.Config);
                policy.RequireClaim(Permission.ConfigEdit, Permission.ConfigEdit);
            });

            // Media policies
            o.AddPolicy(Permission.Media, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Media, Permission.Media);
            });
            o.AddPolicy(Permission.MediaAdd, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Media, Permission.Media);
                policy.RequireClaim(Permission.MediaAdd, Permission.MediaAdd);
            });
            o.AddPolicy(Permission.MediaDelete, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Media, Permission.Media);
                policy.RequireClaim(Permission.MediaDelete, Permission.MediaDelete);
            });
            o.AddPolicy(Permission.MediaEdit, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Media, Permission.Media);
                policy.RequireClaim(Permission.MediaEdit, Permission.MediaEdit);
            });
            o.AddPolicy(Permission.MediaAddFolder, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Media, Permission.Media);
                policy.RequireClaim(Permission.MediaAddFolder, Permission.MediaAddFolder);
            });
            o.AddPolicy(Permission.MediaDeleteFolder, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Media, Permission.Media);
                policy.RequireClaim(Permission.MediaDeleteFolder, Permission.MediaDeleteFolder);
            });

            // Module policies
            o.AddPolicy(Permission.Modules, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Modules, Permission.Modules);
            });

            // Page policies
            o.AddPolicy(Permission.Pages, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Pages, Permission.Pages);
            });
            o.AddPolicy(Permission.PagesAdd, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Pages, Permission.Pages);
                policy.RequireClaim(Permission.PagesAdd, Permission.PagesAdd);
            });
            o.AddPolicy(Permission.PagesDelete, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Pages, Permission.Pages);
                policy.RequireClaim(Permission.PagesDelete, Permission.PagesDelete);
            });
            o.AddPolicy(Permission.PagesEdit, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Pages, Permission.Pages);
                policy.RequireClaim(Permission.PagesEdit, Permission.PagesEdit);
            });
            o.AddPolicy(Permission.PagesPublish, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Pages, Permission.Pages);
                policy.RequireClaim(Permission.PagesPublish, Permission.PagesPublish);
            });
            o.AddPolicy(Permission.PagesSave, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Pages, Permission.Pages);
                policy.RequireClaim(Permission.PagesSave, Permission.PagesSave);
            });

            // Posts policies
            o.AddPolicy(Permission.Posts, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Posts, Permission.Posts);
            });
            o.AddPolicy(Permission.PostsAdd, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Posts, Permission.Posts);
                policy.RequireClaim(Permission.PostsAdd, Permission.PostsAdd);
            });
            o.AddPolicy(Permission.PostsDelete, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Posts, Permission.Posts);
                policy.RequireClaim(Permission.PostsDelete, Permission.PostsDelete);
            });
            o.AddPolicy(Permission.PostsEdit, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Posts, Permission.Posts);
                policy.RequireClaim(Permission.PostsEdit, Permission.PostsEdit);
            });
            o.AddPolicy(Permission.PostsPublish, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Posts, Permission.Posts);
                policy.RequireClaim(Permission.PostsPublish, Permission.PostsPublish);
            });
            o.AddPolicy(Permission.PostsSave, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Posts, Permission.Posts);
                policy.RequireClaim(Permission.PostsSave, Permission.PostsSave);
            });

            // Site policies
            o.AddPolicy(Permission.Sites, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Sites, Permission.Sites);
            });
            o.AddPolicy(Permission.SitesAdd, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Sites, Permission.Sites);
                policy.RequireClaim(Permission.SitesAdd, Permission.SitesAdd);
            });
            o.AddPolicy(Permission.SitesDelete, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Sites, Permission.Sites);
                policy.RequireClaim(Permission.SitesDelete, Permission.SitesDelete);
            });
            o.AddPolicy(Permission.SitesEdit, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Sites, Permission.Sites);
                policy.RequireClaim(Permission.SitesEdit, Permission.SitesEdit);
            });
            o.AddPolicy(Permission.SitesSave, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Sites, Permission.Sites);
                policy.RequireClaim(Permission.SitesSave, Permission.SitesSave);
            });
        });

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the piranha middleware.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaManager(this IApplicationBuilder builder) {
        return builder
            .UseSession()
            .UseMiddleware<Piranha.Manager.ResourceMiddleware>()
            .UseSignalR(routes => 
            {
                routes.MapHub<PreviewHub>("/manager/preview");
            });
    }
}
