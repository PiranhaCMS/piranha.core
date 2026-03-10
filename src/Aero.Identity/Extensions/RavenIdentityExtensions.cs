using Aero.Cms;
using Aero.Identity.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Aero.Cms.Manager.LocalAuth;

namespace Aero.Identity;

/// <summary>
/// Contains extension methods for registering RavenDB-based Identity stores.
/// </summary>
public static class RavenIdentityExtensions
{
    /// <summary>
    /// Adds the Aero.Cms RavenDB identity module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="identityOptions">Optional options for identity</param>
    /// <param name="cookieOptions">Optional options for cookies</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAeroRavenDbIdentity(this IServiceCollection services,
        Action<IdentityOptions>? identityOptions = null,
        Action<CookieAuthenticationOptions>? cookieOptions = null)
    {
        services
            .AddRazorPages()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AllowAnonymousToAreaPage("Manager", "/login");
            });

        // Add the identity module
        App.Modules.Register<RavenIdentityModule>();

        // Setup authorization policies
        services.AddAuthorization(o =>
        {
            // Role policies
            o.AddPolicy(Permissions.Roles, policy =>
            {
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
            });
            o.AddPolicy(Permissions.RolesAdd, policy =>
            {
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesAdd, Permissions.RolesAdd);
            });
            o.AddPolicy(Permissions.RolesDelete, policy =>
            {
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesDelete, Permissions.RolesDelete);
            });
            o.AddPolicy(Permissions.RolesEdit, policy =>
            {
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesEdit, Permissions.RolesEdit);
            });
            o.AddPolicy(Permissions.RolesSave, policy =>
            {
                policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                policy.RequireClaim(Permissions.RolesSave, Permissions.RolesSave);
            });

            // User policies
            o.AddPolicy(Permissions.Users, policy =>
            {
                policy.RequireClaim(Permissions.Users, Permissions.Users);
            });
            o.AddPolicy(Permissions.UsersAdd, policy =>
            {
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersAdd, Permissions.UsersAdd);
            });
            o.AddPolicy(Permissions.UsersDelete, policy =>
            {
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersDelete, Permissions.UsersDelete);
            });
            o.AddPolicy(Permissions.UsersEdit, policy =>
            {
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersEdit, Permissions.UsersEdit);
            });
            o.AddPolicy(Permissions.UsersSave, policy =>
            {
                policy.RequireClaim(Permissions.Users, Permissions.Users);
                policy.RequireClaim(Permissions.UsersSave, Permissions.UsersSave);
            });
        });

        services.AddIdentity<RavenUser, RavenRole>()
            .AddRavenDbStores()
            .AddDefaultTokenProviders();

        services.Configure(identityOptions ?? SetDefaultOptions);
        services.ConfigureApplicationCookie(cookieOptions ?? SetDefaultCookieOptions);
        //services.AddScoped<ISecurity, RavenIdentitySecurity>();

        return services;
    }

    /// <summary>
    /// Adds RavenDB implementations of Identity stores.
    /// </summary>
    /// <param name="builder">The Identity builder.</param>
    /// <returns>The Identity builder.</returns>
    public static IdentityBuilder AddRavenDbStores(this IdentityBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        AddStores(builder.Services, builder.UserType, builder.RoleType);

        return builder;
    }

    private static void AddStores(IServiceCollection services, Type userType, Type? roleType)
    {
        if (userType == null) throw new ArgumentNullException(nameof(userType));

        var userStoreType = typeof(RavenUserStore<>).MakeGenericType(userType);

        if (roleType != null)
        {
            var roleStoreType = typeof(RavenRoleStore<>).MakeGenericType(roleType);

            services.TryAddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);

            services.TryAddScoped(
                typeof(IRoleStore<>).MakeGenericType(roleType),
                roleStoreType);
        }
        else
        {
            services.TryAddScoped(
                typeof(IUserStore<>).MakeGenericType(userType),
                userStoreType);
        }
    }

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

    private static void SetDefaultCookieOptions(CookieAuthenticationOptions options)
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/manager/login";
        options.AccessDeniedPath = "/manager/login";
        options.SlidingExpiration = true;
    }
}
