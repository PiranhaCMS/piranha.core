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
/// Block for referencing a content model.
/// </summary>
[BlockType(Name = "Content link", Category = "References", Icon = "fas fa-link", Component = "content-block")]
public class ContentBlock : Block
{
    /// <summary>
    /// Gets/sets the page link.
    /// </summary>
    public ContentField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body != null && Body.Content != null)
        {
            return Body.Content.Title;
        }
        return "No content selected";
    }
}
