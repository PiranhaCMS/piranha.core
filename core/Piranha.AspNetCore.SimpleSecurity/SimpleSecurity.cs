/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Piranha.AspNetCore
{
    public class SimpleSecurity : ISecurity
    {
        /// <summary>
        /// Gets/sets the available users.
        /// </summary>
        private List<SimpleUser> Users { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimpleSecurity()
        {
            Users = new List<SimpleUser>();
        }

        /// <summary>
        /// Creates a new security object for the given users.
        /// </summary>
        /// <param name="users">The users</param>
        public SimpleSecurity(params SimpleUser[] users) : this()
        {
            Users.AddRange(users);
        }

        /// <summary>
        /// Authenticates the given credentials without
        /// signing in the user.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>If the given credentials were correct</returns>
        public bool Authenticate(string username, string password)
        {
            return Users.Count(u => u.UserName == username && u.Password == password) == 1;
        }

        /// <summary>
        /// Authenticates and signs in the user with the
        /// given credentials.
        /// </summary>
        /// <param name="context">The current application context</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>If the user was signed in</returns>
        public async Task<bool> SignIn(object context, string username, string password)
        {
            if (context is HttpContext)
            {
                if (Authenticate(username, password))
                {
                    var user = Users.Single(u => u.UserName == username && u.Password == password);

                    var claims = new List<Claim>();
                    foreach (var claim in user.Claims)
                    {
                        claims.Add(new Claim(claim, claim));
                    }
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                    claims.Add(new Claim(ClaimTypes.Sid, user.Id));

                    var identity = new ClaimsIdentity(claims, user.Password);
                    var principle = new ClaimsPrincipal(identity);

                    await ((HttpContext)context).SignInAsync("Piranha.SimpleSecurity", principle);

                    return true;
                }
                return false;
            }
            throw new ArgumentException("SimpleSecurity only works with a HttpContext");
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        /// <param name="context">The current application context</param>
        public Task SignOut(object context)
        {
            if (context is HttpContext)
            {
                return ((HttpContext)context).SignOutAsync("Piranha.SimpleSecurity");
            }
            throw new ArgumentException("SimpleSecurity only works with a HttpContext");
        }
    }
}