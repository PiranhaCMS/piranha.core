/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    public sealed class ContentRoutes
    {
        /// <summary>
        /// Gets/sets the selected route.
        /// </summary>
        public RouteModel SelectedRoute { get; set; }

        /// <summary>
        /// Gets/sets the available routes.
        /// </summary>
        public IList<RouteModel> Routes { get; set; } = new List<RouteModel>();

        /// <summary>
        /// Gets/sets the optional redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        public string RedirectType { get; set; } = "permanent";
    }
}