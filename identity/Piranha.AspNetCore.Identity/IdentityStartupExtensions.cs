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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Identity;
using Piranha.AspNetCore;

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
        Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : Db<T>
    {
        serviceBuilder.Services.AddPiranhaIdentity<T>(dbOptions, identityOptions, cookieOptions);

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
        Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : Db<T>
    {
        serviceBuilder.Services.AddPiranhaIdentityWithSeed<T>(dbOptions, identityOptions, cookieOptions);

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
        Action<DbContextOptionsBuilder> dbOptions,
        Action<IdentityOptions> identityOptions = null,
        Action<CookieAuthenticationOptions> cookieOptions = null)
        where T : Db<T>
        where TSeed : class, IIdentitySeed
    {
        serviceBuilder.Services.AddPiranhaIdentityWithSeed<T, TSeed>(dbOptions, identityOptions, cookieOptions);

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