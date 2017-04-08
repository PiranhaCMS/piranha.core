/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Models
{
    public class SitemapItem
    {
        #region Properties
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional navigation title.
        /// </summary>
        public string NavigationTitle { get; set; }

        /// <summary>
        /// Gets the menu title for the item. The menu title returns
        /// the navigation title if set, otherwise the main title.
        /// </summary>
        public string MenuTitle {
            get {
                if (!string.IsNullOrWhiteSpace(NavigationTitle))
                    return NavigationTitle;
                return Title;
            }
        }

        /// <summary>
        /// Gets/sets the unique permalink.
        /// </summary>
        public string Permalink { get; set; }

        /// <summary>
        /// Gets/sets the level in the hierarchy.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets/sets if the item is hidden.
        /// </summary>
        /// <returns></returns>
        public bool IsHidden { get; set;}

        /// <summary>
        /// Gets/sets the published date.
        /// </summary>        
        public DateTime? Published { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the available child items.
        /// </summary>
        public IList<SitemapItem> Items { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SitemapItem() {
            Items = new Sitemap();
        }

        /// <summary>
        /// Checks if the current sitemap item has a
        /// child item with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>If the child was found</returns>
        public bool HasChild(string id) {
            foreach (var item in Items) {
                if (item.Id == id || item.HasChild(id))
                    return true;
            }
            return false;
        }
    }
}
