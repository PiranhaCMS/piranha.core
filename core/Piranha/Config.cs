/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;
using Piranha.Services;

namespace Piranha;

/// <summary>
/// Class for easy access to built-in config parameters.
/// </summary>
public sealed class Config : IDisposable
{
    private readonly IParamService _service;

    private static readonly string ARCHIVE_PAGE_SIZE = "ArchivePageSize";
    private static readonly string CACHE_EXPIRES_PAGES = "CacheExpiresPages";
    private static readonly string CACHE_EXPIRES_POSTS = "CacheExpiresPosts";
    private static readonly string COMMENTS_PAGE_SIZE = "CommentsPageSize";
    private static readonly string COMMENTS_APPROVE = "CommentsApprove";
    private static readonly string COMMENTS_POSTS_ENABLED = "CommentsPostsEnabled";
    private static readonly string COMMENTS_PAGES_ENABLED = "CommentsPagesEnabled";
    private static readonly string COMMENTS_CLOSE_AFTER_DAYS = "CommentsCloseAfterDays";
    private static readonly string HTML_EXCERPT = "HtmlExcerpt";
    private static readonly string MEDIA_CDN_URL = "MediaCdnUrl";
    private static readonly string MANAGER_EXPANDED_SITEMAP_LEVELS = "ManagerExpandedSitemapLevels";
    private static readonly string MANAGER_PAGE_SIZE = "ManagerPageSize";
    private static readonly string MANAGER_DEFAULT_COLLAPSED_BLOCKS = "ManagerDefaultCollapsedBlocks";
    private static readonly string MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS = "ManagerDefaultCollapsedBlockGroupHeaders";
    private static readonly string MANAGER_OUTLINED = "ManagerOutlined";
    private static readonly string MANAGER_XHR_TIMEOUT = "ManagerXhrTimeout";
    private static readonly string PAGES_HIERARCHICAL_SLUGS = "HierarchicalPageSlugs";
    private static readonly string PAGE_REVISIONS = "PageRevisions";
    private static readonly string POST_REVISIONS = "PostRevisions";

    /// <summary>
    /// Gets/sets the currently configured archive page size.
    /// </summary>
    public int ArchivePageSize
    {
        get => GetParam(ARCHIVE_PAGE_SIZE, 0);
        set =>  SetParam(ARCHIVE_PAGE_SIZE, value);
    }

    /// <summary>
    /// Gets/sets the currently configured cache expiration
    /// in minutes for pages.
    /// </summary>
    public int CacheExpiresPages
    {
        get => GetParam(CACHE_EXPIRES_PAGES, 0);
        set => SetParam(CACHE_EXPIRES_PAGES, value);
    }

    /// <summary>
    /// Gets/sets the currently configured cache expiration
    /// in minutes for posts.
    /// </summary>
    public int CacheExpiresPosts
    {
        get =>  GetParam(CACHE_EXPIRES_POSTS, 0);
        set => SetParam(CACHE_EXPIRES_POSTS, value);
    }

    /// <summary>
    /// Gets/sets if comments should be approved by default.
    /// </summary>
    public bool CommentsApprove
    {
        get => GetParam(COMMENTS_APPROVE, true);
        set => SetParam(COMMENTS_APPROVE, value);
    }

    /// <summary>
    /// Gets/sets the number of days commenting should be open after publish
    /// date. A value of 0 means forever.
    /// </summary>
    public int CommentsCloseAfterDays
    {
        get => GetParam(COMMENTS_CLOSE_AFTER_DAYS, 0);
        set => SetParam(COMMENTS_CLOSE_AFTER_DAYS, value);
    }

    /// <summary>
    /// Gets/sets if comments should be enabled by default for posts. The
    /// default value is true.
    /// </summary>
    public bool CommentsEnabledForPosts
    {
        get => GetParam(COMMENTS_POSTS_ENABLED, true);
        set => SetParam(COMMENTS_POSTS_ENABLED, value);
    }

    /// <summary>
    /// Gets/sets if comments should be enabled by default for pages. The
    /// default value is true.
    /// </summary>
    public bool CommentsEnabledForPages
    {
        get => GetParam(COMMENTS_PAGES_ENABLED, false);
        set => SetParam(COMMENTS_PAGES_ENABLED, value);
    }

    /// <summary>
    /// Gets/sets the currently configured page size for comments.
    /// </summary>
    public int CommentsPageSize
    {
        get => GetParam(COMMENTS_PAGE_SIZE, 0);
        set => SetParam(COMMENTS_PAGE_SIZE, value);
    }

    /// <summary>
    /// Gets/sets if page and post excerpt should be in HTML
    /// format by default.
    /// </summary>
    public bool HtmlExcerpt
    {
        get => GetParam(HTML_EXCERPT, false);
        set => SetParam(HTML_EXCERPT, value);
    }

    /// <summary>
    /// Gets/sets if hierarchical slugs should be generated when
    /// creating new pages.
    /// </summary>
    public bool HierarchicalPageSlugs
    {
        get => GetParam(PAGES_HIERARCHICAL_SLUGS, true);
        set => SetParam(PAGES_HIERARCHICAL_SLUGS, value);
    }

    /// <summary>
    /// Gets/sets the default number of expanded sitemap levels
    /// in the manager interface.
    /// </summary>
    public int ManagerExpandedSitemapLevels
    {
        get => GetParam(MANAGER_EXPANDED_SITEMAP_LEVELS, 0);
        set => SetParam(MANAGER_EXPANDED_SITEMAP_LEVELS, value);
    }

    /// <summary>
    /// Gets/sets the page size that should be used for paged lists in the manager.
    /// </summary>
    public int ManagerPageSize
    {
        get => GetParam(MANAGER_PAGE_SIZE, 15);
        set => SetParam(MANAGER_PAGE_SIZE, value);
    }

    /// <summary>
    /// Gets/sets if blocks should be collapsed by default in the
    /// manager interface. Default value is false.
    /// </summary>
    public bool ManagerDefaultCollapsedBlocks
    {
        get => GetParam(MANAGER_DEFAULT_COLLAPSED_BLOCKS, false);
        set => SetParam(MANAGER_DEFAULT_COLLAPSED_BLOCKS, value);
    }

    /// <summary>
    /// Gets/sets if block group headers should be collapsed by default in the
    /// manager interface. Default value is false.
    /// </summary>
    public bool ManagerDefaultCollapsedBlockGroupHeaders
    {
        get => GetParam(MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS, false);
        set => SetParam(MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS, value);
    }

    /// <summary>
    /// Gets/sets if elements in the manager should be outline with borders
    /// to provide stronger visual guidance. The default value is false.
    /// </summary>
    /// <value></value>
    public bool ManagerOutlined
    {
        get => GetParam(MANAGER_OUTLINED, false);
        set => SetParam(MANAGER_OUTLINED, value);
    }

    /// <summary>
    /// Gets/sets the timeout for XHR requests in the manager interface in seconds. This is for
    /// example used when uploading binary files to the manager. The default value is 30.
    /// </summary>
    public int ManagerXhrTimeout
    {
        get => GetParam(MANAGER_XHR_TIMEOUT, 30);
        set => SetParam(MANAGER_XHR_TIMEOUT, value);
    }

    /// <summary>
    /// Gets/sets the optional URL for the CDN used. If this param isn't
    /// null it will be used when generating the PublicUrl for media.
    /// </summary>
    public string MediaCDN
    {
        get => GetParam<string>(MEDIA_CDN_URL, null);
        set => SetParam(MEDIA_CDN_URL, value + (!string.IsNullOrWhiteSpace(value) && !value.EndsWith("/") ? "/" : ""));
    }

    /// <summary>
    /// Gets/sets the currently configured page revisions that should be saved.
    /// </summary>
    public int PageRevisions
    {
        get => GetParam(PAGE_REVISIONS, 10);
        set => SetParam(PAGE_REVISIONS, value);
    }

    /// <summary>
    /// Gets/sets the currently configured post revisions that should be saved.
    /// </summary>
    public int PostRevisions
    {
        get => GetParam(POST_REVISIONS, 10);
        set => SetParam(POST_REVISIONS, value);
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

    private T GetParam<T>(string key, T defaultValue)
    {
        var param = _service.GetByKeyAsync(key).GetAwaiter().GetResult();
        if (param != null)
            return (T)Convert.ChangeType(param.Value, typeof(T));
        return defaultValue;
    }

    private void SetParam<T>(string key, T value)
    {
        var param = _service.GetByKeyAsync(key).GetAwaiter().GetResult();
        if (param == null)
        {
            param = new Param
            {
                Key = key
            };
        }
        param.Value = value.ToString();
        _service.SaveAsync(param).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Disposes the config.
    /// </summary>
    public void Dispose()
    {
    }
}
