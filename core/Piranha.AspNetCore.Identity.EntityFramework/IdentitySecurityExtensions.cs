using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Piranha.AspNetCore.Identity.EntityFramework
{
    public static class IdentitySecurityExtensions
    {
        public static IServiceCollection AddPiranhaIdentitySecurityEF(this IServiceCollection services, Action<IdentityDbBuilder> builder)
        {
            services.AddSingleton<Action<IdentityDbBuilder>>(builder);

            var config = new IdentityDbBuilder();
            builder?.Invoke(config);

            services.AddDbContext<IdentityDb>(options =>
                options.UseSqlServer(config.ConnectionString));

            services.AddIdentity<IdentityAppUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDb>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<IdentityAppUser>, IdentityAppClaimsPrincipalFactory>();

            if (config.IdentityOptions != null)
            {
                services.Configure(ConfgureIdentityOptions(config.IdentityOptions));
            }

            if (config.CookieAuthenticationOptions != null)
            {
                services.Configure(ConfigureCookieOptions(config.CookieAuthenticationOptions));
            }
            else
            {
                // Set some defaults that I like
                services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Expiration = TimeSpan.FromDays(100);
                    options.LoginPath ="/Manager/Login";
                    options.LogoutPath = "/Manager/Logout";
                });
            }

            return services.AddSingleton<ISecurity,IdentitySecurity>();
        }


        #region Helpers

        private static Action<IdentityOptions> ConfgureIdentityOptions(IdentityOptions userOptions)
        {
            return options =>
            {
                options.Password.RequireDigit = userOptions.Password.RequireDigit;
                options.Password.RequiredLength = userOptions.Password.RequiredLength;
                options.Password.RequireNonAlphanumeric = userOptions.Password.RequireNonAlphanumeric;
                options.Password.RequireUppercase = userOptions.Password.RequireUppercase;
                options.Password.RequireLowercase = userOptions.Password.RequireLowercase;
                options.Password.RequiredUniqueChars = userOptions.Password.RequiredUniqueChars;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = userOptions.Lockout.DefaultLockoutTimeSpan;
                options.Lockout.MaxFailedAccessAttempts = userOptions.Lockout.MaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = userOptions.Lockout.AllowedForNewUsers;

                // User settings
                options.User.RequireUniqueEmail = userOptions.User.RequireUniqueEmail;
            };
        }

        private static Action<CookieAuthenticationOptions> ConfigureCookieOptions(CookieAuthenticationOptions userOptions)
        {
            return options =>
            {
                options.Cookie.HttpOnly = userOptions.Cookie.HttpOnly;
                options.Cookie.Expiration = userOptions.Cookie.Expiration;
                options.LoginPath = userOptions.LoginPath;
                options.LogoutPath = userOptions.LogoutPath;
            };
        }
        
        #endregion


        public static IApplicationBuilder UsePiranhaIdentityEFSecurity(this IApplicationBuilder builder)
        {
            return builder.UseAuthentication();
        }

      
    }
}