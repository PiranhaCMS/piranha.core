/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Web
{
    public class StartPageRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static IRouteResponse Invoke(Api api, string url) {
            if (string.IsNullOrWhiteSpace(url) || url == "/") {
                var page = api.Pages.GetStartpage();

                if (page != null) {
                    return new RouteResponse() {
                        Route = page.Route ?? "/page",
                        QueryString = "id=" + page.Id + "&startpage=true&piranha_handled=true",
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
