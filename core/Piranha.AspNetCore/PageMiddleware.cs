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
using Piranha.Web;
using System.Threading.Tasks;

namespace Piranha.AspNetCore
{
    public class PageMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="api">The current api</param>
        public PageMiddleware(RequestDelegate next, Api api) : base(next, api) { }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public override async Task Invoke(HttpContext context) {
            if (!IsHandled(context) && !context.Request.Path.Value.StartsWith("/manager/assets/")) {
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

                var response = PageRouter.Invoke(api, url);
                if (response != null) {
                    if (string.IsNullOrWhiteSpace(response.RedirectUrl)) {
                        context.Request.Path = new PathString(response.Route);

                        if (context.Request.QueryString.HasValue) {
                            context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&" + response.QueryString);
                        } else context.Request.QueryString = new QueryString("?" + response.QueryString);
                    } else {
                        context.Response.Redirect(response.RedirectUrl, response.RedirectType == Data.RedirectType.Permanent);
                        return;
                    }
                }
            }
            await next.Invoke(context);
        }
    }
}
