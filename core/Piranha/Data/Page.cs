/*
 * Copyright (c) 2011-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Data
{
    public sealed class Page : RoutedContent<PageField>, IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the page type id.
        /// </summary>
        public string PageTypeId { get; set; }

        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
	    public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id. Used to
        /// position the page in the sitemap.
        /// </summary>
    	public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the type of content this page
        /// contains.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets/sets the pages sort order in its
        /// hierarchical position.
        /// </summary>
    	public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the optional navigation title.
        /// </summary>
	    public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets/sets if the page should be visible
        /// in the navigation.
        /// </summary>
	    public bool IsHidden { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        /// <returns></returns>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        /// <returns></returns>
        public Models.RedirectType RedirectType { get; set; } = Models.RedirectType.Temporary;

        /// <summary>
        /// Gets/sets the site.
        /// </summary>
        public Site Site { get; set; }

        /// <summary>
        /// Gets/sets the associated page type.
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// Gets/sets the optional page.
        /// </summary>
        public Page Parent { get; set; }

        /// <summary>
        /// Gets/sets the available page blocks.
        /// </summary>
        public IList<PageBlock> Blocks { get; set; } = new List<PageBlock>();

        /// <summary>
        /// Gets/sets the optional page this page is a copy of
        /// </summary>
        public Guid? OriginalPageId { get; set; }
    }
}
