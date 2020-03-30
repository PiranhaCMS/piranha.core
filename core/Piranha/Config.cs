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
        /// The private param service.
        /// </summary>
        private readonly IParamService _service;

        /// <summary>
        /// The system config keys.
        /// </summary>
        public static readonly string ARCHIVE_PAGE_SIZE = "ArchivePageSize";
        public static readonly string CACHE_EXPIRES_PAGES = "CacheExpiresPages";
        public static readonly string CACHE_EXPIRES_POSTS = "CacheExpiresPosts";
        public static readonly string COMMENTS_PAGE_SIZE = "CommentsPageSize";
        public static readonly string COMMENTS_APPROVE = "CommentsApprove";
        public static readonly string COMMENTS_POSTS_ENABLED = "CommentsPostsEnabled";
        public static readonly string COMMENTS_PAGES_ENABLED = "CommentsPagesEnabled";
        public static readonly string COMMENTS_CLOSE_AFTER_DAYS = "CommentsCloseAfterDays";
        public static readonly string PAGES_HIERARCHICAL_SLUGS = "HierarchicalPageSlugs";
        public static readonly string MEDIA_CDN_URL = "MediaCdnUrl";
        public static readonly string MANAGER_EXPANDED_SITEMAP_LEVELS = "ManagerExpandedSitemapLevels";
        public static readonly string MANAGER_PAGE_SIZE = "ManagerPageSize";
        public static readonly string MANAGER_DEFAULT_COLLAPSED_BLOCKS = "ManagerDefaultCollapsedBlocks";
        public static readonly string MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS = "ManagerDefaultCollapsedBlockGroupHeaders";
        public static readonly string PAGE_REVISIONS = "PageRevisions";
        public static readonly string POST_REVISIONS = "PostRevisions";

        /// <summary>
        /// Gets/sets the currently configured archive page size.
        /// </summary>
        public int ArchivePageSize {
            get {
                var param = _service.GetByKeyAsync(ARCHIVE_PAGE_SIZE).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _service.GetByKeyAsync(ARCHIVE_PAGE_SIZE).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = ARCHIVE_PAGE_SIZE
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the currently configured cache expiration
        /// in minutes for pages.
        /// </summary>
        public int CacheExpiresPages {
            get {
                var param = _service.GetByKeyAsync(CACHE_EXPIRES_PAGES).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _service.GetByKeyAsync(CACHE_EXPIRES_PAGES).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = CACHE_EXPIRES_PAGES
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the currently configured cache expiration
        /// in minutes for posts.
        /// </summary>
        public int CacheExpiresPosts {
            get {
                var param = _service.GetByKeyAsync(CACHE_EXPIRES_POSTS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _service.GetByKeyAsync(CACHE_EXPIRES_POSTS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = CACHE_EXPIRES_POSTS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets if comments should be approved by default.
        /// </summary>
        public bool CommentsApprove {
            get {
                var param = _service.GetByKeyAsync(COMMENTS_APPROVE).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return true;
            }
            set {
                var param = _service.GetByKeyAsync(COMMENTS_APPROVE).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = COMMENTS_APPROVE
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the number of days commenting should be open after publish
        /// date. A value of 0 means forever.
        /// </summary>
        public int CommentsCloseAfterDays {
            get {
                var param = _service.GetByKeyAsync(COMMENTS_CLOSE_AFTER_DAYS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _service.GetByKeyAsync(COMMENTS_CLOSE_AFTER_DAYS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = COMMENTS_CLOSE_AFTER_DAYS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets if comments should be enabled by default for posts. The
        /// default value is true.
        /// </summary>
        public bool CommentsEnabledForPosts {
            get {
                var param = _service.GetByKeyAsync(COMMENTS_POSTS_ENABLED).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return true;
            }
            set {
                var param = _service.GetByKeyAsync(COMMENTS_POSTS_ENABLED).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = COMMENTS_POSTS_ENABLED
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets if comments should be enabled by default for pages. The
        /// default value is true.
        /// </summary>
        public bool CommentsEnabledForPages {
            get {
                var param = _service.GetByKeyAsync(COMMENTS_PAGES_ENABLED).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return false;
            }
            set {
                var param = _service.GetByKeyAsync(COMMENTS_PAGES_ENABLED).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = COMMENTS_PAGES_ENABLED
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the currently configured page size for comments.
        /// </summary>
        public int CommentsPageSize {
            get {
                var param = _service.GetByKeyAsync(COMMENTS_PAGE_SIZE).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _service.GetByKeyAsync(COMMENTS_PAGE_SIZE).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = COMMENTS_PAGE_SIZE
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets if hierarchical slugs should be generated when
        /// creating new pages.
        /// </summary>
        public bool HierarchicalPageSlugs {
            get {
                var param = _service.GetByKeyAsync(PAGES_HIERARCHICAL_SLUGS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return true;
            }
            set {
                var param = _service.GetByKeyAsync(PAGES_HIERARCHICAL_SLUGS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = PAGES_HIERARCHICAL_SLUGS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the default number of expanded sitemap levels
        /// in the manager interface.
        /// </summary>
        public int ManagerExpandedSitemapLevels {
            get {
                var param = _service.GetByKeyAsync(MANAGER_EXPANDED_SITEMAP_LEVELS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = _service.GetByKeyAsync(MANAGER_EXPANDED_SITEMAP_LEVELS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = MANAGER_EXPANDED_SITEMAP_LEVELS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the page size that should be used for paged lists in the manager.
        /// </summary>
        public int ManagerPageSize {
            get {
                var param = _service.GetByKeyAsync(MANAGER_PAGE_SIZE).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 15;
            }
            set {
                var param = _service.GetByKeyAsync(MANAGER_PAGE_SIZE).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = MANAGER_PAGE_SIZE
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets if blocks should be collapsed by default in the
        /// manager interface. Default value is false.
        /// </summary>
        public bool ManagerDefaultCollapsedBlocks {
            get {
                var param = _service.GetByKeyAsync(MANAGER_DEFAULT_COLLAPSED_BLOCKS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return false;
            }
            set {
                var param = _service.GetByKeyAsync(MANAGER_DEFAULT_COLLAPSED_BLOCKS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = MANAGER_DEFAULT_COLLAPSED_BLOCKS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets if block group headers should be collapsed by default in the
        /// manager interface. Default value is false.
        /// </summary>
        public bool ManagerDefaultCollapsedBlockGroupHeaders {
            get {
                var param = _service.GetByKeyAsync(MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToBoolean(param.Value);
                return false;
            }
            set {
                var param = _service.GetByKeyAsync(MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the optional URL for the CDN used. If this param isn't
        /// null it will be used when generating the PublicUrl for media.
        /// </summary>
        public string MediaCDN {
            get {
                var param = _service.GetByKeyAsync(MEDIA_CDN_URL).GetAwaiter().GetResult();
                if (param != null)
                    return param.Value;
                return null;
            }
            set {
                var param = _service.GetByKeyAsync(MEDIA_CDN_URL).GetAwaiter().GetResult();
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
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the currently configured page revisions that should be saved.
        /// </summary>
        public int PageRevisions {
            get {
                var param = _service.GetByKeyAsync(PAGE_REVISIONS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 10;
            }
            set {
                var param = _service.GetByKeyAsync(PAGE_REVISIONS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = PAGE_REVISIONS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets/sets the currently configured post revisions that should be saved.
        /// </summary>
        public int PostRevisions {
            get {
                var param = _service.GetByKeyAsync(POST_REVISIONS).GetAwaiter().GetResult();
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 10;
            }
            set {
                var param = _service.GetByKeyAsync(POST_REVISIONS).GetAwaiter().GetResult();
                if (param == null)
                {
                    param = new Param
                    {
                        Key = POST_REVISIONS
                    };
                }
                param.Value = value.ToString();
                _service.SaveAsync(param).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="paramService">The current param service</param>
        public Config(IParamService paramService)
        {
            _service = paramService;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public Config(IApi api)
        {
            _service = api.Params;
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