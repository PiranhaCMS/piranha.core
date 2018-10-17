/*
 * Copyright (c) 2017-2018 Håkan Edling
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
        public static IRouteResponse Invoke(IApi api, string url, Guid siteId)
        {
            if (string.IsNullOrWhiteSpace(url) || url == "/")
            {
                var page = api.Pages.GetStartpage<Models.PageInfo>(siteId);

                if (page != null)
                {
                    if (page.ContentType == "Page")
                    {
                        var site = api.Sites.GetById(siteId);
                        var lastModified = !site.ContentLastModified.HasValue || page.LastModified > site.ContentLastModified 
                            ? page.LastModified : site.ContentLastModified.Value;

                        return new RouteResponse
                        {
                            PageId = page.Id,
                            Route = page.Route ?? "/page",
                            QueryString = "id=" + page.Id + "&startpage=true&piranha_handled=true",
                            IsPublished = page.Published.HasValue && page.Published.Value <= DateTime.Now,
                            CacheInfo = new HttpCacheInfo
                            {
                                EntityTag = Utils.GenerateETag(page.Id.ToString(), lastModified),
                                LastModified = lastModified
                            }
                        };
                    }
                    else if (page.ContentType == "Blog")
                    {
                        return ArchiveRouter.Invoke(api, $"/{page.Slug}", siteId);
                    }
                }
            }
            return null;
        }
    }
}
