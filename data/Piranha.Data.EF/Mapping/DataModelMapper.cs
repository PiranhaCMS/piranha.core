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

namespace Piranha.Data.EF.Mapping;

internal static class DataModelMapper
{
    public static void MapToModel<TField>(Data.ContentBase<TField> source, Models.ContentBase target)
        where TField : Data.ContentFieldBase
    {
        switch (source)
        {
            case Data.Content content when target is Models.GenericContent model:
                Map(content, model);
                break;
            case Data.Page page when target is Models.PageBase model:
                Map(page, model);
                break;
            case Data.Post post when target is Models.PostBase model:
                Map(post, model);
                break;
            case Data.Site site when target is Models.SiteContentBase model:
                Map(site, model);
                break;
            default:
                MapContentBase(source, target);
                break;
        }
    }

    public static void MapToData<TField>(Models.ContentBase source, Data.ContentBase<TField> target)
        where TField : Data.ContentFieldBase
    {
        switch (source)
        {
            case Models.GenericContent model when target is Data.Content content:
                Map(model, content);
                break;
            case Models.PageBase model when target is Data.Page page:
                Map(model, page);
                break;
            case Models.PostBase model when target is Data.Post post:
                Map(model, post);
                break;
            case Models.SiteContentBase model when target is Data.Site site:
                Map(model, site);
                break;
            default:
                MapContentBase(source, target);
                break;
        }
    }

    public static void Map(Data.ContentTranslation source, Models.GenericContent target)
    {
        target.Title = source.Title;
        target.Excerpt = source.Excerpt;
    }

    public static Models.SitemapItem Map(Data.Page source)
    {
        return new Models.SitemapItem
        {
            Id = source.Id,
            OriginalPageId = source.OriginalPageId,
            ParentId = source.ParentId,
            SortOrder = source.SortOrder,
            Title = source.Title,
            NavigationTitle = source.NavigationTitle,
            MetaIndex = source.MetaIndex ?? true,
            MetaPriority = source.MetaPriority,
            Permalink = !source.ParentId.HasValue && source.SortOrder == 0 ? "/" : "/" + source.Slug,
            IsHidden = source.IsHidden,
            Published = source.Published,
            Created = source.Created,
            LastModified = source.LastModified,
            Permissions = source.Permissions.Select(p => p.Permission).ToList()
        };
    }

    public static Models.MediaStructureItem Map(Data.MediaFolder source)
    {
        return new Models.MediaStructureItem
        {
            Id = source.Id,
            Name = source.Name,
            Created = source.Created
        };
    }

    public static Models.ContentGroup Map(Data.ContentGroup source)
    {
        return new Models.ContentGroup
        {
            Id = source.Id,
            CLRType = source.CLRType,
            Title = source.Title,
            Icon = source.Icon,
            IsHidden = source.IsHidden
        };
    }

    public static void Map(Models.ContentGroup source, Data.ContentGroup target)
    {
        target.Id = source.Id;
        target.CLRType = source.CLRType;
        target.Title = source.Title;
        target.Icon = source.Icon;
        target.IsHidden = source.IsHidden;
    }

    private static void Map(Data.Content source, Models.GenericContent target)
    {
        MapContentBase(source, target);

        target.TypeId = source.TypeId;
        target.PrimaryImage = ToImageField(source.PrimaryImageId);
        target.Excerpt = source.Excerpt;
    }

    private static void Map(Models.GenericContent source, Data.Content target)
    {
        MapContentBase(source, target);

        target.TypeId = source.TypeId;
        target.PrimaryImageId = source.PrimaryImage?.Id;
        target.Excerpt = source.Excerpt;
    }

    private static void Map(Data.Page source, Models.PageBase target)
    {
        MapRoutedContentBase(source, target);

        target.TypeId = source.PageTypeId;
        target.SiteId = source.SiteId;
        target.ParentId = source.ParentId;
        target.SortOrder = source.SortOrder;
        target.NavigationTitle = source.NavigationTitle;
        target.IsHidden = source.IsHidden;
        target.OriginalPageId = source.OriginalPageId;
        target.Permalink = "/" + source.Slug;
    }

    private static void Map(Models.PageBase source, Data.Page target)
    {
        MapRoutedContentBase(source, target);

        target.PageTypeId = source.TypeId;
        target.SiteId = source.SiteId;
        target.ParentId = source.ParentId;
        target.SortOrder = source.SortOrder;
        target.NavigationTitle = source.NavigationTitle;
        target.IsHidden = source.IsHidden;
        target.OriginalPageId = source.OriginalPageId;
    }

    private static void Map(Data.Post source, Models.PostBase target)
    {
        MapRoutedContentBase(source, target);

        target.TypeId = source.PostTypeId;
        target.BlogId = source.BlogId;
        target.Category = source.Category != null ? ToTaxonomy(source.Category) : null;
        target.Tags = source.Tags.Select(ToTaxonomy).ToList();
    }

    private static void Map(Models.PostBase source, Data.Post target)
    {
        MapRoutedContentBase(source, target);

        target.PostTypeId = source.TypeId;
        target.BlogId = source.BlogId;
        if (source.Category != null)
        {
            target.CategoryId = source.Category.Id;
        }
    }

    private static void Map(Data.Site source, Models.SiteContentBase target)
    {
        MapContentBase(source, target);

        target.TypeId = source.SiteTypeId;
    }

    private static void Map(Models.SiteContentBase source, Data.Site target)
    {
        target.Id = source.Id;
        target.Title = source.Title;
    }

    private static void MapContentBase<TField>(Data.ContentBase<TField> source, Models.ContentBase target)
        where TField : Data.ContentFieldBase
    {
        target.Id = source.Id;
        target.Title = source.Title;
        target.Created = source.Created;
        target.LastModified = source.LastModified;
    }

    private static void MapContentBase<TField>(Models.ContentBase source, Data.ContentBase<TField> target)
        where TField : Data.ContentFieldBase
    {
        target.Id = source.Id;
        target.Title = source.Title;
    }

    private static void MapRoutedContentBase<TField>(Data.RoutedContentBase<TField> source, Models.RoutedContentBase target)
        where TField : Data.ContentFieldBase
    {
        MapContentBase(source, target);

        target.Slug = source.Slug;
        target.MetaTitle = source.MetaTitle;
        target.MetaKeywords = source.MetaKeywords;
        target.MetaDescription = source.MetaDescription;
        target.MetaIndex = source.MetaIndex ?? true;
        target.MetaFollow = source.MetaFollow ?? true;
        target.MetaPriority = source.MetaPriority;
        target.OgTitle = source.OgTitle;
        target.OgDescription = source.OgDescription;
        target.OgImage = ToImageField(source.OgImageId);
        target.Route = source.Route;
        target.Published = source.Published;
        target.PrimaryImage = ToImageField(GetPrimaryImageId(source));
        target.Excerpt = GetExcerpt(source);
        target.RedirectUrl = GetRedirectUrl(source);
        target.RedirectType = GetRedirectType(source);
        target.EnableComments = GetEnableComments(source);
        target.CloseCommentsAfterDays = GetCloseCommentsAfterDays(source);
    }

    private static void MapRoutedContentBase<TField>(Models.RoutedContentBase source, Data.RoutedContentBase<TField> target)
        where TField : Data.ContentFieldBase
    {
        MapContentBase(source, target);

        target.Slug = source.Slug;
        target.MetaTitle = source.MetaTitle;
        target.MetaKeywords = source.MetaKeywords;
        target.MetaDescription = source.MetaDescription;
        target.MetaIndex = source.MetaIndex;
        target.MetaFollow = source.MetaFollow;
        target.MetaPriority = source.MetaPriority;
        target.OgTitle = source.OgTitle;
        target.OgDescription = source.OgDescription;
        target.OgImageId = source.OgImage?.Id ?? Guid.Empty;
        target.Route = source.Route;
        target.Published = source.Published;

        switch (target)
        {
            case Data.Page page:
                page.PrimaryImageId = source.PrimaryImage?.Id;
                page.Excerpt = source.Excerpt;
                page.RedirectUrl = source.RedirectUrl;
                page.RedirectType = source.RedirectType;
                page.EnableComments = source.EnableComments;
                page.CloseCommentsAfterDays = source.CloseCommentsAfterDays;
                break;
            case Data.Post post:
                post.PrimaryImageId = source.PrimaryImage?.Id;
                post.Excerpt = source.Excerpt;
                post.RedirectUrl = source.RedirectUrl;
                post.RedirectType = source.RedirectType;
                post.EnableComments = source.EnableComments;
                post.CloseCommentsAfterDays = source.CloseCommentsAfterDays;
                break;
            default:
                throw new NotSupportedException($"Unsupported routed content data type {target.GetType().FullName}.");
        }
    }

    private static ImageField ToImageField(Guid? id)
    {
        return new ImageField { Id = id };
    }

    private static ImageField ToImageField(Guid id)
    {
        return new ImageField { Id = id };
    }

    private static Models.Taxonomy ToTaxonomy(Data.Category source)
    {
        return new Models.Taxonomy
        {
            Id = source.Id,
            Title = source.Title,
            Slug = source.Slug,
            Type = Models.TaxonomyType.Category
        };
    }

    private static Models.Taxonomy ToTaxonomy(Data.Tag source)
    {
        return new Models.Taxonomy
        {
            Id = source.Id,
            Title = source.Title,
            Slug = source.Slug,
            Type = Models.TaxonomyType.Tag
        };
    }

    private static Models.Taxonomy ToTaxonomy(Data.PostTag source)
    {
        return new Models.Taxonomy
        {
            Id = source.TagId,
            Title = source.Tag?.Title,
            Slug = source.Tag?.Slug,
            Type = Models.TaxonomyType.Tag
        };
    }

    private static Guid? GetPrimaryImageId<TField>(Data.RoutedContentBase<TField> source)
        where TField : Data.ContentFieldBase
    {
        return source switch
        {
            Data.Page page => page.PrimaryImageId,
            Data.Post post => post.PrimaryImageId,
            _ => null
        };
    }

    private static string GetExcerpt<TField>(Data.RoutedContentBase<TField> source)
        where TField : Data.ContentFieldBase
    {
        return source switch
        {
            Data.Page page => page.Excerpt,
            Data.Post post => post.Excerpt,
            _ => null
        };
    }

    private static string GetRedirectUrl<TField>(Data.RoutedContentBase<TField> source)
        where TField : Data.ContentFieldBase
    {
        return source switch
        {
            Data.Page page => page.RedirectUrl,
            Data.Post post => post.RedirectUrl,
            _ => null
        };
    }

    private static Models.RedirectType GetRedirectType<TField>(Data.RoutedContentBase<TField> source)
        where TField : Data.ContentFieldBase
    {
        return source switch
        {
            Data.Page page => page.RedirectType,
            Data.Post post => post.RedirectType,
            _ => Models.RedirectType.Temporary
        };
    }

    private static bool GetEnableComments<TField>(Data.RoutedContentBase<TField> source)
        where TField : Data.ContentFieldBase
    {
        return source switch
        {
            Data.Page page => page.EnableComments,
            Data.Post post => post.EnableComments,
            _ => false
        };
    }

    private static int GetCloseCommentsAfterDays<TField>(Data.RoutedContentBase<TField> source)
        where TField : Data.ContentFieldBase
    {
        return source switch
        {
            Data.Page page => page.CloseCommentsAfterDays,
            Data.Post post => post.CloseCommentsAfterDays,
            _ => 0
        };
    }
}
