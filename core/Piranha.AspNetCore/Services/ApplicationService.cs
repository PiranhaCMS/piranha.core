/*
 * Copyright (c) 2018 Håkan Edling
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
using Piranha.Models;

namespace Piranha.AspNetCore.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApi _api;

        /// <summary>
        /// Gets the current api.
        /// </summary>
        public IApi Api { get; private set; }

        /// <summary>
        /// Gets the currently requested URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets the id of the currently requested site.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets the id of the currently requested page.
        /// </summary>
        public Guid PageId { get; set; }
        
        /// <summary>
        /// Gets the sitemap of the currently requested site.
        /// </summary>
        public Sitemap Sitemap { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ApplicationService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public void Init(HttpContext context)
        {
            // Gets the current site info
            if (!context.Request.Path.Value.StartsWith("/manager/"))
            {
                Data.Site site = null;

                // Try to get the requested site by hostname & prefix
                var url = context.Request.Path.HasValue ? context.Request.Path.Value : "";
                if (!string.IsNullOrEmpty(url) && url.Length > 1)
                {
                    var segments = url.Substring(1).Split(new char[] { '/' });
                    site = _api.Sites.GetByHostname($"{context.Request.Host.Host}/{segments[0]}");

                    if (site != null)
                        context.Request.Path = "/" + string.Join("/", segments.Skip(1));
                }

                // Try to get the requested site by hostname
                if (site == null)
                    site = _api.Sites.GetByHostname(context.Request.Host.Host);

                // If we didn't find the site, get the default site
                if (site == null)
                    site = _api.Sites.GetDefault();

                // Store the current site id for the current request
                if (site != null)
                {
                    SiteId = site.Id;                    
                    Sitemap = _api.Sites.GetSitemap(SiteId);
                }
            }

            // Get the current url
            Url = context.Request.Path.Value;
        }

        /// <summary>
        /// Gets the site content for the current site.
        /// </summary>
        /// <typeparam name="T">The content type</typeparam>
        /// <returns>The site content model</returns>
        public T GetSiteContent<T>() where T : SiteContent<T>
        {
            if (SiteId != Guid.Empty)
            {
                return _api.Sites.GetContentById<T>(SiteId);
            }
            return null;
        }
    }
}
