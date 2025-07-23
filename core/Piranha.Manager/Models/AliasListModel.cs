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
/// Alias model.
/// </summary>
public class AliasListModel
{
    /// <summary>
    /// A list item in the alias model.
    /// </summary>
    public class AliasItem
    {
        /// <summary>
        /// Gets/sets the optional id.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the alias url.
        /// </summary>
        public string AliasUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if the redirect should be permanent.
        /// </summary>
        public bool IsPermanent { get; set; }
    }

    public class AliasSite
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// Gets/sets the current site id.
    /// </summary>
    public Guid SiteId { get; set; }

    /// <summary>
    /// Gets/sets the current site title.
    /// </summary>
    public string SiteTitle { get; set; }

    /// <summary>
    /// Gets/sets the available sites.
    /// </summary>
    public IList<AliasSite> Sites { get; set; } = new List<AliasSite>();

    /// <summary>
    /// Gets/set the available items.
    /// </summary>
    public IList<AliasItem> Items { get; set; } = new List<AliasItem>();

    /// <summary>
    /// Gets/sets the optional status message from the last operation.
    /// </summary>
    public StatusMessage Status { get; set; }
}
