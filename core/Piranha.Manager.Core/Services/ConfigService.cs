/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class ConfigService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ConfigService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the config model.
        /// </summary>
        /// <returns>The model</returns>
        public ConfigModel Get()
        {
            using (var config = new Config(_api))
            {
                return new ConfigModel
                {
                    HierarchicalPageSlugs = config.HierarchicalPageSlugs,
                    ExpandedSitemapLevels = config.ManagerExpandedSitemapLevels,
                    ArchivePageSize = config.ArchivePageSize,
                    PagesExpires = config.CacheExpiresPages,
                    PostsExpires = config.CacheExpiresPosts,
                    MediaCDN = config.MediaCDN,
                    PageRevisions = config.PageRevisions,
                    PostRevisions = config.PostRevisions
                };
            }
        }

        /// <summary>
        /// Saves the given config model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        public void Save(ConfigModel model)
        {
            using (var config = new Config(_api))
            {
                config.HierarchicalPageSlugs = model.HierarchicalPageSlugs;
                config.ManagerExpandedSitemapLevels = model.ExpandedSitemapLevels;
                config.ArchivePageSize = model.ArchivePageSize;
                config.CacheExpiresPages = model.PagesExpires;
                config.CacheExpiresPosts = model.PostsExpires;
                config.MediaCDN = model.MediaCDN;
                config.PageRevisions = model.PageRevisions;
                config.PostRevisions = model.PostRevisions;
            }
        }
    }
}