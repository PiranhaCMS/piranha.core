/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Newtonsoft.Json;

namespace Piranha.Data;

/// <summary>
/// Content field for a block.
/// </summary>
[Serializable]
public sealed class BlockField : BlockFieldBase
{
    /// <summary>
    /// Gets/sets the block containing the field.
    /// </summary>
    [JsonIgnore]
    public Block Block { get; set; }
}
