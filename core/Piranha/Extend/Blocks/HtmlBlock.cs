/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.RegularExpressions;
using Piranha.Extend.Fields;

namespace Piranha.Extend.Blocks;

/// <summary>
/// Single column HTML block.
/// </summary>
[BlockType(Name = "Content", Category = "Content", Icon = "fas fa-paragraph", Component = "html-block")]
public class HtmlBlock : Block, ISearchable, ITranslatable
{
    /// <summary>
    /// Gets/sets the HTML body.
    /// </summary>
    public HtmlField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body?.Value != null)
        {
            var title = Regex.Replace(Body.Value, @"<[^>]*>", "");

            if (title.Length > 40)
            {
                title = title.Substring(0, 40) + "...";
            }
            return title;
        }
        return "Empty";
    }

    /// <inheritdoc />
    public string GetIndexedContent()
    {
        return !string.IsNullOrEmpty(Body.Value) ? Body.Value : "";
    }

    /// <summary>
    /// Implicitly converts the Html block to a string.
    /// </summary>
    /// <param name="block">The block</param>
    public static implicit operator string(HtmlBlock block)
    {
        return block.Body?.Value;
    }
}
