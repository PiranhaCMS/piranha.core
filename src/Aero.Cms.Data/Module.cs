
using Aero.Cms.Data.Data;
using Mapster;
using MapsterMapper;
using Aero.Cms.Extend;
using Microsoft.Extensions.Logging;

namespace Aero.Cms.Data;

/// <summary>
/// The identity module.
/// </summary>
public class Module : IModule
{
    public static IMapper Mapper { get; private set; }

    /// <summary>
    /// Gets the Author
    /// </summary>
    public string Author => "Microbians";

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => "Aero.Cms.Data";

    /// <summary>
    /// Gets the Version
    /// </summary>
    public string Version => Aero.Cms.Utils.GetAssemblyVersion(GetType().Assembly);

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description => "Data implementation for Entity Framework Core.";

    /// <summary>
    /// Gets the package url.
    /// </summary>
    public string PackageUrl => "https://www.nuget.org/packages/Aero.Cms.Data";

    /// <summary>
    /// Gets the icon url.
    /// </summary>
    public string IconUrl => "https://Aerocms.org/assets/twitter-shield.png";

    /// <summary>
    /// Create automapping.
    /// </summary>
    static Module()
    {
        var config = new TypeAdapterConfig();
        
        // Enable inheritance mapping similar to AutoMapper
        config.AllowImplicitDestinationInheritance = true;
        config.AllowImplicitSourceInheritance = true;

        config.NewConfig<Alias, Alias>()
            .Ignore(a => a.Id)
            .Ignore(a => a.Created);
        config.NewConfig<Category, Category>()
            .Ignore(c => c.Id)
            .Ignore(c => c.Created);
        config.NewConfig<Category, Models.Taxonomy>()
            .Map(dest => dest.Type, src => Models.TaxonomyType.Category);
        config.NewConfig<Content, Models.GenericContent>()
            .Map(dest => dest.PrimaryImage, src => src.PrimaryImageId)
            .Ignore("Blocks")
            .Ignore(dest => dest.Permissions);
        config.NewConfig<Models.GenericContent, Content>()
            .Ignore(dest => dest.CategoryId)
            .Ignore(dest => dest.Category)
            .Ignore(dest => dest.Blocks)
            .Ignore(dest => dest.Fields)
            .Ignore(dest => dest.Tags)
            .Ignore(dest => dest.Type)
            .Ignore(dest => dest.Translations)
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.LastModified);
        config.NewConfig<ContentGroup, Models.ContentGroup>();
        config.NewConfig<Models.ContentGroup, ContentGroup>()
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.LastModified);
        config.NewConfig<ContentTranslation, Models.GenericContent>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.TypeId)
            .Ignore(dest => dest.PrimaryImage)
            .Ignore("Blocks")
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.LastModified)
            .Ignore(dest => dest.Permissions);
        config.NewConfig<MediaFolder, MediaFolder>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.Media);
        config.NewConfig<MediaFolder, Models.MediaStructureItem>()
            .Ignore(dest => dest.Level)
            .Ignore(dest => dest.FolderCount)
            .Ignore(dest => dest.MediaCount)
            .Ignore(dest => dest.Items);
        config.NewConfig<Page, Models.PageBase>()
            .Map(dest => dest.TypeId, src => src.PageTypeId)
            .Map(dest => dest.PrimaryImage, src => src.PrimaryImageId)
            .Map(dest => dest.OgImage, src => src.OgImageId)
            .Map(dest => dest.Permalink, src => "/" + src.Slug)
            .Ignore(dest => dest.Permissions)
            .Ignore(dest => dest.Blocks)
            .Ignore(dest => dest.CommentCount);
        config.NewConfig<Models.PageBase, Page>()
            .Ignore(dest => dest.ContentType)
            .Map(dest => dest.PrimaryImageId,
                src => src.PrimaryImage != null ? src.PrimaryImage.Id : null)
            .Map(dest => dest.OgImageId, src => src.OgImage != null ? src.OgImage.Id : null)
            .Map(dest => dest.PageTypeId, src => src.TypeId)
            .Ignore(dest => dest.Blocks)
            .Ignore(dest => dest.Fields)
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.LastModified)
            .Ignore(dest => dest.Permissions)
            .Ignore(dest => dest.PageType)
            .Ignore(dest => dest.Site)
            .Ignore(dest => dest.Parent);
        config.NewConfig<Page, Models.SitemapItem>()
            .Ignore(dest => dest.MenuTitle)
            .Ignore(dest => dest.Level)
            .Ignore(dest => dest.Items)
            .Ignore(dest => dest.PageTypeName)
            .Map(dest => dest.Permalink,
                src => string.IsNullOrEmpty(src.ParentId) && src.SortOrder == 0 ? "/" : "/" + src.Slug)
            .Map(dest => dest.Permissions, src => src.Permissions.Select(dp => dp.Permission).ToList());
        config.NewConfig<Param, Param>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Created);
        config.NewConfig<Post, Models.PostBase>()
            .Map(dest => dest.TypeId, src => src.PostTypeId)
            .Map(dest => dest.PrimaryImage, src => src.PrimaryImageId)
            .Map(dest => dest.OgImage, src => src.OgImageId)
            .Ignore(dest => dest.Permalink)
            .Ignore(dest => dest.Permissions)
            .Ignore(dest => dest.Blocks)
            .Ignore(dest => dest.CommentCount);
        config.NewConfig<PostTag, Models.Taxonomy>()
            .Map(dest => dest.Id, src => src.TagId)
            .Map(dest => dest.Title, src => src.Tag.Title)
            .Map(dest => dest.Slug, src => src.Tag.Slug)
            .Map(dest => dest.Type, src => Models.TaxonomyType.Tag);
        config.NewConfig<Models.PostBase, Post>()
            .Map(dest => dest.PostTypeId, src => src.TypeId)
            .Map(dest => dest.CategoryId, src => src.Category.Id)
            .Map(dest => dest.PrimaryImageId,
                src => src.PrimaryImage != null ? src.PrimaryImage.Id : null)
            .Map(dest => dest.OgImageId, src => src.OgImage != null ? src.OgImage.Id : null)
            .Ignore(dest => dest.Blocks)
            .Ignore(dest => dest.Fields)
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.LastModified)
            .Ignore(dest => dest.Permissions)
            .Ignore(dest => dest.PostType)
            .Ignore(dest => dest.Blog)
            .Ignore(dest => dest.Category)
            .Ignore(dest => dest.Tags);
        config.NewConfig<Site, Site>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Language)
            .Ignore(dest => dest.Created);
        config.NewConfig<Site, Models.SiteContentBase>()
            .Map(dest => dest.TypeId, src => src.SiteTypeId)
            .Ignore(dest => dest.Permissions);
        config.NewConfig<Models.SiteContentBase, Site>()
            .Ignore(dest => dest.LanguageId)
            .Ignore(dest => dest.SiteTypeId)
            .Ignore(dest => dest.InternalId)
            .Ignore(dest => dest.Description)
            .Ignore(dest => dest.LogoId)
            .Ignore(dest => dest.Hostnames)
            .Ignore(dest => dest.IsDefault)
            .Ignore(dest => dest.Culture)
            .Ignore(dest => dest.Fields)
            .Ignore(dest => dest.Language)
            .Ignore(dest => dest.Created)
            .Ignore(dest => dest.LastModified)
            .Ignore(dest => dest.ContentLastModified);
        config.NewConfig<Tag, Tag>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Created);
        config.NewConfig<Tag, Models.Taxonomy>()
            .Map(dest => dest.Type, src => Models.TaxonomyType.Tag);

        Mapper = new Mapper(config);
    }

    /// <summary>
    /// Initializes the module.
    /// </summary>
    public void Init()
    {
    }
}
