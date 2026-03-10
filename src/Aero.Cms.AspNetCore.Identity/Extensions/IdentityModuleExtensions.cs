

using Aero.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Aero.Cms;
using Aero.Cms.AspNetCore.Identity;
using Aero.Cms.AspNetCore.Identity.Data;
using Aero.Cms.Manager;
using Aero.Cms.Manager.LocalAuth;

using IIdentityDb = Aero.Cms.AspNetCore.Identity.IIdentityDb;
using Module = Aero.Cms.AspNetCore.Identity.Module;

namespace Aero.Cms.AspNetCore.Identity;

public static class IdentityModuleExtensions
{
    /// <summary>
    /// Adds the Aero.Cms identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="dbOptions">Options for configuring the database</param>
    /// <param name="identityOptions">Optional options for identity</param>
    /// <param name="cookieOptions">Optional options for cookies</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroIdentity<T>(this IServiceCollection services,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
    {
        services
            .AddRazorPages()
            .AddRazorPagesOptions(options =>
        {
            options.Conventions.AllowAnonymousToAreaPage("Manager", "/login");
        });

        // Add the identity module
        App.Modules.Register<Module>();

        // Setup authorization policies
        services.AddAuthorization(o =>
        {
            // Role policies
            o.AddPolicy(Aero.Identity.Permissions.Roles, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Roles, Aero.Identity.Permissions.Roles);
            });
            o.AddPolicy(Aero.Identity.Permissions.RolesAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Roles, Aero.Identity.Permissions.Roles);
                policy.RequireClaim(Aero.Identity.Permissions.RolesAdd, Aero.Identity.Permissions.RolesAdd);
            });
            o.AddPolicy(Aero.Identity.Permissions.RolesDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Roles, Aero.Identity.Permissions.Roles);
                policy.RequireClaim(Aero.Identity.Permissions.RolesDelete, Aero.Identity.Permissions.RolesDelete);
            });
            o.AddPolicy(Aero.Identity.Permissions.RolesEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Roles, Aero.Identity.Permissions.Roles);
                policy.RequireClaim(Aero.Identity.Permissions.RolesEdit, Aero.Identity.Permissions.RolesEdit);
            });
            o.AddPolicy(Aero.Identity.Permissions.RolesSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Roles, Aero.Identity.Permissions.Roles);
                policy.RequireClaim(Aero.Identity.Permissions.RolesSave, Aero.Identity.Permissions.RolesSave);
            });

            // User policies
            o.AddPolicy(Aero.Identity.Permissions.Users, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Users, Aero.Identity.Permissions.Users);
            });
            o.AddPolicy(Aero.Identity.Permissions.UsersAdd, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Users, Aero.Identity.Permissions.Users);
                policy.RequireClaim(Aero.Identity.Permissions.UsersAdd, Aero.Identity.Permissions.UsersAdd);
            });
            o.AddPolicy(Aero.Identity.Permissions.UsersDelete, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Users, Aero.Identity.Permissions.Users);
                policy.RequireClaim(Aero.Identity.Permissions.UsersDelete, Aero.Identity.Permissions.UsersDelete);
            });
            o.AddPolicy(Aero.Identity.Permissions.UsersEdit, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Users, Aero.Identity.Permissions.Users);
                policy.RequireClaim(Aero.Identity.Permissions.UsersEdit, Aero.Identity.Permissions.UsersEdit);
            });
            o.AddPolicy(Aero.Identity.Permissions.UsersSave, policy =>
            {
                policy.RequireClaim(Permission.Admin, Permission.Admin);
                policy.RequireClaim(Aero.Identity.Permissions.Users, Aero.Identity.Permissions.Users);
                policy.RequireClaim(Aero.Identity.Permissions.UsersSave, Aero.Identity.Permissions.UsersSave);
            });
        });

        //services.AddDbContext<T>(dbOptions);
        services.AddScoped<IIdentityDb, T>();
        services.AddScoped<T, T>();
        services.AddIdentity<User, Role>()
            //.AddEntityFrameworkStores<T>()
            .AddRavenDbStores()
            .AddDefaultTokenProviders();
        services.Configure(identityOptions != null ? identityOptions : SetDefaultOptions);
        services.ConfigureApplicationCookie(cookieOptions != null ? cookieOptions : SetDefaultCookieOptions);
        services.AddScoped<ISecurity, IdentitySecurity>();

        return services;
    }

    /// <summary>
    /// Adds the Aero.Cms identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="dbOptions">Options for configuring the database</param>
    /// <param name="identityOptions">Optional options for identity</param>
    /// <param name="cookieOptions">Optional options for cookies</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroIdentityWithSeed<T, TSeed>(this IServiceCollection services,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
        where TSeed : class, IIdentitySeed
    {
        services = AddAeroIdentity<T>(services, identityOptions, cookieOptions);
        services.AddScoped<IIdentitySeed, TSeed>();

        return services;
    }

    /// <summary>
    /// Adds the Aero.Cms identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="dbOptions">Options for configuring the database</param>
    /// <param name="identityOptions">Optional options for identity</param>
    /// <param name="cookieOptions">Optional options for cookies</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroIdentityWithSeed<T>(this IServiceCollection services,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
    {
        return AddAeroIdentityWithSeed<T, DefaultIdentitySeed>(services, identityOptions, cookieOptions);
    }

    /// <summary>
    /// Sets the default identity options if none was provided. Please note that
    /// these settings provide very LOW security in terms of password rules, but
    /// this is just so the default user can be seeded on first startup.
    /// </summary>
    /// <param name="options">The identity options</param>
    private static void SetDefaultOptions(IdentityOptions options)
    {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    }

    /// <summary>
    /// Sets the default cookie options if none was provided.
    /// </summary>
    /// <param name="options">The cookie options</param>
    private static void SetDefaultCookieOptions(CookieAuthenticationOptions options)
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/manager/login";
        options.AccessDeniedPath = "/manager/login";
        options.SlidingExpiration = true;
    }

    /// <summary>
    /// Uses the Aero.Cms identity module.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseAeroIdentity(this IApplicationBuilder builder)
    {
        // Set logout url to point to local auth
        Aero.Cms.App.Modules.Manager().LogoutUrl = "~/manager/logout";

        //
        // Add the embedded resources
        //
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(IdentityModuleExtensions).Assembly, "Aero.Cms.AspNetCore.Identity.assets"),
            RequestPath = "/manager/identity"
        });
    }
}