/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;

namespace Piranha.Data
{
    public class PageRevision : ContentRevisionBase
    {
        /// <summary>
        /// Gets/sets the id of the page this revision
        /// belongs to.
        /// </summary>
        public Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the page this revision belongs to.
        /// </summary>
        public Page Page { get; set; }
    }
}