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

namespace Piranha.AspNetCore.Identity
{
    public static class IdentityStartupExtensions
    {
        /// <summary>
        /// Extensions method for simplified setup.
        /// </summary>
        /// <param name="serviceBuilder">The current service builder</param>
        /// <param name="identityOptions">The optional identity options</param>
        /// <param name="cookieOptions">The optional cookie options</param>
        /// <typeparam name="T">The DbContext type</typeparam>
        /// <returns>The builder</returns>
        public static PiranhaServiceBuilder UseIdentity(this PiranhaServiceBuilder serviceBuilder,
            Action<IdentityOptions> identityOptions = null,
            Action<CookieAuthenticationOptions> cookieOptions = null)
        {
            serviceBuilder.Services.AddPiranhaIdentity(identityOptions, cookieOptions);

            return serviceBuilder;
        }

        /// <summary>
        /// Extensions method for simplified setup.
        /// </summary>
        /// <param name="serviceBuilder">The current service builder</param>
        /// <param name="identityOptions">The optional identity options</param>
        /// <param name="cookieOptions">The optional cookie options</param>
        /// <typeparam name="T">The DbContext type</typeparam>
        /// <returns>The builder</returns>
        public static PiranhaServiceBuilder UseIdentityWithSeed(this PiranhaServiceBuilder serviceBuilder,
            Action<IdentityOptions> identityOptions = null,
            Action<CookieAuthenticationOptions> cookieOptions = null)
        {
            serviceBuilder.Services.AddPiranhaIdentityWithSeed<DefaultIdentitySeed>(identityOptions, cookieOptions);

            return serviceBuilder;
        }

        /// <summary>
        /// Extensions method for simplified setup.
        /// </summary>
        /// <param name="serviceBuilder">The current service builder</param>
        /// <param name="identityOptions">The optional identity options</param>
        /// <param name="cookieOptions">The optional cookie options</param>
        /// <typeparam name="T">The DbContext type</typeparam>
        /// <typeparam name="TSeed">The seed type</typeparam>
        /// <returns>The builder</returns>
        public static PiranhaServiceBuilder UseIdentityWithSeed<TSeed>(this PiranhaServiceBuilder serviceBuilder,
            Action<IdentityOptions> identityOptions = null,
            Action<CookieAuthenticationOptions> cookieOptions = null)
            where TSeed : class, IIdentitySeed
        {
            serviceBuilder.Services.AddPiranhaIdentityWithSeed<TSeed>(identityOptions, cookieOptions);

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
}