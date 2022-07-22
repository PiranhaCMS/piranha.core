/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Identity;

namespace Piranha.Manager.LocalAuth
{
    /// <summary>
    /// Interface for services implementing local authorization.
    /// </summary>
    public interface ISecurity
    {
        /// <summary>
        /// Authenticates and signs in the user with the
        /// given credentials.
        /// </summary>
        /// <param name="context">The current application context</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>SignInResult</returns>
        Task<SignInResult> SignIn(object context, string username, string password);

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        /// <param name="context">The current application context</param>
        Task SignOut(object context);
    }
}