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
    public class StartPageRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="siteId">The site id</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
        {
            if (string.IsNullOrWhiteSpace(url) || url == "/")
            {
                var page = await api.Pages.GetStartpageAsync<Models.PageInfo>(siteId)
                    .ConfigureAwait(false);

                if (page != null)
                {
                    var type = App.PageTypes.GetById(page.TypeId);

                    if (type != null)
                    {
                        if (!type.IsArchive)
                        {
                            var site = await api.Sites.GetByIdAsync(siteId).ConfigureAwait(false);
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

                        return await ArchiveRouter.InvokeAsync(api, $"/{page.Slug}", siteId).ConfigureAwait(false);
                    }
                }
            }
            return null;
        }
    }
}
