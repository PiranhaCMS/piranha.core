using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Aero.AspNetCore;

namespace Aero.Cms.AspNetCore.Identity.Extensions;

public static class IdentityStartupExtensions
{
    /// <summary>
    /// Extensions method for simplified setup.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="dbOptions">The db options</param>
    /// <param name="identityOptions">The optional identity options</param>
    /// <param name="cookieOptions">The optional cookie options</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The builder</returns>
    public static AeroServiceBuilder UseIdentity<T>(this AeroServiceBuilder serviceBuilder,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
    {
        serviceBuilder.Services.AddAeroIdentity<T>(identityOptions, cookieOptions);

        return serviceBuilder;
    }

    /// <summary>
    /// Extensions method for simplified setup.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="dbOptions">The db options</param>
    /// <param name="identityOptions">The optional identity options</param>
    /// <param name="cookieOptions">The optional cookie options</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <returns>The builder</returns>
    public static AeroServiceBuilder UseIdentityWithSeed<T>(this AeroServiceBuilder serviceBuilder,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
    {
        serviceBuilder.Services.AddAeroIdentityWithSeed<T>(identityOptions, cookieOptions);

        return serviceBuilder;
    }

    /// <summary>
    /// Extensions method for simplified setup.
    /// </summary>
    /// <param name="serviceBuilder">The current service builder</param>
    /// <param name="dbOptions">The db options</param>
    /// <param name="identityOptions">The optional identity options</param>
    /// <param name="cookieOptions">The optional cookie options</param>
    /// <typeparam name="T">The DbContext type</typeparam>
    /// <typeparam name="TSeed">The seed type</typeparam>
    /// <returns>The builder</returns>
    public static AeroServiceBuilder UseIdentityWithSeed<T, TSeed>(this AeroServiceBuilder serviceBuilder,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
        where TSeed : class, IIdentitySeed
    {
        serviceBuilder.Services.AddAeroIdentityWithSeed<T, TSeed>(identityOptions, cookieOptions);

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Aero.Cms identity module.
    /// </summary>
    /// <param name="applicationBuilder">The current application builder</param>
    /// <returns>The builder</returns>
    public static AeroApplicationBuilder UseIdentity(this AeroApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UseAeroIdentity();

        return applicationBuilder;
    }
}