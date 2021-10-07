/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha.core
 *
 */

using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Manager;
using Piranha.Manager.LocalAuth;

namespace Piranha.AspNetCore.Identity
{
    public static class IdentityModuleExtensions
    {
        /// <summary>
        /// Adds the Piranha identity module.
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="identityOptions">Optional options for identity</param>
        /// <param name="cookieOptions">Optional options for cookies</param>
        /// <returns>The services</returns>
        public static IServiceCollection AddPiranhaIdentity(this IServiceCollection services,
            Action<IdentityOptions> identityOptions = null,
            Action<CookieAuthenticationOptions> cookieOptions = null)
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
                o.AddPolicy(Permissions.Roles, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                });
                o.AddPolicy(Permissions.RolesAdd, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                    policy.RequireClaim(Permissions.RolesAdd, Permissions.RolesAdd);
                });
                o.AddPolicy(Permissions.RolesDelete, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                    policy.RequireClaim(Permissions.RolesDelete, Permissions.RolesDelete);
                });
                o.AddPolicy(Permissions.RolesEdit, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                    policy.RequireClaim(Permissions.RolesEdit, Permissions.RolesEdit);
                });
                o.AddPolicy(Permissions.RolesSave, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Roles, Permissions.Roles);
                    policy.RequireClaim(Permissions.RolesSave, Permissions.RolesSave);
                });

                // User policies
                o.AddPolicy(Permissions.Users, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Users, Permissions.Users);
                });
                o.AddPolicy(Permissions.UsersAdd, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Users, Permissions.Users);
                    policy.RequireClaim(Permissions.UsersAdd, Permissions.UsersAdd);
                });
                o.AddPolicy(Permissions.UsersDelete, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Users, Permissions.Users);
                    policy.RequireClaim(Permissions.UsersDelete, Permissions.UsersDelete);
                });
                o.AddPolicy(Permissions.UsersEdit, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Users, Permissions.Users);
                    policy.RequireClaim(Permissions.UsersEdit, Permissions.UsersEdit);
                });
                o.AddPolicy(Permissions.UsersSave, policy =>
                {
                    policy.RequireClaim(Permission.Admin, Permission.Admin);
                    policy.RequireClaim(Permissions.Users, Permissions.Users);
                    policy.RequireClaim(Permissions.UsersSave, Permissions.UsersSave);
                });
            });

            services.AddIdentity<User, Role>()
                .AddDefaultTokenProviders();

            services.Configure(identityOptions ?? SetDefaultOptions);
            services.ConfigureApplicationCookie(cookieOptions ?? SetDefaultCookieOptions);
            services.AddScoped<ISecurity, IdentitySecurity>();

            return services;
        }

        /// <summary>
        /// Adds the Piranha identity module.
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="identityOptions">Optional options for identity</param>
        /// <param name="cookieOptions">Optional options for cookies</param>
        /// <returns>The services</returns>
        public static IServiceCollection AddPiranhaIdentityWithSeed<TSeed>(this IServiceCollection services,
            Action<IdentityOptions> identityOptions = null,
            Action<CookieAuthenticationOptions> cookieOptions = null)
            where TSeed : class, IIdentitySeed
        {
            services = AddPiranhaIdentity(services, identityOptions, cookieOptions);
            services.AddScoped<IIdentitySeed, TSeed>();

            return services;
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
        /// Uses the Piranha identity module.
        /// </summary>
        /// <param name="builder">The current application builder</param>
        /// <returns>The builder</returns>
        public static IApplicationBuilder UsePiranhaIdentity(this IApplicationBuilder builder)
        {
            // Set logout url to point to local auth
            Piranha.App.Modules.Manager().LogoutUrl = "~/manager/logout";

            //
            // Add the embedded resources
            //
            return builder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new EmbeddedFileProvider(typeof(IdentityModuleExtensions).Assembly, "Piranha.AspNetCore.Identity.assets"),
                RequestPath = "/manager/identity"
            });
        }
    }
}