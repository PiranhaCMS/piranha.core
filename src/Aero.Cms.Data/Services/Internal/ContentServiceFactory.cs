
// todo - replace automapper w/ Mapster or manual mappings
using AutoMapper;
using Aero.Cms.Data.Data;
using Aero.Cms.Data.Services.Internal;
using Aero.Cms.Services;

namespace Aero.Cms.Data.Services.Internal;

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
        where TContent : Data.ContentBase<TField>
        where TField : Data.ContentFieldBase
        where TModelBase : Models.ContentBase
    {
        return new ContentService<TContent, TField, TModelBase>(_factory, mapper);
    }

    /// <inheritdoc />
    public IContentService<Data.Content, Data.ContentField, Models.GenericContent> CreateContentService()
    {
        return new ContentService<Data.Content, Data.ContentField, Models.GenericContent>(_factory, Module.Mapper);
    }

    /// <inheritdoc />
    public IContentService<Data.Page, Data.PageField, Models.PageBase> CreatePageService()
    {
        return new ContentService<Data.Page, Data.PageField, Models.PageBase>(_factory, Module.Mapper);
    }

    /// <inheritdoc />
    public IContentService<Data.Post, Data.PostField, Models.PostBase> CreatePostService()
    {
        return new ContentService<Data.Post, Data.PostField, Models.PostBase>(_factory, Module.Mapper);
    }

    /// <inheritdoc />
    public IContentService<Data.Site, Data.SiteField, Models.SiteContentBase> CreateSiteService()
    {
        return new ContentService<Data.Site, Data.SiteField, Models.SiteContentBase>(_factory, Module.Mapper);
    }
}
