/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data.RavenDb.Data;

public class PageRevision : ContentRevisionBase
{
    /// <summary>
    /// Gets/sets the id of the page this revision belongs to.
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// Gets/sets the site id, denormalized from the parent Page
    /// to avoid cross-collection navigation queries.
    /// </summary>
    public string SiteId { get; set; }

    /// <summary>
    /// Gets/sets the snapshot of the page's LastModified at the
    /// time this revision was created. Used for draft detection
    /// without loading the parent Page document.
    /// </summary>
    public DateTime PageLastModified { get; set; }
}
