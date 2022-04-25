/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
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
    public static IServiceCollection AddPiranhaManager(this IServiceCollection services)
    {
        return services.AddPiranhaManager(null);
    }

    /// <summary>
    /// Adds the Piranha manager module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="configurePolicy">The delegate that will be used to build the Piranha Manager named policies.</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaManager(this IServiceCollection services, Action<string, AuthorizationPolicyBuilder> configurePolicy)
    {
        // Add the manager module
        Piranha.App.Modules.Register<Piranha.Manager.Module>();

        // Add the manager services
        services.AddScoped<AliasService>();
        services.AddScoped<CommentService>();
        services.AddScoped<ConfigService>();
        services.AddScoped<ContentService>();
        services.AddScoped<ContentTypeService>();
        services.AddScoped<LanguageService>();
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

        // Add preview policies
        services.AddAuthorization(o =>
        {
            o.AddPolicy(Piranha.Security.Permission.PagePreview, policy =>
            {
                policy.RequireClaim(Piranha.Security.Permission.PagePreview, Piranha.Security.Permission.PagePreview);
            });
            o.AddPolicy(Piranha.Security.Permission.PostPreview, policy =>
            {
                policy.RequireClaim(Piranha.Security.Permission.PostPreview, Piranha.Security.Permission.PostPreview);
            });
        });

        // Setup authorization policies
        services.AddAuthorization(o =>
        {
            if (configurePolicy is not null)
            {
                //If custom AuthorizationOptions delegate is provided, invoke
                foreach (var permission in Permission.All())
                {
                    o.AddPolicy(permission, configure => configurePolicy.Invoke(permission, configure));
                }
            }
            else
            {
                //Else configure default Claims based Policies, using Piranha nested permissions structure
                //Add root (Admin) policy and Claims
                o.AddPolicy(Permission.PermissionsStructure.PermissionName, configure => { configure.RequireClaim(Permission.PermissionsStructure.PermissionName, Permission.PermissionsStructure.PermissionName); });

                foreach (var level1Permission in Permission.PermissionsStructure.ChildPermissions)
                {
                    //Add 1 level deep nested level policy and Claims
                    o.AddPolicy(level1Permission.PermissionName, configure =>
                    {
                        configure.RequireClaim(Permission.PermissionsStructure.PermissionName, Permission.PermissionsStructure.PermissionName);
                        configure.RequireClaim(level1Permission.PermissionName, level1Permission.PermissionName);
                    });

                    //Add 2 levels deep policy and Claims
                    foreach (var level2Permission in level1Permission.ChildPermissions)
                    {
                        o.AddPolicy(level2Permission.PermissionName, configure =>
                        {
                            configure.RequireClaim(Permission.PermissionsStructure.PermissionName, Permission.PermissionsStructure.PermissionName);
                            configure.RequireClaim(level1Permission.PermissionName, level1Permission.PermissionName);
                            configure.RequireClaim(level2Permission.PermissionName, level2Permission.PermissionName);
                        });
                    }
                }
            }
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
        return builder
            .UseStaticFiles()
            .UseStaticFiles(new StaticFileOptions
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

    public static IMvcBuilder AddPiranhaManagerOptions(this IMvcBuilder builder,
        Action<MvcNewtonsoftJsonOptions> jsonOptions = null)
    {
        return builder
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Manager", "/");
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization()
            .AddNewtonsoftJson(options =>
            {
                // Invoke custom json options
                jsonOptions?.Invoke(options);

                // Set required options for Piranha
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
