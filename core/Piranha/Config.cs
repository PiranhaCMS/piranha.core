/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Data;

namespace Piranha
{
    /// <summary>
    /// Class for easy access to built-in config parameters.
    /// </summary>
    public sealed class Config : IDisposable
    {
        #region Members
        /// <summary>
        /// The private api.
        /// </summary>
        private readonly IApi api;

        /// <summary>
        /// The private transaction.
        /// </summary>
        private readonly IDbTransaction transaction;

        /// <summary>
        /// The system config keys.
        /// </summary>
        public static readonly string CACHE_EXPIRES_PAGES = "CacheExpiresPages";
        public static readonly string PAGES_HIERARCHICAL_SLUGS = "HierarchicalPageSlugs";
        public static readonly string MANAGER_EXPANDED_SITEMAP_LEVELS = "ManagerExpandedSitemapLevels";
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the currently configured cache expiration
        /// in minutes for pages.
        /// </summary>
        public int CacheExpiresPages {
            get
            {
                var param = api.Params.GetByKey(CACHE_EXPIRES_PAGES, transaction);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = api.Params.GetByKey(CACHE_EXPIRES_PAGES, transaction);
                if (param == null) {
                    param = new Data.Param() {
                        Key = CACHE_EXPIRES_PAGES
                    };
                }
                param.Value = value.ToString();
                api.Params.Save(param, transaction);
            }
        }

        /// <summary>
        /// Gets/sets if hierarchical slugs should be generated when
        /// creating new pages.
        /// </summary>
        public bool HierarchicalPageSlugs {
            get {
                var param = api.Params.GetByKey(PAGES_HIERARCHICAL_SLUGS, transaction);
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return true;
            }
            set {
                var param = api.Params.GetByKey(PAGES_HIERARCHICAL_SLUGS, transaction);
                if (param == null) {
                    param = new Data.Param() {
                        Key = PAGES_HIERARCHICAL_SLUGS
                    };
                }
                param.Value = value.ToString();
                api.Params.Save(param, transaction);
            }
        }

        /// <summary>
        /// Gets/sets the default number of expanded sitemap levels
        /// in the manager interface.
        /// </summary>
        public int ManagerExpandedSitemapLevels {
            get {
                var param = api.Params.GetByKey(MANAGER_EXPANDED_SITEMAP_LEVELS, transaction);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = api.Params.GetByKey(MANAGER_EXPANDED_SITEMAP_LEVELS, transaction);
                if (param == null) {
                    param = new Data.Param() {
                        Key = MANAGER_EXPANDED_SITEMAP_LEVELS
                    };
                }
                param.Value = value.ToString();
                api.Params.Save(param, transaction);                
            }
        }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="tx"></param>
        public Config(IApi api, IDbTransaction tx = null) {
            this.api = api;
            this.transaction = tx;
        }

        /// <summary>
        /// Disposes the config.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}