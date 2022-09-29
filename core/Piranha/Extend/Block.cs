/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend;

/// <summary>
/// Base class for blocks.
/// </summary>
public abstract class Block
{
    /// <summary>
    /// Gets/sets the id of the block instance.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets/set the block type id.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets the title of the block when used in a block group.
    /// </summary>
    /// <returns>The title</returns>
    public virtual string GetTitle()
    {
        var blockType = App.Blocks.GetByType(GetType());
        var title = "[Not Implemented]";

        if (!string.IsNullOrEmpty(blockType.ListTitleField))
        {
            var prop = GetType().GetProperty(blockType.ListTitleField, App.PropertyBindings);

            if (prop != null && typeof(IField).IsAssignableFrom(prop.PropertyType))
            {
                var field = (IField)prop.GetValue(this);

                title = field.GetTitle();
            }
        }
        return title;
    }
}
