/*
 * Copyright (c) 2011-2017 Håkan Edling
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
    public sealed class Page : IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
	    public string Id { get; set; }

        /// <summary>
        /// Gets/sets the page type id.
        /// </summary>
        public string PageTypeId { get; set; }

        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
	    public string SiteId { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id. Used to
        /// position the page in the sitemap.
        /// </summary>
    	public string ParentId { get; set; }

        /// <summary>
        /// Gets/sets the pages sort order in its
        /// hierarchical position.
        /// </summary>
    	public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
	    public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional navigation title.
        /// </summary>
	    public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
	    public string Slug { get; set; }

        /// <summary>
        /// Gets/sets if the page should be visible
        /// in the navigation.
        /// </summary>
	    public bool IsHidden { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
	    public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
	    public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the optional route.
        /// </summary>
	    public string Route { get; set; }

        /// <summary>
        /// Gets/sets the publishe date.
        /// </summary>
	    public DateTime? Published { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        /// <returns></returns>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        /// <returns></returns>
        public RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
	    public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
	    public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<PageField> Fields { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Page() {
            Fields = new List<PageField>();
        }
    }
}
