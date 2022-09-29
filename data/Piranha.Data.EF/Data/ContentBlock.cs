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
/// Basic content block.
/// </summary>
[Serializable]
public sealed class ContentBlock : BlockBase<ContentBlockField>
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Gets/sets the zero based sort index.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    [JsonIgnore]
    public Content Content { get; set; }
}
