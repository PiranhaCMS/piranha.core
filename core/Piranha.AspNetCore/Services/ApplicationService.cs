/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.AspNetCore.Services
{
    public class ApplicationService : IApplicationService
    {
        public class SiteHelper : ISiteHelper
        {
            private readonly IApi _api;

            /// <summary>
            /// Gets the id of the currently requested site.
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Gets/sets the optional culture of the requested site.
            /// </summary>
            public string Culture { get; set; }

            /// <summary>
            /// Gets the sitemap of the currently requested site.
            /// </summary>
            public Sitemap Sitemap { get; set; }

            /// <summary>
            /// Default internal constructur.
            /// </summary>
            internal SiteHelper(IApi api)
            {
                _api = api;
            }

            /// <summary>
            /// Gets the site content for the current site.
            /// </summary>
            /// <typeparam name="T">The content type</typeparam>
            /// <returns>The site content model</returns>
            public Task<T> GetContentAsync<T>() where T : SiteContent<T>
            {
                if (Id != Guid.Empty)
                {
                    return _api.Sites.GetContentByIdAsync<T>(Id);
                }
                return null;
            }
        }

        public class MediaHelper : IMediaHelper
        {
            private readonly IApi _api;

            /// <summary>
            /// Default internal constructur.
            /// </summary>
            internal MediaHelper(IApi api)
            {
                _api = api;
            }

            /// <summary>
            /// Resizes the given image to the given dimensions.
            /// </summary>
            /// <param name="image"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public string ResizeImage(ImageField image, int width, int? height = null)
            {
                if (image.Id.HasValue)
                {
                    return _api.Media.EnsureVersion(image.Id.Value, width, height);
                }
                return null;
            }

            /// <summary>
            /// Resizes the given image to the given dimensions.
            /// </summary>
            /// <param name="image"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public string ResizeImage(Media image, int width, int? height = null)
            {
                if (image.Id != Guid.Empty && image.Type == MediaType.Image)
                {
                    return _api.Media.EnsureVersion(image.Id, width, height);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the current api.
        /// </summary>
        public IApi Api { get; }

        /// <summary>
        /// Gets the site helper.
        /// </summary>
        public ISiteHelper Site { get; internal set; }

        /// <summary>
        /// Gets the media helper.
        /// </summary>
        public IMediaHelper Media { get; internal set; }

        /// <summary>
        /// Gets the currently requested URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets the id of the currently requested page.
        /// </summary>
        public Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the current page.
        /// </summary>
        public PageBase CurrentPage { get; set; }

        /// <summary>
        /// Gets/sets the current post.
        /// </summary>
        public PostBase CurrentPost { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ApplicationService(IApi api)
        {
            Api = api;

            Site = new SiteHelper(api);
            Media = new MediaHelper(api);
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public async Task InitAsync(HttpContext context)
        {
            // Gets the current site info
            if (!context.Request.Path.Value.StartsWith("/manager/"))
            {
                Site site = null;

                // Try to get the requested site by hostname & prefix
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                if (!string.IsNullOrEmpty(url) && url.Length > 1)
                {
                    var segments = url.Substring(1).Split(new char[] { '/' });
                    site = await Api.Sites.GetByHostnameAsync($"{context.Request.Host.Host}/{segments[0]}");

                    if (site != null)
                        context.Request.Path = "/" + string.Join("/", segments.Skip(1));
                }

                // Try to get the requested site by hostname
                if (site == null)
                    site = await Api.Sites.GetByHostnameAsync(context.Request.Host.Host);

                // If we didn't find the site, get the default site
                if (site == null)
                    site = await Api.Sites.GetDefaultAsync();

                // Store the current site id & get the sitemap
                if (site != null)
                {
                    Site.Id = site.Id;
                    Site.Culture = site.Culture;
                    Site.Sitemap = await Api.Sites.GetSitemapAsync(Site.Id);
                }
            }

            // Get the current url
            Url = context.Request.Path.Value;
        }
    }
}
