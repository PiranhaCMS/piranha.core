/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Piranha.Manager;
using Piranha.Manager.Hubs;
using Piranha.Manager.Services;

public static class ManagerModuleExtensions
{
    /// <summary>
    /// Adds the Piranha manager module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaManager(this IServiceCollection services) {
        // Add the manager module
        Piranha.App.Modules.Register<Piranha.Manager.Module>();

        // Add the manager services
        services.AddScoped<AliasService>();
        services.AddScoped<CommentService>();
        services.AddScoped<ConfigService>();
        services.AddScoped<ContentTypeService>();
        services.AddScoped<MediaService>();
        services.AddScoped<ModuleService>();
        services.AddScoped<PageService>();
        services.AddScoped<PostService>();
        services.AddScoped<SiteService>();

        // Add localization service
        services.AddScoped<ManagerLocalizer>();

        // Add session support
        services.AddSession();

        // Add SignalR
        services.AddSignalR();

        // Setup authorization policies
        services.AddAuthorization(o => {
            // Admin policy
            o.AddPolicy(Permission.Admin, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
            });

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

            // Comment policies
            o.AddPolicy(Permission.Comments, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Comments, Permission.Comments);
            });
            o.AddPolicy(Permission.CommentsApprove, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Comments, Permission.Comments);
                policy.RequireClaim(Permission.CommentsApprove, Permission.CommentsApprove);
            });
            o.AddPolicy(Permission.CommentsDelete, policy => {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Permission.Comments, Permission.Comments);
                policy.RequireClaim(Permission.CommentsDelete, Permission.CommentsDelete);
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
    /// Uses the Piranha Manager.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaManager(this IApplicationBuilder builder) {
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(ManagerModuleExtensions).Assembly, "Piranha.Manager.assets.dist"),
            RequestPath = "/manager/assets"
        });
    }

    /// <summary>
    /// Adds the mappings needed for the Piranha Manager to
    /// the endpoint routes.
    /// </summary>
    /// <param name="builder">The route builder</param>
    public static void MapPiranhaManager(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<PreviewHub>("/manager/preview");
        builder.MapRazorPages();
    }

    public static IMvcBuilder AddPiranhaManagerOptions(this IMvcBuilder builder)
    {
        return builder
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Manager", "/");
                options.Conventions.AllowAnonymousToAreaPage("Manager", "/login");
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            });
    }

    /// <summary>
    /// Static accessor to Manager module if it is registered in the Piranha
    /// application.
    /// </summary>
    /// <param name="modules">The available modules</param>
    /// <returns>The manager module</returns>
    public static Piranha.Manager.Module Manager(this Piranha.Runtime.AppModuleList modules)
    {
        return modules.Get<Piranha.Manager.Module>();
    }
}
