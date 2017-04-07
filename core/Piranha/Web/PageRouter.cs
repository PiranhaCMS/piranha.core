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
    public class PageRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static IRouteResponse Invoke(Api api, string url) {
            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1) {
                var segments = url.Substring(1).Split(new char[] { '/' });

                var include = segments.Length;

                // Scan for the most unique slug
                for (var n = include; n > 0; n--) {
                    var slug = string.Join("/", segments.Subset(0, n));
                    var page = api.Pages.GetBySlug(slug);

                    if (page != null) {
                        if (string.IsNullOrWhiteSpace(page.RedirectUrl)) {
                            var route = page.Route ?? "/page";

                            if (n < include) {
                                route += "/" + string.Join("/", segments.Subset(n));
                            }

                            return new RouteResponse() {
                                Route = route,
                                QueryString = $"id={page.Id}&startpage={page.IsStartPage}&piranha_handled=true"
                            };
                        } else {
                            return new RouteResponse() {
                                RedirectUrl = page.RedirectUrl,
                                RedirectType = page.RedirectType
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
