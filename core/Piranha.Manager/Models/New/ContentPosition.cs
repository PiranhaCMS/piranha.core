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

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Class for specifying the site position of
    /// a page.
    /// </summary>
    public sealed class ContentPosition
    {
        /// <summary>
        /// Gets/sets the current site id.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the display title of the site.
        /// </summary>
        public string SiteTitle { get; set; }

        /// <summary>
        /// Gets/sets the sort order in its hierarchical position.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets if the content should be hidden.
        /// </summary>
        public bool IsHidden { get; set; }
    }
}