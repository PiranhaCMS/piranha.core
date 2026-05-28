/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend.Fields;

namespace Piranha.Data.EF;

/// <summary>
/// Hand-written mapper that replaces AutoMapper in the EF data layer.
/// All 22 mapping pairs are explicit and compile-time type-safe.
/// </summary>
internal sealed class PiranhaMapper : IPiranhaMapper
{
    /// <inheritdoc />
    public void Map<TSource, TDest>(TSource source, TDest dest)
    {
        // Dispatch to the correct explicit overload based on the runtime type pair.
        // These are the only 4 combinations called from ContentService.
        switch (source, dest)
        {
            case (Data.Content src, Models.GenericContent dst):
                MapContentToGenericContent(src, dst);
                break;
            case (Models.GenericContent src, Data.Content dst):
                MapGenericContentToContent(src, dst);
                break;
            case (Data.Page src, Models.PageBase dst):
                MapPageToPageBase(src, dst);
                break;
            case (Models.PageBase src, Data.Page dst):
                MapPageBaseToPage(src, dst);
                break;
            case (Data.Post src, Models.PostBase dst):
                MapPostToPostBase(src, dst);
                break;
            case (Models.PostBase src, Data.Post dst):
                MapPostBaseToPost(src, dst);
                break;
            case (Data.Site src, Models.SiteContentBase dst):
                MapSiteToSiteContentBase(src, dst);
                break;
            case (Models.SiteContentBase src, Data.Site dst):
                MapSiteContentBaseToSite(src, dst);
                break;
            default:
                throw new InvalidOperationException(
                    $"No mapping defined from {typeof(TSource).Name} to {typeof(TDest).Name}.");
        }
    }

    /// <inheritdoc />
    public void MapTranslation(Data.ContentTranslation source, Models.GenericContent dest)
    {
        // Maps ContentTranslation → GenericContent (title/excerpt only;
        // Id/TypeId/PrimaryImage/Created/Permissions are intentionally left untouched).
        dest.Title        = source.Title;
        dest.Excerpt      = source.Excerpt;
        dest.LastModified = source.LastModified;
    }

    // -------------------------------------------------------------------------
    // Alias → Alias  (copy-update, ignores Id and Created)
    // -------------------------------------------------------------------------
    internal static void MapAliasToAlias(Data.Alias src, Data.Alias dst)
    {
        dst.SiteId      = src.SiteId;
        dst.AliasUrl    = src.AliasUrl;
        dst.RedirectUrl = src.RedirectUrl;
        dst.Type        = src.Type;
        dst.LastModified = src.LastModified;
    }

    // -------------------------------------------------------------------------
    // Category → Category  (copy-update, ignores Id and Created)
    // -------------------------------------------------------------------------
    internal static void MapCategoryToCategory(Data.Category src, Data.Category dst)
    {
        dst.BlogId       = src.BlogId;
        dst.Title        = src.Title;
        dst.Slug         = src.Slug;
        dst.LastModified = src.LastModified;
    }

    // -------------------------------------------------------------------------
    // Category → Taxonomy
    // -------------------------------------------------------------------------
    internal static Models.Taxonomy MapCategoryToTaxonomy(Data.Category src)
    {
        return new Models.Taxonomy
        {
            Id    = src.Id,
            Title = src.Title,
            Slug  = src.Slug,
            Type  = Models.TaxonomyType.Category
        };
    }

    // -------------------------------------------------------------------------
    // Tag → Tag  (copy-update, ignores Id and Created)
    // -------------------------------------------------------------------------
    internal static void MapTagToTag(Data.Tag src, Data.Tag dst)
    {
        dst.BlogId       = src.BlogId;
        dst.Title        = src.Title;
        dst.Slug         = src.Slug;
        dst.LastModified = src.LastModified;
    }

    // -------------------------------------------------------------------------
    // Tag → Taxonomy
    // -------------------------------------------------------------------------
    internal static Models.Taxonomy MapTagToTaxonomy(Data.Tag src)
    {
        return new Models.Taxonomy
        {
            Id    = src.Id,
            Title = src.Title,
            Slug  = src.Slug,
            Type  = Models.TaxonomyType.Tag
        };
    }

    // -------------------------------------------------------------------------
    // PostTag → Taxonomy
    // -------------------------------------------------------------------------
    internal static Models.Taxonomy MapPostTagToTaxonomy(Data.PostTag src)
    {
        return new Models.Taxonomy
        {
            Id    = src.TagId,
            Title = src.Tag.Title,
            Slug  = src.Tag.Slug,
            Type  = Models.TaxonomyType.Tag
        };
    }

    // -------------------------------------------------------------------------
    // Param → Param  (copy-update, ignores Id and Created)
    // -------------------------------------------------------------------------
    internal static void MapParamToParam(Data.Param src, Data.Param dst)
    {
        dst.Key          = src.Key;
        dst.Value        = src.Value;
        dst.Description  = src.Description;
        dst.LastModified = src.LastModified;
    }

    // -------------------------------------------------------------------------
    // ContentGroup → ContentGroup  (copy-update, ignores Created and LastModified)
    // -------------------------------------------------------------------------
    internal static Models.ContentGroup MapContentGroupToModel(Data.ContentGroup src)
    {
        return new Models.ContentGroup
        {
            Id      = src.Id,
            Title   = src.Title,
            CLRType = src.CLRType,
            Icon    = src.Icon,
            IsHidden = src.IsHidden
        };
    }

    internal static void MapModelToContentGroup(Models.ContentGroup src, Data.ContentGroup dst)
    {
        // Created and LastModified managed by repository
        dst.Id       = src.Id;
        dst.Title    = src.Title;
        dst.CLRType  = src.CLRType;
        dst.Icon     = src.Icon;
        dst.IsHidden = src.IsHidden;
    }

    // -------------------------------------------------------------------------
    // MediaFolder → MediaFolder  (copy-update, ignores Id, Created, Media)
    // -------------------------------------------------------------------------
    internal static void MapMediaFolderToMediaFolder(Data.MediaFolder src, Data.MediaFolder dst)
    {
        dst.ParentId    = src.ParentId;
        dst.Name        = src.Name;
        dst.Description = src.Description;
        // Created: ignored (managed by repository)
    }

    // -------------------------------------------------------------------------
    // MediaFolder → MediaStructureItem
    // -------------------------------------------------------------------------
    internal static Models.MediaStructureItem MapMediaFolderToStructureItem(Data.MediaFolder src)
    {
        return new Models.MediaStructureItem
        {
            Id      = src.Id,
            Name    = src.Name,
            Created = src.Created
            // Level, FolderCount, MediaCount, Items set by repository after this call
        };
    }

    // -------------------------------------------------------------------------
    // Content → GenericContent
    // -------------------------------------------------------------------------
    private static void MapContentToGenericContent(Data.Content src, Models.GenericContent dst)
    {
        dst.Id           = src.Id;
        dst.TypeId       = src.TypeId;
        dst.Title        = src.Title;
        dst.PrimaryImage = new ImageField { Id = src.PrimaryImageId };
        dst.Excerpt      = src.Excerpt;
        dst.Created      = src.Created;
        dst.LastModified = src.LastModified;
        // Permissions: ignored — managed separately by the caller
    }

    // -------------------------------------------------------------------------
    // GenericContent → Content
    // -------------------------------------------------------------------------
    private static void MapGenericContentToContent(Models.GenericContent src, Data.Content dst)
    {
        // CategoryId, Category, Blocks, Fields, Tags, Type, Translations,
        // Created and LastModified are all ignored — managed by the repository.
        dst.Id      = src.Id;
        dst.TypeId  = src.TypeId;
        dst.Title   = src.Title;
        dst.Excerpt = src.Excerpt;
        dst.PrimaryImageId = src.PrimaryImage?.Id;
    }

    // -------------------------------------------------------------------------
    // Page → PageBase
    // -------------------------------------------------------------------------
    private static void MapPageToPageBase(Data.Page src, Models.PageBase dst)
    {
        dst.Id               = src.Id;
        dst.TypeId           = src.PageTypeId;
        dst.SiteId           = src.SiteId;
        dst.ParentId         = src.ParentId;
        dst.SortOrder        = src.SortOrder;
        dst.Title            = src.Title;
        dst.NavigationTitle  = src.NavigationTitle;
        dst.IsHidden         = src.IsHidden;
        dst.Slug             = src.Slug;
        dst.Permalink        = "/" + src.Slug;
        dst.MetaTitle        = src.MetaTitle;
        dst.MetaKeywords     = src.MetaKeywords;
        dst.MetaDescription  = src.MetaDescription;
        dst.MetaIndex        = src.MetaIndex ?? true;
        dst.MetaFollow       = src.MetaFollow ?? true;
        dst.MetaPriority     = src.MetaPriority;
        dst.OgTitle          = src.OgTitle;
        dst.OgDescription    = src.OgDescription;
        dst.OgImage          = new ImageField { Id = src.OgImageId };
        dst.PrimaryImage     = new ImageField { Id = src.PrimaryImageId };
        dst.Excerpt          = src.Excerpt;
        dst.Route            = src.Route;
        dst.Published        = src.Published;
        dst.RedirectUrl      = src.RedirectUrl;
        dst.RedirectType     = src.RedirectType;
        dst.EnableComments   = src.EnableComments;
        dst.CloseCommentsAfterDays = src.CloseCommentsAfterDays;
        dst.OriginalPageId   = src.OriginalPageId;
        dst.Created          = src.Created;
        dst.LastModified     = src.LastModified;
        // Blocks, Permissions, CommentCount: ignored — managed separately
    }

    // -------------------------------------------------------------------------
    // PageBase → Page
    // -------------------------------------------------------------------------
    private static void MapPageBaseToPage(Models.PageBase src, Data.Page dst)
    {
        // ContentType, Blocks, Fields, Created, LastModified,
        // Permissions, PageType, Site, Parent: all ignored
        dst.PageTypeId           = src.TypeId;
        dst.SiteId               = src.SiteId;
        dst.ParentId             = src.ParentId;
        dst.SortOrder            = src.SortOrder;
        dst.Title                = src.Title;
        dst.NavigationTitle      = src.NavigationTitle;
        dst.IsHidden             = src.IsHidden;
        dst.Slug                 = src.Slug;
        dst.MetaTitle            = src.MetaTitle;
        dst.MetaKeywords         = src.MetaKeywords;
        dst.MetaDescription      = src.MetaDescription;
        dst.MetaIndex            = src.MetaIndex;
        dst.MetaFollow           = src.MetaFollow;
        dst.MetaPriority         = src.MetaPriority;
        dst.OgTitle              = src.OgTitle;
        dst.OgDescription        = src.OgDescription;
        dst.OgImageId            = src.OgImage?.Id ?? Guid.Empty;
        dst.PrimaryImageId       = src.PrimaryImage != null ? src.PrimaryImage.Id : (Guid?)null;
        dst.Excerpt              = src.Excerpt;
        dst.Route                = src.Route;
        dst.Published            = src.Published;
        dst.RedirectUrl          = src.RedirectUrl;
        dst.RedirectType         = src.RedirectType;
        dst.EnableComments       = src.EnableComments;
        dst.CloseCommentsAfterDays = src.CloseCommentsAfterDays;
        dst.OriginalPageId       = src.OriginalPageId;
    }

    // -------------------------------------------------------------------------
    // Page → SitemapItem
    // -------------------------------------------------------------------------
    internal static Models.SitemapItem MapPageToSitemapItem(Data.Page src)
    {
        return new Models.SitemapItem
        {
            Id              = src.Id,
            OriginalPageId  = src.OriginalPageId,
            ParentId        = src.ParentId,
            SortOrder       = src.SortOrder,
            Title           = src.Title,
            NavigationTitle = src.NavigationTitle,
            MetaIndex       = src.MetaIndex ?? true,
            MetaPriority    = src.MetaPriority,
            IsHidden        = src.IsHidden,
            Published       = src.Published,
            Created         = src.Created,
            LastModified    = src.LastModified,
            Permalink       = !src.ParentId.HasValue && src.SortOrder == 0 ? "/" : "/" + src.Slug,
            Permissions     = src.Permissions.Select(p => p.Permission).ToList()
            // MenuTitle, Level, Items, PageTypeName: set by repository after this call
        };
    }

    // -------------------------------------------------------------------------
    // Post → PostBase
    // -------------------------------------------------------------------------
    private static void MapPostToPostBase(Data.Post src, Models.PostBase dst)
    {
        dst.Id               = src.Id;
        dst.TypeId           = src.PostTypeId;
        dst.BlogId           = src.BlogId;
        dst.Title            = src.Title;
        dst.Slug             = src.Slug;
        dst.PrimaryImage     = new ImageField { Id = src.PrimaryImageId };
        dst.OgImage          = new ImageField { Id = src.OgImageId };
        dst.Excerpt          = src.Excerpt;
        dst.MetaTitle        = src.MetaTitle;
        dst.MetaKeywords     = src.MetaKeywords;
        dst.MetaDescription  = src.MetaDescription;
        dst.MetaIndex        = src.MetaIndex ?? true;
        dst.MetaFollow       = src.MetaFollow ?? true;
        dst.MetaPriority     = src.MetaPriority;
        dst.OgTitle          = src.OgTitle;
        dst.OgDescription    = src.OgDescription;
        dst.Route            = src.Route;
        dst.Published        = src.Published;
        dst.RedirectUrl      = src.RedirectUrl;
        dst.RedirectType     = src.RedirectType;
        dst.EnableComments   = src.EnableComments;
        dst.CloseCommentsAfterDays = src.CloseCommentsAfterDays;
        dst.Created          = src.Created;
        dst.LastModified     = src.LastModified;

        // Category
        if (src.Category != null)
        {
            dst.Category = new Models.Taxonomy
            {
                Id    = src.Category.Id,
                Title = src.Category.Title,
                Slug  = src.Category.Slug,
                Type  = Models.TaxonomyType.Category
            };
        }

        // Tags
        dst.Tags.Clear();
        foreach (var postTag in src.Tags)
        {
            if (postTag.Tag != null)
            {
                dst.Tags.Add(new Models.Taxonomy
                {
                    Id    = postTag.TagId,
                    Title = postTag.Tag.Title,
                    Slug  = postTag.Tag.Slug,
                    Type  = Models.TaxonomyType.Tag
                });
            }
        }

        // Permalink, Permissions, Blocks, CommentCount: ignored — managed separately
    }

    // -------------------------------------------------------------------------
    // PostBase → Post
    // -------------------------------------------------------------------------
    private static void MapPostBaseToPost(Models.PostBase src, Data.Post dst)
    {
        // PostTypeId mapped from TypeId; CategoryId mapped from Category.Id
        // Blocks, Fields, Created, LastModified, Permissions, PostType,
        // Blog, Category, Tags: all ignored
        dst.PostTypeId           = src.TypeId;
        dst.CategoryId           = src.Category.Id;
        dst.BlogId               = src.BlogId;
        dst.Title                = src.Title;
        dst.Slug                 = src.Slug;
        dst.PrimaryImageId       = src.PrimaryImage != null ? src.PrimaryImage.Id : (Guid?)null;
        dst.OgImageId            = src.OgImage?.Id ?? Guid.Empty;
        dst.Excerpt              = src.Excerpt;
        dst.MetaTitle            = src.MetaTitle;
        dst.MetaKeywords         = src.MetaKeywords;
        dst.MetaDescription      = src.MetaDescription;
        dst.MetaIndex            = src.MetaIndex;
        dst.MetaFollow           = src.MetaFollow;
        dst.MetaPriority         = src.MetaPriority;
        dst.OgTitle              = src.OgTitle;
        dst.OgDescription        = src.OgDescription;
        dst.Route                = src.Route;
        dst.Published            = src.Published;
        dst.RedirectUrl          = src.RedirectUrl;
        dst.RedirectType         = src.RedirectType;
        dst.EnableComments       = src.EnableComments;
        dst.CloseCommentsAfterDays = src.CloseCommentsAfterDays;
        // Blocks, Fields, Created, LastModified, Permissions, PostType,
        // Blog, Category, Tags: all ignored
    }

    // -------------------------------------------------------------------------
    // Site → Site  (copy-update, ignores Id, Language, Created)
    // -------------------------------------------------------------------------
    internal static void MapSiteToSite(Data.Site src, Data.Site dst)
    {
        dst.SiteTypeId       = src.SiteTypeId;
        dst.InternalId       = src.InternalId;
        dst.Title            = src.Title;
        dst.Description      = src.Description;
        dst.Hostnames        = src.Hostnames;
        dst.IsDefault        = src.IsDefault;
        dst.Culture          = src.Culture;
        dst.LastModified     = src.LastModified;
        dst.ContentLastModified = src.ContentLastModified;
    }

    // -------------------------------------------------------------------------
    // Site → SiteContentBase
    // -------------------------------------------------------------------------
    private static void MapSiteToSiteContentBase(Data.Site src, Models.SiteContentBase dst)
    {
        dst.Id           = src.Id;
        dst.TypeId       = src.SiteTypeId;
        dst.Title        = src.Title;
        dst.Created      = src.Created;
        dst.LastModified = src.LastModified;
        // Permissions: ignored
    }

    // -------------------------------------------------------------------------
    // SiteContentBase → Site
    // -------------------------------------------------------------------------
    private static void MapSiteContentBaseToSite(Models.SiteContentBase src, Data.Site dst)
    {
        // LanguageId, SiteTypeId, InternalId, Description, LogoId,
        // Hostnames, IsDefault, Culture, Fields, Language,
        // Created, LastModified, ContentLastModified: all ignored
        dst.Id    = src.Id;
        dst.Title = src.Title;
    }
}
