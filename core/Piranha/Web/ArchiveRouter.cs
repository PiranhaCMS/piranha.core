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
    public class ArchiveRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="hostname">The optional hostname</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static IRouteResponse Invoke(IApi api, string url, string hostname) {
            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1) {
                var segments = url.Substring(1).Split(new char[] { '/' });

                if (segments.Length >= 1) {
                    Data.Site site = null;
                    
                    if (!string.IsNullOrWhiteSpace(hostname))
                        site = api.Sites.GetByHostname(hostname);
                    if (site == null)
                        site = api.Sites.GetDefault();

                    var blog = api.Pages.GetBySlug(segments[0], site.Id);

                    if (blog != null && blog.ContentType == "Blog") {
                        var route = blog.Route ?? "/archive";

                        int? page = null;
                        int? year = null;
                        int? month = null;
                        bool foundPage = false;

                        for (var n = 1; n < segments.Length; n++) {
                            if (segments[n] == "page") {
                                foundPage = true;
                                continue;
                            }

                            if (foundPage) {
                                try {
                                    page = Convert.ToInt32(segments[n]);
                                } catch { }
                                break;
                            }

                            if (!year.HasValue) {
                                try {
                                    year = Convert.ToInt32(segments[n]);

                                    if (year.Value > DateTime.Now.Year)
                                        year = DateTime.Now.Year;
                                } catch { }
                            } else {
                                try {
                                    month = Math.Max(Math.Min(Convert.ToInt32(segments[n]), 12), 1);
                                } catch { }
                            }
                        }

                        return new RouteResponse() {
                            Route = route,
                            QueryString = $"id={blog.Id}&year={year}&month={month}&page={page}&piranha_handled=true",
                            IsPublished = true
                        };                            
                    }
                }
            }
            return null;
        }
    }
}
