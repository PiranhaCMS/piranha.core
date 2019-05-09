/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Meta information for regions.
    /// </summary>
    public class RegionMeta : ContentMeta
    {
        /// <summary>
        /// Gets/sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets if  this is a collection region.
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// Gets/sets how the region should be display (content/full/setting)
        /// </summary>
        public string Display { get; set; }
    }
}