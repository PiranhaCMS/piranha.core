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

namespace Piranha.AspNetCore
{
    /// <summary>
    /// The simplest user ever.
    /// </summary>
    public class SimpleUser
    {
        /// <summary>
        /// Gets/sets the user id.
        /// </summary>
        internal string Id { get; set; }

        /// <summary>
        /// Gets/sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets/sets the user password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets/sets the user claims.
        /// </summary>
        public List<string> Claims { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimpleUser()
        {
            Id = Guid.NewGuid().ToString();
            Claims = new List<string>();
        }

        /// <summary>
        /// Creates a new user with the given claims.
        /// </summary>
        /// <param name="claims">The claims</param>
        public SimpleUser(params string[] claims) : this()
        {
            Claims.AddRange(claims);
        }
    }
}