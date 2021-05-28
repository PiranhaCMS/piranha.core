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
using Microsoft.EntityFrameworkCore;

namespace Piranha.AspNetCore.Identity.EntityFrameworkCore
{
    public static class IdentityStartupExtensions
    {
        /// <summary>
        /// Extensions method for simplified setup.
        /// </summary>
        /// <param name="serviceBuilder">The current service builder</param>
        /// <param name="dbOptions">The db options</param>
        /// <typeparam name="T">The DbContext type</typeparam>
        /// <returns>The builder</returns>
        public static PiranhaServiceBuilder UseIdentityEF<T>(this PiranhaServiceBuilder serviceBuilder, Action<DbContextOptionsBuilder> dbOptions)
            where T : Db<T>
        {
            serviceBuilder.Services.AddPiranhaIdentityEF<T>(dbOptions);

            return serviceBuilder;
        }
    }
}