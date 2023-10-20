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
/// Attribute for specifying an allowed block item type for a block group.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BlockItemTypeAttribute : Attribute
{
    /// <summary>
    /// Gets/sets the type of the accepted child item.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BlockItemTypeAttribute() { }

    /// <summary>
    /// Creates and initializes the item type.
    /// </summary>
    /// <param name="type">The specified item type</param>
    public BlockItemTypeAttribute(Type type)
    {
        Type = type;
    }
}
