/*
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
    public class StartPageRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="hostname">The optional hostname</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static IRouteResponse Invoke(IApi api, string url, string hostname) {
            if (string.IsNullOrWhiteSpace(url) || url == "/") {
                Data.Site site = null;
                
                if (!string.IsNullOrWhiteSpace(hostname))
                    site = api.Sites.GetByHostname(hostname);
                if (site == null)
                    site = api.Sites.GetDefault();


                var page = api.Pages.GetStartpage(site.Id);

                if (page != null) {
                    return new RouteResponse() {
                        Route = page.Route ?? "/page",
                        QueryString = "id=" + page.Id + "&startpage=true&piranha_handled=true",
                            IsPublished = page.Published.HasValue && page.Published.Value <= DateTime.Now,
                        CacheInfo = new HttpCacheInfo() {
                            EntityTag = Utils.GenerateETag(page.Id, page.LastModified),
                            LastModified = page.LastModified
                        }
                    };
                }
            }
            return null;
        }
    }
}
