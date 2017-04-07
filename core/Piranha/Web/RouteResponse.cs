/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Web
{
    public class RouteResponse : IRouteResponse
    {
        /// <summary>
        /// Gets/sets the route.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets/sets the optional query string.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect url.
        /// </summary>
        public string RedirectUrl { get; set;}

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        public Data.RedirectType RedirectType { get; set; }
    }
}
