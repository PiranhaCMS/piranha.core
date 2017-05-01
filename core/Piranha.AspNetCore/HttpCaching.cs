/*
 * Copyright (c) 2017 Håkan Edling
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
    public static class HttpCaching
    {
        public static bool IsCached(HttpContext context, HttpCacheInfo serverInfo) {
            var clientInfo = Get(context);

            if (clientInfo.EntityTag == serverInfo.EntityTag)
                return true;

            if (clientInfo.LastModified.HasValue)
                return clientInfo.LastModified.Value >= serverInfo.LastModified.Value;
            
            return false;
        }

        public static HttpCacheInfo Get(Models.PageBase page) {
            return new HttpCacheInfo() {
                EntityTag = Utils.GenerateETag(page.Id, page.LastModified),
                LastModified = page.LastModified
            };
        }

        public static HttpCacheInfo Get(HttpContext context) {
            var info = new HttpCacheInfo();

            info.EntityTag = context.Request.Headers["If-None-Match"];

            string lastMod = context.Request.Headers["If-Modified-Since"];
            if (!string.IsNullOrWhiteSpace(lastMod)) {
                try {
                    info.LastModified = DateTime.Parse(lastMod);
                } catch {}
            }
            return info;
        }
    }
}