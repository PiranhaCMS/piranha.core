/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text;
using Microsoft.AspNetCore.Html;
using Piranha.AspNetCore.Services;
using Piranha.Models;

/// <summary>
/// Extension class with html helper methods.
/// </summary>
public static class HtmlExtensions
{
    /// <summary>
    /// Generates meta tags for the given content.
    /// </summary>
    /// <param name="app">The application service</param>
    /// <param name="content">The content</param>
    /// <param name="meta">If meta tags should be generated</param>
    /// <param name="opengraph">If open graph tags should be generated</param>
    /// <param name="generator">If generator tag should be generated</param>
    /// <returns>The meta tags</returns>
    public static HtmlString MetaTags(this IApplicationService app, IMeta content, bool meta = true, bool opengraph = true, bool generator = true)
    {
        var sb = new StringBuilder();

        if (meta)
        {
            // Generate meta tags
            if (!string.IsNullOrWhiteSpace(content.MetaKeywords))
            {
                sb.AppendLine($"<meta name=\"keywords\" value=\"{ content.MetaKeywords }\">");
            }
            if (!string.IsNullOrWhiteSpace(content.MetaDescription))
            {
                sb.AppendLine($"<meta name=\"description\" value=\"{ content.MetaDescription }\">");
            }
        }

        if (generator)
        {
            // Generate generator tag
            sb.AppendLine($"<meta name=\"generator\" value=\"Piranha CMS { Piranha.Utils.GetAssemblyVersion(typeof(Piranha.App).Assembly) }\">");
        }

        if (opengraph)
        {
            // Generate open graph tags
            if (content is PageBase page && page.IsStartPage)
            {
                sb.AppendLine($"<meta name=\"og:type\" value=\"website\">");
            }
            else
            {
                sb.AppendLine($"<meta name=\"og:type\" value=\"article\">");
            }
            sb.AppendLine($"<meta name=\"og:title\" value=\"{ OgTitle(content) }\">");
            if (content.OgImage != null && content.OgImage.HasValue)
            {
                sb.AppendLine($"<meta name=\"og:image\" value=\"{ app.AbsoluteContentUrl(content.OgImage) }\">");
            }
            else if (content is RoutedContentBase contentBase && contentBase.PrimaryImage != null && contentBase.PrimaryImage.HasValue)
            {
                // If there's no OG image specified but we have a primary image,
                // default to the primary image.
                sb.AppendLine($"<meta name=\"og:image\" value=\"{ app.AbsoluteContentUrl(contentBase.PrimaryImage) }\">");
            }
            if (!string.IsNullOrWhiteSpace(OgDescription(content)))
            {
                sb.AppendLine($"<meta name=\"og:description\" value=\"{ OgDescription(content) }\">");
            }
        }
        return new HtmlString(sb.ToString());
    }

    private static string MetaTitle(IMeta content)
    {
        return !string.IsNullOrWhiteSpace(content.MetaTitle) ? content.MetaTitle : content.Title;
    }

    public static string OgTitle(IMeta content)
    {
        return !string.IsNullOrWhiteSpace(content.OgTitle) ? content.OgTitle : MetaTitle(content);
    }

    public static string OgDescription(IMeta content)
    {
        return !string.IsNullOrWhiteSpace(content.OgDescription) ? content.OgDescription : content.MetaDescription;
    }
}