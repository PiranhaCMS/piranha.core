/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha
{
    /// <summary>
    /// Interface for the security manager.
    /// </summary>
    public interface ISecurity
    {
        /// <summary>
        /// Authenticates the given user credentials without
        /// signing in the user.
        /// </summary>
        /// <param name="username">The user</param>
        /// <param name="password">The password</param>
        /// <returns>If the credentials was authenticated successfully</returns>
        bool Authenticate(string username, string password);

        /// <summary>
        /// Signs in the user with the given username and password.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>If the user was signed in</returns>
        bool SignIn(string username, string password);

        /// <summary>
        /// Signs out the currently authenticated user.
        /// </summary>
        void SignOut();

        /// <summary>
        /// Checks if the current user is authenticated.
        /// </summary>
        /// <returns>If the user is authenticated</returns>
        bool IsAuthenticated();

        /// <summary>
        /// Checks if the current user has the given role.
        /// </summary>
        /// <param name="role">The role</param>
        /// <returns>If the user has the role</returns>
        bool IsInRole(string role);

        /// <summary>
        /// Gets the user id of the currently authenticated user.
        /// </summary>
        /// <returns>The user id</returns>
        string GetUserId();
    }
}
