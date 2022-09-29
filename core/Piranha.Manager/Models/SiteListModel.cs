/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models;

/// <summary>
/// Site list model.
/// </summary>
public class SiteListModel
{
    /// <summary>
    /// Gets/sets the currently selected site id.
    /// </summary>
    public Guid SiteId { get; set; }

    /// <summary>
    /// Gets/sets the title of the currently selected site.
    /// </summary>
    public string SiteTitle { get; set; }

    /// <summary>
    /// Gets/sets the available sites.
    /// </summary>
    public IList<PageListModel.PageSite> Sites { get; set; } = new List<PageListModel.PageSite>();

    /// <summary>
    /// Gets/sets the items in the currently selected site.
    /// </summary>
    public IList<PageListModel.PageItem> Items { get; set; } = new List<PageListModel.PageItem>();
}
