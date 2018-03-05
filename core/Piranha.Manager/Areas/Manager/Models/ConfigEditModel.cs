/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Areas.Manager.Models
{
    public class ConfigEditModel
    {
        /// <summary>
        /// Inner class for cache configuration settings.
        /// </summary>
        public class CacheConfig
        {
            /// <summary>
            /// Gets/sets the cache expiration for pages.
            /// </summary>
            public int PagesExpires { get; set; }

            /// <summary>
            /// Gets/sets the cache expiration for posts.
            /// </summary>
            public int PostsExpires { get; set; }

            /// <summary>
            /// Gets/sets the optional CDN for uploaded media.
            /// </summary>
            public string MediaCDN { get; set; }
        }

        /// <summary>
        /// Inner class for general configuration settings.
        /// </summary>
        public class GeneralConfig
        {
            /// <summary>
            /// Gets/sets the archive page size.
            /// </summary>
            public int ArchivePageSize { get; set; }

            /// <summary>
            /// Gets/sets if hierarchical slugs should be used.
            /// </summary>
            public bool HierarchicalPageSlugs { get; set; }

            /// <summary>
            /// Gets/sets the default expanded levels in the manager sitemap.
            /// </summary>
            public int ExpandedSitemapLevels { get; set; }
        }

        /// <summary>
        /// Gets/sets the cache config.
        /// </summary>
        public CacheConfig Cache { get; set; }

        /// <summary>
        /// Gets/sets the general config.
        /// </summary>
        public GeneralConfig General { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigEditModel() {
            Cache = new CacheConfig();
            General = new GeneralConfig();
        }

        /// <summary>
        /// Gets the cache edit model.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The model</returns>
        public static ConfigEditModel Get(IApi api) {
            var model = new ConfigEditModel();

            using (var config = new Config(api)) {
                model.Cache.PagesExpires = config.CacheExpiresPages;
                model.Cache.PostsExpires = config.CacheExpiresPosts;
                model.Cache.MediaCDN = config.MediaCDN;

                model.General.ArchivePageSize = config.ArchivePageSize;
                model.General.HierarchicalPageSlugs = config.HierarchicalPageSlugs;
                model.General.ExpandedSitemapLevels = config.ManagerExpandedSitemapLevels;
            }
            return model;
        }

        /// <summary>
        /// Saves the current config model.
        /// </summary>
        /// <param name="api">The current api</param>
        public void Save(IApi api) {
            using (var config = new Config(api)) {
                config.CacheExpiresPages = Cache.PagesExpires;
                config.CacheExpiresPosts = Cache.PostsExpires;
                config.MediaCDN = Cache.MediaCDN;
                config.ArchivePageSize = General.ArchivePageSize;
                config.HierarchicalPageSlugs = General.HierarchicalPageSlugs;
                config.ManagerExpandedSitemapLevels = General.ExpandedSitemapLevels;
            }
        }
    }
}