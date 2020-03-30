/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Threading.Tasks;

namespace Piranha.Web
{
    public class PageRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="siteId">The requested site id</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static async Task <IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
        {
            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1)
            {
                var segments = url.Substring(1).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                var include = segments.Length;

                // Scan for the most unique slug
                for (var n = include; n > 0; n--)
                {
                    var slug = string.Join("/", segments.Subset(0, n));
                    var page = await api.Pages.GetBySlugAsync<Models.PageInfo>(slug, siteId)
                        .ConfigureAwait(false);

                    if (page != null)
                    {
                        var type = App.PageTypes.GetById(page.TypeId);
                        if (type == null || type.IsArchive)
                        {
                            return null;
                        }

                        if (string.IsNullOrWhiteSpace(page.RedirectUrl))
                        {
                            var site = await api.Sites.GetByIdAsync(siteId);
                            var route = page.Route ?? "/page";
                            var lastModified = !site.ContentLastModified.HasValue || page.LastModified > site.ContentLastModified
                                ? page.LastModified : site.ContentLastModified.Value;

                            if (n < include)
                            {
                                route += "/" + string.Join("/", segments.Subset(n));
                            }

                            var isStartPage = !page.ParentId.HasValue && page.SortOrder == 0;

                            return new RouteResponse
                            {
                                PageId = page.Id,
                                Route = route,
                                QueryString = $"id={page.Id}&startpage={isStartPage.ToString().ToLower()}&piranha_handled=true",
                                IsPublished = page.Published.HasValue && page.Published.Value <= DateTime.Now,
                                CacheInfo = new HttpCacheInfo
                                {
                                    EntityTag = Utils.GenerateETag(page.Id.ToString(), lastModified),
                                    LastModified = lastModified
                                }
                            };
                        }

                        return new RouteResponse
                        {
                            IsPublished = page.Published.HasValue && page.Published.Value <= DateTime.Now,
                            RedirectUrl = page.RedirectUrl,
                            RedirectType = page.RedirectType
                        };
                    }
                }
            }
            return null;
        }
    }
}
