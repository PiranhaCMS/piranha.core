﻿/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Web
{
    public class PostRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="siteId">The requested site id</param>
        /// <returns>The piranha response, null if no matching post was found</returns>
        public static IRouteResponse Invoke(IApi api, string url, Guid siteId) {
            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1) {
                var segments = url.Substring(1).Split('/');

                if (segments.Length >= 2) {
                    var post = api.Posts.GetBySlug(segments[0], segments[1], siteId);                    

                    if (post != null) {
                        var route = post.Route ?? "/post";

                        if (segments.Length > 2) {
                            route += "/" + string.Join("/", segments.Subset(2));
                        }

                        return new RouteResponse
                        {
                            Route = route,
                            QueryString = $"id={post.Id}&piranha_handled=true",
                            IsPublished = post.Published.HasValue && post.Published.Value <= DateTime.Now,
                            CacheInfo = new HttpCacheInfo
                            {
                                EntityTag = Utils.GenerateETag(post.Id.ToString(), post.LastModified),
                                LastModified = post.LastModified
                            }
                        };
                    }
                }
            }
            return null;
        }
    }
}
