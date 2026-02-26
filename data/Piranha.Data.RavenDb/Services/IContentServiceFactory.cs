/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using AutoMapper;
using Piranha.Data.RavenDb.Data;

namespace Piranha.Data.RavenDb.Services;

/// <summary>
/// Factory for creating content transformation services.
/// </summary>
public interface IContentServiceFactory
{
    /// <summary>
    /// Creates a new content service for the specified types.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance to use for transformation</param>
    /// <returns>The content service</returns>
    IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>(IMapper mapper)
        where TContent : ContentBase<TField>
        where TField : ContentFieldBase
        where TModelBase : Models.ContentBase;

    /// <summary>
    /// Creates a new content service.
    /// </summary>
    /// <returns>The content service</returns>
    IContentService<Content, ContentField, Models.GenericContent> CreateContentService();

    /// <summary>
    /// Creates a new page content service.
    /// </summary>
    /// <returns>The content service</returns>
    IContentService<Page, PageField, Models.PageBase> CreatePageService();

    /// <summary>
    /// Creates a new post content service.
    /// </summary>
    /// <returns>The content service</returns>
    IContentService<Post, PostField, Models.PostBase> CreatePostService();

    /// <summary>
    /// Creates a new site content service.
    /// </summary>
    /// <returns>The content service</returns>
    IContentService<Site, SiteField, Models.SiteContentBase> CreateSiteService();
}
