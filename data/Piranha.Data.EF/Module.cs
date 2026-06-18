/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Mapster;
using MapsterMapper;
using Piranha.Extend;

namespace Piranha.Data.EF;

/// <summary>
/// The identity module.
/// </summary>
public class Module : IModule
{
    public static IMapper Mapper { get; private set; }

    /// <summary>
    /// Gets the Author
    /// </summary>
    public string Author => "Piranha";

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => "Piranha.Data.EF";

    /// <summary>
    /// Gets the Version
    /// </summary>
    public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

    /// <summary>
    /// Gets the description
    /// </summary>
    public string Description => "Data implementation for Entity Framework Core.";

    /// <summary>
    /// Gets the package url.
    /// </summary>
    public string PackageUrl => "https://www.nuget.org/packages/Piranha.Data.EF";

    /// <summary>
    /// Gets the icon url.
    /// </summary>
    public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

    /// <summary>
    /// Initializes the Mapster mapping configuration.
    /// </summary>
    static Module()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<Data.Alias, Data.Alias>()
            .Ignore(a => a.Id)
            .Ignore(a => a.Created);

        config.NewConfig<Data.Category, Data.Category>()
            .Ignore(c => c.Id)
            .Ignore(c => c.Created);

        config.NewConfig<Data.Category, Models.Taxonomy>()
            .Map(c => c.Type, m => Models.TaxonomyType.Category);

        config.NewConfig<Data.Content, Models.GenericContent>()
            .Map(p => p.PrimaryImage, m => m.PrimaryImageId)
            .Ignore(p => p.Permissions);

        config.NewConfig<Models.GenericContent, Data.Content>()
            .Ignore(c => c.CategoryId)
            .Ignore(c => c.Category)
            .Ignore(c => c.Blocks)
            .Ignore(c => c.Fields)
            .Ignore(c => c.Tags)
            .Ignore(c => c.Type)
            .Ignore(c => c.Translations)
            .Ignore(c => c.Created)
            .Ignore(c => c.LastModified);

        config.NewConfig<Data.ContentGroup, Models.ContentGroup>();

        config.NewConfig<Models.ContentGroup, Data.ContentGroup>()
            .Ignore(g => g.Created)
            .Ignore(g => g.LastModified);

        config.NewConfig<Data.ContentTranslation, Models.GenericContent>()
            .Ignore(c => c.Id)
            .Ignore(c => c.TypeId)
            .Ignore(c => c.PrimaryImage)
            .Ignore(c => c.Created)
            .Ignore(c => c.LastModified)
            .Ignore(c => c.Permissions);

        config.NewConfig<Data.MediaFolder, Data.MediaFolder>()
            .Ignore(f => f.Id)
            .Ignore(f => f.Created)
            .Ignore(f => f.Media);

        config.NewConfig<Data.MediaFolder, Models.MediaStructureItem>()
            .Ignore(f => f.Level)
            .Ignore(f => f.FolderCount)
            .Ignore(f => f.MediaCount)
            .Ignore(f => f.Items);

        config.NewConfig<Data.Page, Models.PageBase>()
            .Map(p => p.TypeId, m => m.PageTypeId)
            .Map(p => p.PrimaryImage, m => m.PrimaryImageId)
            .Map(p => p.OgImage, m => m.OgImageId)
            .Map(p => p.Permalink, m => "/" + m.Slug)
            .Ignore(p => p.Permissions)
            .Ignore(p => p.Blocks)
            .Ignore(p => p.CommentCount);

        config.NewConfig<Models.PageBase, Data.Page>()
            .Ignore(p => p.ContentType)
            .Map(p => p.PrimaryImageId, m => m.PrimaryImage != null ? m.PrimaryImage.Id : (Guid?)null)
            .Map(p => p.OgImageId, m => m.OgImage != null ? m.OgImage.Id : (Guid?)null)
            .Map(p => p.PageTypeId, m => m.TypeId)
            .Ignore(p => p.Blocks)
            .Ignore(p => p.Fields)
            .Ignore(p => p.Created)
            .Ignore(p => p.LastModified)
            .Ignore(p => p.Permissions)
            .Ignore(p => p.PageType)
            .Ignore(p => p.Site)
            .Ignore(p => p.Parent);

        config.NewConfig<Data.Page, Models.SitemapItem>()
            .Ignore(p => p.MenuTitle)
            .Ignore(p => p.Level)
            .Ignore(p => p.Items)
            .Ignore(p => p.PageTypeName)
            .Map(p => p.Permalink, d => !d.ParentId.HasValue && d.SortOrder == 0 ? "/" : "/" + d.Slug)
            .Map(p => p.Permissions, d => d.Permissions.Select(dp => dp.Permission).ToList());

        config.NewConfig<Data.Param, Data.Param>()
            .Ignore(p => p.Id)
            .Ignore(p => p.Created);

        config.NewConfig<Data.Post, Models.PostBase>()
            .Map(p => p.TypeId, m => m.PostTypeId)
            .Map(p => p.PrimaryImage, m => m.PrimaryImageId)
            .Map(p => p.OgImage, m => m.OgImageId)
            .Ignore(p => p.Permalink)
            .Ignore(p => p.Permissions)
            .Ignore(p => p.Blocks)
            .Ignore(p => p.CommentCount);

        config.NewConfig<Data.PostTag, Models.Taxonomy>()
            .Map(p => p.Id, m => m.TagId)
            .Map(p => p.Title, m => m.Tag.Title)
            .Map(p => p.Slug, m => m.Tag.Slug)
            .Map(p => p.Type, m => Models.TaxonomyType.Tag);

        config.NewConfig<Models.PostBase, Data.Post>()
            .Map(p => p.PostTypeId, m => m.TypeId)
            .Map(p => p.CategoryId, m => m.Category.Id)
            .Map(p => p.PrimaryImageId, m => m.PrimaryImage != null ? m.PrimaryImage.Id : (Guid?)null)
            .Map(p => p.OgImageId, m => m.OgImage != null ? m.OgImage.Id : (Guid?)null)
            .Ignore(p => p.Blocks)
            .Ignore(p => p.Fields)
            .Ignore(p => p.Created)
            .Ignore(p => p.LastModified)
            .Ignore(p => p.Permissions)
            .Ignore(p => p.PostType)
            .Ignore(p => p.Blog)
            .Ignore(p => p.Category)
            .Ignore(p => p.Tags);

        config.NewConfig<Data.Site, Data.Site>()
            .Ignore(s => s.Id)
            .Ignore(s => s.Language)
            .Ignore(s => s.Created);

        config.NewConfig<Data.Site, Models.SiteContentBase>()
            .Map(s => s.TypeId, m => m.SiteTypeId)
            .Ignore(s => s.Permissions);

        config.NewConfig<Models.SiteContentBase, Data.Site>()
            .Ignore(s => s.LanguageId)
            .Ignore(s => s.SiteTypeId)
            .Ignore(s => s.InternalId)
            .Ignore(s => s.Description)
            .Ignore(s => s.LogoId)
            .Ignore(s => s.Hostnames)
            .Ignore(s => s.IsDefault)
            .Ignore(s => s.Culture)
            .Ignore(s => s.Fields)
            .Ignore(s => s.Language)
            .Ignore(s => s.Created)
            .Ignore(s => s.LastModified)
            .Ignore(s => s.ContentLastModified);

        config.NewConfig<Data.Tag, Data.Tag>()
            .Ignore(t => t.Id)
            .Ignore(t => t.Created);

        config.NewConfig<Data.Tag, Models.Taxonomy>()
            .Map(t => t.Type, m => Models.TaxonomyType.Tag);

        Mapper = new global::MapsterMapper.Mapper(config);
    }

    /// <summary>
    /// Initializes the module.
    /// </summary>
    public void Init()
    {
    }
}