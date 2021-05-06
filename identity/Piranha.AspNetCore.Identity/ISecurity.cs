/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Threading.Tasks;

namespace Piranha
{
    public interface ISecurity
    {
        /// <summary>
        /// Authenticates and signs in the user with the
        /// given credentials.
        /// </summary>
        /// <param name="context">The current application context</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>If the user was signed in</returns>
        Task<bool> SignIn(object context, string username, string password);

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        /// <param name="context">The current application context</param>
        Task SignOut(object context);
    }
}