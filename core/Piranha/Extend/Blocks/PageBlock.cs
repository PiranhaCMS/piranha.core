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

namespace Piranha.Extend.Blocks;

/// <summary>
/// Block for referencing a page.
/// </summary>
[BlockType(Name = "Page link", Category = "References", Icon = "fas fa-link", Component = "page-block")]
public class PageBlock : Block
{
    /// <summary>
    /// Gets/sets the page link.
    /// </summary>
    public PageField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Page != null)
        {
            return Body.Page.Title;
        }
        return "No page selected";
    }
}
