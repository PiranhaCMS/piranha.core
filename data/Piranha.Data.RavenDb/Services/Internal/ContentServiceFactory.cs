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
using Piranha.Services;

namespace Piranha.Data.RavenDb.Services.Internal;

/// <inheritdoc />
internal class ContentServiceFactory : IContentServiceFactory
{
    private readonly IContentFactory _factory;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="factory">The content factory</param>
    public ContentServiceFactory(IContentFactory factory)
    {
        _factory = factory;
    }

    /// <inheritdoc />
    public IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>(IMapper mapper)
        where TContent : ContentBase<TField>
        where TField : ContentFieldBase
        where TModelBase : Models.ContentBase
    {
        return new ContentService<TContent, TField, TModelBase>(_factory, mapper);
    }

    /// <inheritdoc />
    public IContentService<Content, ContentField, Models.GenericContent> CreateContentService()
    {
        return new ContentService<Content, ContentField, Models.GenericContent>(_factory, Module.Mapper);
    }

    /// <inheritdoc />
    public IContentService<Page, PageField, Models.PageBase> CreatePageService()
    {
        return new ContentService<Page, PageField, Models.PageBase>(_factory, Module.Mapper);
    }

    /// <inheritdoc />
    public IContentService<Post, PostField, Models.PostBase> CreatePostService()
    {
        return new ContentService<Post, PostField, Models.PostBase>(_factory, Module.Mapper);
    }

    /// <inheritdoc />
    public IContentService<Site, SiteField, Models.SiteContentBase> CreateSiteService()
    {
        return new ContentService<Site, SiteField, Models.SiteContentBase>(_factory, Module.Mapper);
    }
}
