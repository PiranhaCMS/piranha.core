﻿/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Piranha.AspNetCore
{
    /// <summary>
    /// Base class for middleware.
    /// </summary>
    public abstract class MiddlewareBase
    {
        /// <summary>
        /// The next middleware in the pipeline.
        /// </summary>
        protected readonly RequestDelegate next;

        /// <summary>
        /// The optional logger.
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// The item key for accessing the stored site id.
        /// </summary>
        public const string SiteId = "Piranha_SiteId";

        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public MiddlewareBase(RequestDelegate next) {
            this.next = next;
        }

        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public MiddlewareBase(RequestDelegate next, ILoggerFactory factory) : this(next) {
            if (factory != null)
                logger = factory.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="api">The current api</param>
        /// <returns>An async task</returns>
        public abstract Task Invoke(HttpContext context, IApi api);

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

        /// <summary>
        /// Gets the id from the currently requested site.
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <returns>The requested site id</returns>
        protected Guid GetSiteId(HttpContext context) {
            object id = null;

            if (context.Items.TryGetValue(SiteId, out id))
                return (Guid)id;
            return Guid.Empty;
        }
    }
}
