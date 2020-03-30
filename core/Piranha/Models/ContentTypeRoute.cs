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

namespace Piranha.Models
{
    [Serializable]
    public sealed class ContentTypeRoute
    {
        /// <summary>
        /// Gets/sets the display title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the internal route.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Implicit type conversion between a route and a string.
        /// </summary>
        public static implicit operator string(ContentTypeRoute r)
        {
            return r.Route;
        }
    }
}