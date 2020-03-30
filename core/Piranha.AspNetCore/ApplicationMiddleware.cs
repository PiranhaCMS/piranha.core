/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Piranha.AspNetCore.Services;
using Piranha.Web;
using System.Globalization;

namespace Piranha.AspNetCore
{
    public class ApplicationMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The logger factory</param>
        public ApplicationMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="api">The current api</param>
        /// <param name="service">The application service</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context, IApi api, IApplicationService service)
        {
            await service.InitAsync(context);

            // Set culture if applicable
            if (!string.IsNullOrEmpty(service.Site.Culture))
            {
                var cultureInfo = new CultureInfo(service.Site.Culture);
                CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = cultureInfo;
            }

            // Nothing to see here, move along
            await _next.Invoke(context);
        }
    }
}
