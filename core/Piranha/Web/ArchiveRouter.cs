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
    public class ArchiveRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="siteId">The requested site id</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
        {
            if (string.IsNullOrWhiteSpace(url) || url.Length <= 1)
            {
                return null;
            }

            var segments = url.Substring(1).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 1) return null;

            // Scan for the most unique slug
            for (var i = segments.Length; i > 0; i--)
            {
                var slug = string.Join("/", segments.Subset(0, i));
                var blog = await api.Pages.GetBySlugAsync<Models.PageInfo>(slug, siteId)
                    .ConfigureAwait(false);

                if (blog == null) continue;

                var type = App.PageTypes.GetById(blog.TypeId);
                if (type == null || !type.IsArchive)
                {
                    return null;
                }

                // First check that this is a valid archive URL
                if (segments.Length == i + 1)
                {
                    try
                    {
                        var number = Convert.ToInt32(segments[i]);
                        if (number < 1900 || number > DateTime.Now.Year)
                            return null;
                    }
                    catch
                    {
                        return null;
                    }
                }

                var route = blog.Route ?? "/archive";

                int? page = null;
                int? year = null;
                int? month = null;
                Guid? categoryId = null;
                Guid? tagId = null;
                bool foundCategory = false;
                bool foundTag = false;
                bool foundPage = false;

                for (var n = i; n < segments.Length; n++)
                {
                    if (segments[n] == "category" && !foundPage)
                    {
                        foundCategory = true;
                        continue;
                    }

                    if (segments[n] == "tag" && !foundPage && !foundCategory)
                    {
                        foundTag = true;
                        continue;
                    }

                    if (segments[n] == "page")
                    {
                        foundPage = true;
                        continue;
                    }

                    if (foundCategory)
                    {
                        try
                        {
                            categoryId = (await api.Posts.GetCategoryBySlugAsync(blog.Id, segments[n]).ConfigureAwait(false))?.Id ??
                                         Guid.Empty;
                        }
                        finally
                        {
                            foundCategory = false;
                        }
                    }

                    if (foundTag)
                    {
                        try
                        {
                            tagId = (await api.Posts.GetTagBySlugAsync(blog.Id, segments[n]).ConfigureAwait(false))?.Id ??
                                    Guid.Empty;
                        }
                        finally
                        {
                            foundTag = false;
                        }
                    }

                    if (foundPage)
                    {
                        try
                        {
                            page = Convert.ToInt32(segments[n]);
                        }
                        catch
                        {
                            // We don't care about the exception, we just
                            // discard malformed input
                        }
                        break;
                    }

                    if (!year.HasValue)
                    {
                        try
                        {
                            year = Convert.ToInt32(segments[n]);

                            if (year.Value > DateTime.Now.Year)
                                year = DateTime.Now.Year;
                        }
                        catch
                        {
                            // We don't care about the exception, we just
                            // discard malformed input
                        }
                    }
                    else
                    {
                        try
                        {
                            month = Math.Max(Math.Min(Convert.ToInt32(segments[n]), 12), 1);
                        }
                        catch
                        {
                            // We don't care about the exception, we just
                            // discard malformed input
                        }
                    }
                }

                return new RouteResponse
                {
                    PageId = blog.Id,
                    Route = route,
                    QueryString = $"id={blog.Id}&year={year}&month={month}&page={page}&pagenum={page}&category={categoryId}&tag={tagId}&piranha_handled=true",
                    IsPublished = blog.Published.HasValue && blog.Published.Value <= DateTime.Now,
                    CacheInfo = new HttpCacheInfo
                    {
                        EntityTag = Utils.GenerateETag(blog.Id.ToString(), blog.LastModified),
                        LastModified = blog.LastModified
                    }
                };
            }
            return null;
        }
    }
}
