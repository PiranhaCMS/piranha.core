/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Areas.Manager.Models
{
    public class PageListModel
    {
        #region Properties
        /// <summary>
        /// Gets/sets the available page types.
        /// </summary>
        public IList<Extend.PageType> PageTypes { get; set; }

        /// <summary>
        /// Gets/sets the current sitemap.
        /// </summary>
        public IList<Piranha.Models.SitemapItem> Sitemap { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageListModel() {
            PageTypes = new List<Extend.PageType>();
            Sitemap = new List<Piranha.Models.SitemapItem>();
        }

        /// <summary>
        /// Gets the page list view model.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The model</returns>
        public static PageListModel Get(IApi api) {
            var model = new PageListModel();

            model.PageTypes = App.PageTypes;
            model.Sitemap = api.Sitemap.Get(false);

            return model;
        }
    }
}
