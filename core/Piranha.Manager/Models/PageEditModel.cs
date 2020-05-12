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
using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Page edit model.
    /// </summary>
    public class PageEditModel : Content.ContentEditModel
    {
        /// <summary>
        /// Gets/sets the mandatory site id.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id. This determines the
        /// hierarchical position of the page.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the id of the original page if this is a copy.
        /// </summary>
        public Guid? OriginalId { get; set; }

        /// <summary>
        /// Gets/sets the sort order of the page in its
        /// hierarchical position.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the optional navigation title.
        /// </summary>
        public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets if the page should be hidden in the menu structure.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>
        public string Published { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        public string RedirectType { get; set; } = "permanent";

        /// <summary>
        /// Gets/sets if comments should be enabled.
        /// </summary>
        public bool EnableComments { get; set; }

        /// <summary>
        /// Gets/sets after how many days after publish date comments
        /// should be closed. A value of 0 means never.
        /// </summary>
        public int CloseCommentsAfterDays { get; set; }

        /// <summary>
        /// Gets/sets the total comment count.
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// Gets/sets the number of pending comments.
        /// </summary>
        /// <value></value>
        public int PendingCommentCount { get; set; }

        /// <summary>
        /// Gets/sets the content status.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets/sets if this is a copy.
        /// </summary>
        public bool IsCopy { get { return OriginalId.HasValue; } }

        /// <summary>
        /// Gets/sets the available routes.
        /// </summary>
        public List<RouteModel> Routes { get; set; } = new List<RouteModel>();

        /// <summary>
        /// Gets/sets the selected route.
        /// </summary>
        public RouteModel SelectedRoute { get; set; }

        /// <summary>
        /// Gets/sets the currently selected permissions.
        /// </summary>
        public IList<string> SelectedPermissions { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets all of the available permissions.
        /// </summary>
        public IList<KeyValuePair<string, string>> Permissions { get; set; } = new List<KeyValuePair<string, string>>();
    }
}