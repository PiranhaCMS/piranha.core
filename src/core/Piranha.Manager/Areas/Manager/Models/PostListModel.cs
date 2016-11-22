/*
 * Copyright (c) 2016 Billy Wolfington
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
    public class PostListModel
    {
        #region Properties
        /// <summary>
        /// Gets/sets the available post types.
        /// </summary>
        public IList<Extend.PostType> PostTypes { get; set; }

        /// <summary>
        /// Gets/sets the current sitemap.
        /// </summary>
        public IList<Piranha.Models.SitemapItem> Sitemap { get; set; }
        #endregion
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostListModel() {
            PostTypes = new List<Extend.PostType>();
            Sitemap = new List<Piranha.Models.SitemapItem>();
        }

        public static PostListModel Get(IApi api, string category = null)
        {
            var model = new PostListModel();

            // TODO: Map to model roperties
            model.Sitemap = api.Sitemap.Get(false);

            return model;
        }
    }
}
