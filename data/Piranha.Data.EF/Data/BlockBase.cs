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
/// Abstract base class for all content blocks
/// </summary>
[Serializable]
public abstract class BlockBase<T> where T : BlockFieldBase
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// This is not part of the data model. It's only used
    /// for internal mapping.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets/sets the CLR type of the block.
    /// </summary>
    public string CLRType { get; set; }

    /// <summary>
    /// Gets/sets the available fields.
    /// </summary>
    public IList<T> Fields { get; set; } = new List<T>();
}
