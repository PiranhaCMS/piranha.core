/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for all content that can be routed to directly
    /// in the client application.
    /// </summary>
    [Serializable]
    public abstract class RoutedContent : Content
    {
        /// <summary>
        /// Gets/sets the optional navigation title.
        /// </summary>
        [StringLength(128)]
        public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        [StringLength(128)]
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional meta title.
        /// </summary>
        [StringLength(128)]
        public string MetaTitle { get; set; }

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
        /// Gets/sets the route.
        /// </summary>
        [StringLength(256)]
        public string Route { get; set; }
    }
}