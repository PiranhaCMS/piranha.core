/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Extend;

/// <summary>
/// Attribute for marking a class as a block group type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BlockGroupTypeAttribute : BlockTypeAttribute
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public BlockGroupTypeAttribute()
    {
        _component = null;
    }

    /// <summary>
    /// Gets/sets how the blocks inside the group should be
    /// displayed in the manager interface.
    /// </summary>
    public BlockDisplayMode Display { get; set; } = BlockDisplayMode.MasterDetail;
}
