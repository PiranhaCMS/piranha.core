/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Threading.Tasks;

namespace Piranha.AspNet
{
	public class StartPageMiddleware : MiddlewareBase
	{
		/// <summary>
		/// Creates a new middleware instance.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline</param>
		public StartPageMiddleware(RequestDelegate next) : base(next) { }

		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <returns>An async task</returns>
		public override async Task Invoke(HttpContext context) {
			var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

			if (String.IsNullOrWhiteSpace(url) || url == "/") {
				var page = api.Pages.GetStartpage();

				if (page != null) {
					// Set path
					context.Request.Path = new PathString(page.Route);

					// Set query
					if (context.Request.QueryString.HasValue) {
						context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&id=" + page.Id);
					} else context.Request.QueryString = new QueryString("?id=" + page.Id);
				}
			}
			await next.Invoke(context);
		}
	}
}
