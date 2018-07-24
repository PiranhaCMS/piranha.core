/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// View model for the page sitemap.
    /// </summary>
    public class SitemapModel
    {
        /// <summary>
        /// Gets/sets the sitemap.
        /// </summary>
        public IList<Piranha.Models.SitemapItem> Sitemap { get; set; }

        /// <summary>
        /// Gets/sets the optional site id.
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Gets/sets the optional selected page id.
        /// </summary>
        public string PageId { get; set; }

        /// <summary>
        /// Gets/sets the default number of expanded levels.
        /// </summary>
        public int ExpandedLevels { get; set; }

        /// <summary>
        /// Gets/sets if the model is readonly for modals.
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}