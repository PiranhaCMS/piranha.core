/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    public class Sitemap : List<SitemapItem>
    {
        /// <summary>
        /// Gets the partial sitemap with the pages positioned
        /// below the page with the given id.
        /// </summary>
        /// <param name="pageId">The page id</param>
        /// <returns>The partial sitemap</returns>
        public Sitemap GetPartial(string pageId) {
            return GetPartialRecursive(this, pageId);
        }

        /// <summary>
        /// Gets the breadcrumb for the page with the given id.
        /// </summary>
        /// <param name="pageId">The page id</param>
        /// <returns>The breadcrumb</returns>
        public IList<SitemapItem> GetBreadcrumb(string pageId) {
            return GetBreadcrumbRecursive(this, pageId);
        }

        /// <summary>
        /// Gets the partial sitemap by going through the
        /// sitemap recursively.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="pageId">The page id</param>
        /// <returns>The partial sitemap if found</returns>
        private Sitemap GetPartialRecursive(IList<SitemapItem> items, string pageId) {
            foreach (var item in items) {
                if (item.Id == pageId) {
                    return (Sitemap)item.Items;
                } else {
                    var partial = GetPartialRecursive(item.Items, pageId);

                    if (partial != null)
                        return (Sitemap)partial;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the breadcrumb items by going through the
        /// sitemap recursively.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="pageId">The page id</param>
        /// <returns>The breadcrumb items</returns>
        private IList<SitemapItem> GetBreadcrumbRecursive(IList<SitemapItem> items, string pageId) {
            foreach (var item in items) {
                if (item.Id == pageId) {
                    return new List<SitemapItem>() {
                        item
                    };
                } else {
                    var crumb = GetBreadcrumbRecursive(item.Items, pageId);

                    if (crumb != null) {
                        crumb.Insert(0, item);

                        return crumb;
                    }
                }
            }
            return null;
        }
    }
}