/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data;

/// <summary>
/// Reusable content block.
/// </summary>
[Serializable]
public sealed class Block : BlockBase<BlockField>
{
    /// <summary>
    /// Gets/sets the optional title. This property
    /// is only used for reusable blocks within the
    /// block library.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets if this is a reusable block.
    /// </summary>
    public bool IsReusable { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
