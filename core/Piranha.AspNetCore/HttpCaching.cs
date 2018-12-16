/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using Piranha.Web;
using System;

namespace Piranha.AspNetCore
{
    /// <summary>
    /// Helper class for handling HTTP cache headers.
    /// </summary>
    public static class HttpCaching
    {
        /// <summary>
        /// Checks if the client has the current version cached.
        /// </summary>
        /// <param name="context">The HTTP Context</param>
        /// <param name="serverInfo">The service info</param>
        /// <returns></returns>
        public static bool IsCached(HttpContext context, HttpCacheInfo serverInfo)
        {
            var clientInfo = Get(context);

            if (clientInfo.EntityTag == serverInfo.EntityTag)
                return true;

            if (clientInfo.LastModified.HasValue)
                return clientInfo.LastModified.Value >= serverInfo.LastModified.Value;

            return false;
        }

        /// <summary>
        /// Gets the HTTP Cache info for the given page.
        /// </summary>
        /// <param name="page">The page</param>
        /// <returns>The cache info</returns>
        public static HttpCacheInfo Get(Models.PageBase page)
        {
            return new HttpCacheInfo
            {
                EntityTag = Utils.GenerateETag(page.Id.ToString(), page.LastModified),
                LastModified = page.LastModified
            };
        }

        /// <summary>
        /// Gets the HTTP Cache info for the given context
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>The cache info</returns>
        public static HttpCacheInfo Get(HttpContext context)
        {
            var info = new HttpCacheInfo
            {
                EntityTag = context.Request.Headers["If-None-Match"]

            };

            string lastMod = context.Request.Headers["If-Modified-Since"];
            if (!string.IsNullOrWhiteSpace(lastMod))
            {
                try
                {
                    info.LastModified = DateTime.Parse(lastMod);
                }
                catch { }
            }
            return info;
        }
    }
}