/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Services;

namespace Piranha;

/// <summary>
/// The main application api.
/// </summary>
public interface IApi : IDisposable
{
    /// <summary>
    /// Gets the alias service.
    /// </summary>
    IAliasService Aliases { get; }

    /// <summary>
    /// Gets the archive service.
    /// </summary>
    IArchiveService Archives { get; }

    /// <summary>
    /// Gets the content service.
    /// </summary>
    IContentService Content { get; }

    /// <summary>
    /// Gets the content group service.
    /// </summary>
    IContentGroupService ContentGroups { get; }

    /// <summary>
    /// Gets the content type service.
    /// </summary>
    IContentTypeService ContentTypes { get; }

    /// <summary>
    /// Gets the language service.
    /// </summary>
    ILanguageService Languages { get; }

    /// <summary>
    /// Gets the media service.
    /// </summary>
    IMediaService Media { get; }

    /// <summary>
    /// Gets the page repository.
    /// </summary>
    IPageService Pages { get; }

    /// <summary>
    /// Gets the page type service.
    /// </summary>
    IPageTypeService PageTypes { get; }

    /// <summary>
    /// Gets the param service.
    /// </summary>
    IParamService Params { get; }

    /// <summary>
    /// Gets the post service.
    /// </summary>
    IPostService Posts { get; }

    /// <summary>
    /// Gets the post type service.
    /// </summary>
    IPostTypeService PostTypes { get; }

    /// <summary>
    /// Gets the site service.
    /// </summary>
    ISiteService Sites { get; }

    /// <summary>
    /// Gets the site type service.
    /// </summary>
    ISiteTypeService SiteTypes { get; }
}
