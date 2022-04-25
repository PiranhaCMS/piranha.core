/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text;
using Piranha.AspNetCore.Services;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;

/// <summary>
/// Extension class with helper methods for generating
/// URL's for the default components.
/// </summary>
public static class PiranhaUrlExtensions
{
    /// <summary>
    /// Generates an absolute url for the given page.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="page">The page</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, PageBase page)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, page) }";
    }

    /// <summary>
    /// Generates an absolute url for the given page block.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="block">The block</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, PageBlock block)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, block) }";
    }

    /// <summary>
    /// Generates an absolute url for the given page field.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="field">The field</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, PageField field)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, field) }";
    }

    /// <summary>
    /// Generates an absolute url for the given post.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="post">The post</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, PostBase post)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, post) }";
    }

    /// <summary>
    /// Generates an absolute url for the given post block.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="block">The block</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, PostBlock block)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, block) }";
    }

    /// <summary>
    /// Generates an absolute url for the given post field.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="field">The field</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, PostField field)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, field) }";
    }

    /// <summary>
    /// Generates an absolute url for the given taxonomy in the
    /// current archive.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="taxonomy">The taxonomy</param>
    /// <returns></returns>
    public static string AbsoluteUrl(this IApplicationService app, Taxonomy taxonomy)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, taxonomy) }";
    }

    /// <summary>
    /// Generates a local url for the given page.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="page">The page</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, PageBase page)
    {
        if (page != null)
        {
            return Url(app, page.Permalink);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given page block.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="block">The block</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, PageBlock block)
    {
        if (block != null)
        {
            return Url(app, block.Body);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given page field.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="field">The field</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, PageField field)
    {
        if (field != null)
        {
            return Url(app, field.Page);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given post.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="post">The post</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, PostBase post)
    {
        if (post != null)
        {
            return Url(app, post.Permalink);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given post block.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="block">The block</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, PostBlock block)
    {
        if (block != null)
        {
            return Url(app, block.Body);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given post field.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="field">The field</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, PostField field)
    {
        if (field != null)
        {
            return Url(app, field.Post);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given sitemap item.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="item">The sitemap item</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, SitemapItem item)
    {
        if (item != null)
        {
            return Url(app, item.Permalink);
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given taxonomy in the
    /// current archive.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="taxonomy">The taxonomy</param>
    /// <returns></returns>
    public static string Url(this IApplicationService app, Taxonomy taxonomy)
    {
        if (app.CurrentPage != null && taxonomy != null && taxonomy.Type != TaxonomyType.NotSet)
        {
            if (taxonomy.Type == TaxonomyType.Category)
            {
                return $"{ Url(app, app.CurrentPage) }/category/{ taxonomy.Slug }";
            }
            return $"{ Url(app, app.CurrentPage) }/tag/{ taxonomy.Slug }";
        }
        return "";
    }

    /// <summary>
    /// Generates a local url for the given slug.
    /// </summary>
    /// <param name="app">The current application service</param>
    /// <param name="slug">The slug</param>
    /// <returns>The url</returns>
    public static string Url(this IApplicationService app, string slug)
    {
        return GenerateUrl(app, slug, true);
    }

    /// <summary>
    /// Generates a local url for the given content.
    /// </summary>
    /// <param name="app">The current application service</param>
    /// <param name="url">The content url</param>
    /// <returns>The url</returns>
    public static string ContentUrl(this IApplicationService app, string url)
    {
        return GenerateUrl(app, url, false);
    }

    /// <summary>
    /// Generates an absolute url for the given slug.
    /// </summary>
    /// <param name="app">The current application service</param>
    /// <param name="slug">The slug</param>
    /// <returns>The url</returns>
    public static string AbsoluteUrl(this IApplicationService app, string slug)
    {
        return $"{ AbsoluteUrlStart(app) }{ Url(app, slug) }";
    }

    /// <summary>
    /// Generates an absolute url for the given content.
    /// </summary>
    /// <param name="app">The current application service</param>
    /// <param name="url">The content url</param>
    /// <returns>The url</returns>
    public static string AbsoluteContentUrl(this IApplicationService app, string url)
    {
        var contentUrl = ContentUrl(app, url);

        if (contentUrl.ToLower().StartsWith("http"))
        {
            return contentUrl;
        }
        return $"{ AbsoluteUrlStart(app) }{ contentUrl }";
    }

    /// <summary>
    /// Generates the scheme://host:port segment of the url from
    /// the current application request.
    /// </summary>
    /// <param name="app">The current application service</param>
    /// <returns>The url segment</returns>
    private static string AbsoluteUrlStart(IApplicationService app)
    {
        var sb = new StringBuilder();

        sb.Append(app.Request.Scheme);
        sb.Append("://");
        sb.Append(app.Request.Host);
        if (app.Request.Port.HasValue)
        {
            sb.Append(":");
            sb.Append(app.Request.Port.ToString());
        }
        return sb.ToString();
    }

    /// <summary>
    /// Generates a local url for the given slug.
    /// </summary>
    /// <param name="app">The current application service</param>
    /// <param name="slug">The slug</param>
    /// <param name="prefix">If site prefix should be appended</param>
    /// <returns>The url</returns>
    private static string GenerateUrl(this IApplicationService app, string slug, bool prefix)
    {
        // Make sure we got a slug
        if (slug == null)
            return null;

        // If this is an external url, return it
        if (slug.ToLower().StartsWith("http"))
        {
            return slug;
        }

        // Fix internal urls
        if (slug.StartsWith("~/"))
        {
            slug = slug.Replace("~/", "/");
        }

        // Fix slug that doesn't start with /
        if (!slug.StartsWith("/"))
        {
            slug = $"/{ slug }";
        }

        // Append site prefix, if available
        if (prefix && !string.IsNullOrEmpty(app.Site.SitePrefix))
        {
            slug = $"/{ app.Site.SitePrefix }{ slug }";
        }
        return slug;
    }
}