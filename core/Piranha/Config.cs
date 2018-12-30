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
using Piranha.Models;
using Piranha.Services;

namespace Piranha
{
    /// <summary>
    /// Class for easy access to built-in config parameters.
    /// </summary>
    public sealed class Config : IDisposable
    {
        /// <summary>
        /// The private api.
        /// </summary>
        private readonly IApi _api;

        /// <summary>
        /// The system config keys.
        /// </summary>
        public static readonly string ARCHIVE_PAGE_SIZE = "ArchivePageSize";
        public static readonly string CACHE_EXPIRES_PAGES = "CacheExpiresPages";
        public static readonly string CACHE_EXPIRES_POSTS = "CacheExpiresPosts";
        public static readonly string PAGES_HIERARCHICAL_SLUGS = "HierarchicalPageSlugs";
        public static readonly string MEDIA_CDN_URL = "MediaCdnUrl";
        public static readonly string MANAGER_EXPANDED_SITEMAP_LEVELS = "ManagerExpandedSitemapLevels";

        /// <summary>
        /// Gets/sets the currently configured archive page size.
        /// </summary>
        public int ArchivePageSize {
            get {
                var param = _api.Params.GetByKey(ARCHIVE_PAGE_SIZE);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _api.Params.GetByKey(ARCHIVE_PAGE_SIZE);
                if (param == null)
                {
                    param = new Param
                    {
                        Key = ARCHIVE_PAGE_SIZE
                    };
                }
                param.Value = value.ToString();
                _api.Params.Save(param);
            }
        }

        /// <summary>
        /// Gets/sets the currently configured cache expiration
        /// in minutes for pages.
        /// </summary>
        public int CacheExpiresPages {
            get {
                var param = _api.Params.GetByKey(CACHE_EXPIRES_PAGES);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _api.Params.GetByKey(CACHE_EXPIRES_PAGES);
                if (param == null)
                {
                    param = new Param
                    {
                        Key = CACHE_EXPIRES_PAGES
                    };
                }
                param.Value = value.ToString();
                _api.Params.Save(param);
            }
        }

        /// <summary>
        /// Gets/sets the currently configured cache expiration
        /// in minutes for posts.
        /// </summary>
        public int CacheExpiresPosts {
            get {
                var param = _api.Params.GetByKey(CACHE_EXPIRES_POSTS);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _api.Params.GetByKey(CACHE_EXPIRES_POSTS);
                if (param == null)
                {
                    param = new Param
                    {
                        Key = CACHE_EXPIRES_POSTS
                    };
                }
                param.Value = value.ToString();
                _api.Params.Save(param);
            }
        }

        /// <summary>
        /// Gets/sets if hierarchical slugs should be generated when
        /// creating new pages.
        /// </summary>
        public bool HierarchicalPageSlugs {
            get {
                var param = _api.Params.GetByKey(PAGES_HIERARCHICAL_SLUGS);
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return true;
            }
            set {
                var param = _api.Params.GetByKey(PAGES_HIERARCHICAL_SLUGS);
                if (param == null)
                {
                    param = new Param
                    {
                        Key = PAGES_HIERARCHICAL_SLUGS
                    };
                }
                param.Value = value.ToString();
                _api.Params.Save(param);
            }
        }

        /// <summary>
        /// Gets/sets the default number of expanded sitemap levels
        /// in the manager interface.
        /// </summary>
        public int ManagerExpandedSitemapLevels {
            get {
                var param = _api.Params.GetByKey(MANAGER_EXPANDED_SITEMAP_LEVELS);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _api.Params.GetByKey(MANAGER_EXPANDED_SITEMAP_LEVELS);
                if (param == null)
                {
                    param = new Param
                    {
                        Key = MANAGER_EXPANDED_SITEMAP_LEVELS
                    };
                }
                param.Value = value.ToString();
                _api.Params.Save(param);
            }
        }

        /// <summary>
        /// Gets/sets the optional URL for the CDN used. If this param isn't
        /// null it will be used when generating the PublicUrl for media.
        /// </summary>
        public string MediaCDN {
            get {
                var param = _api.Params.GetByKey(MEDIA_CDN_URL);
                if (param != null)
                    return param.Value;
                return null;
            }
            set {
                var param = _api.Params.GetByKey(MEDIA_CDN_URL);
                if (param == null)
                {
                    param = new Param
                    {
                        Key = MEDIA_CDN_URL
                    };
                }

                // Ensure trailing slash
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith("/"))
                    value = value + "/";

                param.Value = value;
                _api.Params.Save(param);
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public Config(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Disposes the config.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}