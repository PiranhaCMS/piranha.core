/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Http;

namespace Piranha.AspNetCore.Http;

/// <summary>
/// Helper class for handling HTTP cache headers.
/// </summary>
public static class HttpCaching
{
    /// <summary>
    /// Checks if the client has the current version cached.
    /// </summary>
    /// <param name="context">The HTTP Context</param>
    /// <param name="etag">The entity tag</param>
    /// <param name="lastModified">The modification date</param>
    /// <returns>If the client has the same version cached</returns>
    public static bool IsCached(HttpContext context, string etag, DateTime lastModified)
    {
        var clientInfo = Get(context);

        if (clientInfo.EntityTag == etag)
        {
            return true;
        }

        if (clientInfo.LastModified.HasValue)
        {
            return clientInfo.LastModified.Value >= lastModified;
        }
        return false;
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
            catch
            {
                info.LastModified = null;
            }
        }
        return info;
    }
}
