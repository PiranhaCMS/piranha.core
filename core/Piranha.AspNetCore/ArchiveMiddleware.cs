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
using Piranha.AspNetCore.Services;
using Piranha.Web;
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    public class ArchiveMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="factory">The optional logger factory</param>
        public ArchiveMiddleware(RequestDelegate next, ILoggerFactory factory = null) : base(next, factory) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="api">The current api</param>
        /// <param name="service">The application service</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context, IApi api, IApplicationService service)
        {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/"))
            {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                var siteId = service.Site.Id;
                bool authorized = true;

                var response = await ArchiveRouter.InvokeAsync(api, url, siteId);

                if (response != null)
                {
                    _logger?.LogInformation($"Found archive\n  Route: {response.Route}\n  Params: {response.QueryString}");

                    if (!response.IsPublished)
                    {
                        if (!context.User.HasClaim(Security.Permission.PagePreview, Security.Permission.PagePreview))
                        {
                            _logger?.LogInformation($"User not authorized to preview unpublished archive page");
                            authorized = false;
                        }
                    }

                    if (authorized)
                    {
                        service.PageId = response.PageId;
                        context.Request.Path = new PathString(response.Route);

                        if (context.Request.QueryString.HasValue)
                        {
                            context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&" + response.QueryString);
                        }
                        else
                        {
                            context.Request.QueryString = new QueryString("?" + response.QueryString);
                        }
                    }
                }
            }
            await _next.Invoke(context);
        }
    }
}
