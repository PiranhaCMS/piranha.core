/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Services;

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
    public IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>()
        where TContent : Data.ContentBase<TField>
        where TField : Data.ContentFieldBase
        where TModelBase : Models.ContentBase
    {
        return new ContentService<TContent, TField, TModelBase>(_factory);
    }

    /// <inheritdoc />
    public IContentService<Data.Content, Data.ContentField, Models.GenericContent> CreateContentService()
    {
        return new ContentService<Data.Content, Data.ContentField, Models.GenericContent>(_factory);
    }

    /// <inheritdoc />
    public IContentService<Data.Page, Data.PageField, Models.PageBase> CreatePageService()
    {
        return new ContentService<Data.Page, Data.PageField, Models.PageBase>(_factory);
    }

    /// <inheritdoc />
    public IContentService<Data.Post, Data.PostField, Models.PostBase> CreatePostService()
    {
        return new ContentService<Data.Post, Data.PostField, Models.PostBase>(_factory);
    }

    /// <inheritdoc />
    public IContentService<Data.Site, Data.SiteField, Models.SiteContentBase> CreateSiteService()
    {
        return new ContentService<Data.Site, Data.SiteField, Models.SiteContentBase>(_factory);
    }
}
