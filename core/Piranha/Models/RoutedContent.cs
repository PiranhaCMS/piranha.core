/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Abstract base class for templated content with a route.
    /// </summary>
    public abstract class RoutedContent : Content
    {
        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        [StringLength(128)]
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the public permalink.
        /// </summary>
        public string Permalink { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
        [StringLength(128)]
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
        [StringLength(256)]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the optional route used by the middleware.
        /// </summary>
        [StringLength(256)]
        public string Route { get; set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        public DateTime? Published { get; set; }
    }
}
