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
	public class PostMiddleware : MiddlewareBase
	{
		/// <summary>
		/// Creates a new middleware instance.
		/// </summary>
		/// <param name="next">The next middleware in the pipeline</param>
		public PostMiddleware(RequestDelegate next) : base(next) { }

		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		/// <param name="context">The current http context</param>
		/// <returns>An async task</returns>
		public override async Task Invoke(HttpContext context) {
			var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";

			if (!String.IsNullOrWhiteSpace(url) && url.Length > 2) {
				var segments = url.Substring(1).Split(new char[] { '/' });

				var category = api.Categories.GetBySlug(segments[0]);

				if (category != null) {
					var post = api.Posts.GetBySlug(category.Id, segments[1]);

					if (post != null) {
						// Get the route
						var route = post.Route;
						if (segments.Length > 2)
							route += "/" + segments.Subset(2).Implode("/");

						// Set path
						context.Request.Path = new PathString(route);

						// Set query
						if (context.Request.QueryString.HasValue) {
							context.Request.QueryString = new QueryString(context.Request.QueryString.Value + "&id=" + post.Id);
						} else context.Request.QueryString = new QueryString("?id=" + post.Id);
					}
				}
			}
			await next.Invoke(context);
		}
	}
}
