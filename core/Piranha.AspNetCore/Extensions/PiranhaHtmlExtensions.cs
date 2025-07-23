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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Piranha.AspNetCore.Services;
using Piranha.Extend;
using Piranha.Models;

/// <summary>
/// Extension class with html helper methods.
/// </summary>
public static class PiranhaHtmlExtensions
{
    /// <summary>
    /// Converts the type name of the block into a pretty
    /// css class name.
    /// </summary>
    /// <param name="block">The current block</param>
    /// <returns>The css class name</returns>
    public static string CssName(this Block block)
    {
        return ClassNameToWebName(block.GetType().Name);
    }

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
            sb.AppendLine($"<meta name=\"robots\" content=\"{ MetaRobots(content) }\">");

            if (!string.IsNullOrWhiteSpace(content.MetaKeywords))
            {
                sb.AppendLine($"<meta name=\"keywords\" content=\"{ content.MetaKeywords }\">");
            }
            if (!string.IsNullOrWhiteSpace(content.MetaDescription))
            {
                sb.AppendLine($"<meta name=\"description\" content=\"{ content.MetaDescription }\">");
            }
        }

        if (generator)
        {
            // Generate generator tag
            sb.AppendLine($"<meta name=\"generator\" content=\"Piranha CMS { Piranha.Utils.GetAssemblyVersion(typeof(Piranha.App).Assembly) }\">");
        }

        if (opengraph)
        {
            // Generate open graph tags
            if (content is PageBase page && page.IsStartPage)
            {
                sb.AppendLine($"<meta property=\"og:type\" content=\"website\"/>");
            }
            else
            {
                sb.AppendLine($"<meta property=\"og:type\" content=\"article\"/>");
            }
            sb.AppendLine($"<meta property=\"og:title\" content=\"{ OgTitle(content) }\"/>");
            if (content.OgImage != null && content.OgImage.HasValue)
            {
                sb.AppendLine($"<meta property=\"og:image\" content=\"{ app.AbsoluteContentUrl(content.OgImage) }\"/>");
            }
            else if (content is RoutedContentBase contentBase && contentBase.PrimaryImage != null && contentBase.PrimaryImage.HasValue)
            {
                // If there's no OG image specified but we have a primary image,
                // default to the primary image.
                sb.AppendLine($"<meta property=\"og:image\" content=\"{ app.AbsoluteContentUrl(contentBase.PrimaryImage) }\"/>");
            }
            if (!string.IsNullOrWhiteSpace(OgDescription(content)))
            {
                sb.AppendLine($"<meta property=\"og:description\" content=\"{ OgDescription(content) }\"/>");
            }
        }
        return new HtmlString(sb.ToString());
    }

    private static string MetaTitle(IMeta content)
    {
        return !string.IsNullOrWhiteSpace(content.MetaTitle) ? content.MetaTitle : content.Title;
    }

    private static string MetaRobots(IMeta content)
    {
        return (content.MetaIndex ? "index," : "noindex,") + (content.MetaFollow ? "follow" : "nofollow");
    }

    private static string OgTitle(IMeta content)
    {
        return !string.IsNullOrWhiteSpace(content.OgTitle) ? content.OgTitle : MetaTitle(content);
    }

    private static string OgDescription(IMeta content)
    {
        return !string.IsNullOrWhiteSpace(content.OgDescription) ? content.OgDescription : content.MetaDescription;
    }

    /// <summary>
    /// Converts a standard camel case class name to a lowercase
    /// string with each word separated with a dash, suitable
    /// for use in views.
    /// </summary>
    /// <param name="str">The camel case string</param>
    /// <returns>The converted string</returns>
    private static string ClassNameToWebName(string str)
    {
        return Regex.Replace(str, "([A-Z])", " $1", RegexOptions.Compiled).Trim().Replace(" ", "-").ToLower();
    }
}
