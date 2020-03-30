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

namespace Piranha.Models
{
    [Serializable]
    public class SitemapItem : StructureItem<Sitemap, SitemapItem>
    {
        /// <summary>
        /// Gets/sets the optional original id.
        /// </summary>
        public Guid? OriginalPageId { get; set; }

        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the sort order.
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
        /// Gets the menu title for the item. The menu title returns
        /// the navigation title if set, otherwise the main title.
        /// </summary>
        public string MenuTitle
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NavigationTitle))
                {
                    return NavigationTitle;
                }
                return Title;
            }
        }

        /// <summary>
        /// Gets/sets the name of the page type.
        /// </summary>
        public string PageTypeName { get; set; }

        /// <summary>
        /// Gets/sets the unique permalink.
        /// </summary>
        public string Permalink { get; set; }

        /// <summary>
        /// Gets/sets if the item is hidden.
        /// </summary>
        /// <returns></returns>
        public bool IsHidden { get; set; }

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
        /// Default constructor.
        /// </summary>
        public SitemapItem()
        {
            Items = new Sitemap();
        }

        /// <summary>
        /// Checks if the current sitemap item has a
        /// child item with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>If the child was found</returns>
        public bool HasChild(Guid id)
        {
            foreach (var item in Items)
            {
                if (item.Id == id || item.HasChild(id))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
