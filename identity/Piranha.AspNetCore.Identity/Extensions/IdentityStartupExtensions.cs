using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Piranha;
using Piranha.AspNetCore.Identity;
using Piranha.AspNetCore;

namespace Piranha.AspNetCore.Identity.Extensions;

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
    public static PiranhaServiceBuilder UseIdentity<T>(this PiranhaServiceBuilder serviceBuilder,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
    {
        serviceBuilder.Services.AddPiranhaIdentity<T>(identityOptions, cookieOptions);

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
    public static PiranhaServiceBuilder UseIdentityWithSeed<T>(this PiranhaServiceBuilder serviceBuilder,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
    {
        serviceBuilder.Services.AddPiranhaIdentityWithSeed<T>(identityOptions, cookieOptions);

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
    public static PiranhaServiceBuilder UseIdentityWithSeed<T, TSeed>(this PiranhaServiceBuilder serviceBuilder,
        //Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : IdentityDb<T>
        where TSeed : class, IIdentitySeed
    {
        serviceBuilder.Services.AddPiranhaIdentityWithSeed<T, TSeed>(identityOptions, cookieOptions);

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Piranha identity module.
    /// </summary>
    /// <param name="applicationBuilder">The current application builder</param>
    /// <returns>The builder</returns>
    public static PiranhaApplicationBuilder UseIdentity(this PiranhaApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UsePiranhaIdentity();

        return applicationBuilder;
    }
}