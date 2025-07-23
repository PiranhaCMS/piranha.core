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
/// Single column text block.
/// </summary>
[BlockType(Name = "Text", Category = "Content", Icon = "fas fa-font", Component = "text-block")]
public class TextBlock : Block, ISearchable, ITranslatable
{
    /// <summary>
    /// Gets/sets the text body.
    /// </summary>
    public TextField Body { get; set; }

    /// <inheritdoc />
    public override string GetTitle()
    {
        if (Body?.Value != null)
        {
            return Body.Value;
        }
        return "Empty";
    }

    /// <inheritdoc />
    public string GetIndexedContent()
    {
        return !string.IsNullOrEmpty(Body.Value) ? Body.Value : "";
    }

    /// <summary>
    /// Implicitly converts the Text block to a string.
    /// </summary>
    /// <param name="block">The block</param>
    public static implicit operator string(TextBlock block)
    {
        return block.Body?.Value;
    }
}
