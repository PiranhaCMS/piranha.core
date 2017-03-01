/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    /// <summary>
    /// Base class for middleware.
    /// </summary>
    public abstract class MiddlewareBase
    {
        #region Members
        /// <summary>
        /// The next middleware in the pipeline.
        /// </summary>
        protected readonly RequestDelegate next;

        /// <summary>
        /// The current api.
        /// </summary>
        protected readonly Api api;
        #endregion

        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public MiddlewareBase(RequestDelegate next, Api api) {
            this.next = next;
            this.api = api;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public abstract Task Invoke(HttpContext context);

        /// <summary>
        /// Checks if the request has already been handled by another
        /// Piranha middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>If the request has already been handled</returns>
        protected bool IsHandled(HttpContext context) {
            var values = context.Request.Query["piranha_handled"];
            if (values.Count > 0) {
                return values[0] == "true";
            }
            return false;
        }
    }
}
