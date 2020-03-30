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

namespace Piranha.Web
{
    public interface IRouteResponse
    {
        /// <summary>
        /// Gets/sets the page id.
        /// </summary>
        Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the route.
        /// </summary>
        string Route { get; set; }

        /// <summary>
        /// Gets/sets the optional query string.
        /// </summary>
        string QueryString { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect url.
        /// </summary>
        string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if the route is published or not.
        /// </summary>
        bool IsPublished { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        Models.RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets/sets the cache info.
        /// </summary>
        HttpCacheInfo CacheInfo { get; set; }
    }
}
