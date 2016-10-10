/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Piranha.AspNet
{
	public class PostMiddleware : MiddlewareBase
	{
		/// <summary>
		/// Creates a new middleware instance.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline</param>
		public PostMiddleware(RequestDelegate next, IApi api) : base(next, api) { }

		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <returns>An async task</returns>
		public override async Task Invoke(HttpContext context) {
			if (!IsHandled(context)) {
				var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

				if (!String.IsNullOrWhiteSpace(url) && url.Length > 1) {
					var segments = url.Substring(1).Split(new char[] { '/' });

					var include = segments.Length;

					// Check that we have at least two segments
					if (segments.Length >= 2) {
						var post = api.Posts.GetBySlug(segments[0], segments[1]);

						if (post != null) {
							var route = post.Route ?? "/post";

							if (segments.Length > 2) {
								route += "/" + segments.Subset(2).Implode("/");
							}

							// Set path
							context.Request.Path = new PathString(route);

							// Set query
							if (context.Request.QueryString.HasValue) {
								context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&id=" + post.Id + "&piranha_handled=true");
							} else context.Request.QueryString = new QueryString("?id=" + post.Id + "&piranha_handled=true");
						}
					}
				}
			}
			await next.Invoke(context);
		}
	}
}
