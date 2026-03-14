

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using Aero.Cms.Manager;
using Aero.Cms.Manager.Hubs;
using Aero.Cms.Manager.Services;
using Aero.Manager;

public static class ManagerModuleExtensions
{
    /// <summary>
    /// Adds the Aero.Cms manager module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroManager(this IServiceCollection services)
    {
        return services.AddAeroManager(null);
    }

    /// <summary>
    /// Adds the Aero.Cms manager module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="configurePolicy">The delegate that will be used to build the Aero.Cms Manager named policies.</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroManager(this IServiceCollection services, Action<string, AuthorizationPolicyBuilder> configurePolicy)
    {
        // Add the manager module
        Aero.Cms.App.Modules.Register<Aero.Cms.Manager.Module>();

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
            o.AddPolicy(Aero.Cms.Security.Permission.PagePreview, policy =>
            {
                policy.RequireClaim(Aero.Cms.Security.Permission.PagePreview, Aero.Cms.Security.Permission.PagePreview);
            });
            o.AddPolicy(Aero.Cms.Security.Permission.PostPreview, policy =>
            {
                policy.RequireClaim(Aero.Cms.Security.Permission.PostPreview, Aero.Cms.Security.Permission.PostPreview);
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
                //Else configure default Claims based Policies, using Aero.Cms nested permissions structure
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
    /// Uses the Aero.Cms Manager.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseAeroManager(this IApplicationBuilder builder) {
        return builder
            .UseStaticFiles()
            .UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new EmbeddedFileProvider(typeof(ManagerModuleExtensions).Assembly, "Aero.Cms.Manager.assets.dist"),
                RequestPath = "/manager/assets"
            });
    }

    /// <summary>
    /// Adds the mappings needed for the Aero.Cms Manager to
    /// the endpoint routes.
    /// </summary>
    /// <param name="builder">The route builder</param>
    public static void MapAeroManager(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<PreviewHub>("/manager/preview");
        builder.MapRazorPages();
    }

    public static IMvcBuilder AddAeroManagerOptions(this IMvcBuilder builder,
        Action<JsonOptions> jsonOptions = null)
    {
        return builder
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Manager", "/");
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization()
            .AddJsonOptions(opts =>
            {
                // Invoke custom json options
                jsonOptions?.Invoke(opts);

                // Set required options for Aero.Cms
                
            });
        // .AddNewtonsoftJson(options =>
        // {
        //     // Invoke custom json options
        //     jsonOptions?.Invoke(options);
        //
        //     // Set required options for Aero.Cms
        //     options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        // });
    }

    /// <summary>
    /// Static accessor to Manager module if it is registered in the Aero.Cms
    /// application.
    /// </summary>
    /// <param name="modules">The available modules</param>
    /// <returns>The manager module</returns>
    public static Aero.Cms.Manager.Module Manager(this Aero.Cms.Runtime.AppModuleList modules)
    {
        return modules.Get<Aero.Cms.Manager.Module>();
    }
}
