/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.Json.Serialization;

namespace Piranha.Data.RavenDb.Data;

[Serializable]
public sealed class PostField : ContentFieldBase
{
    /// <summary>
    /// Gets/sets the post id.
    /// </summary>
    public string PostId { get; set; }

    /// <summary>
    /// Gets/sets the post.
    /// </summary>
    [JsonIgnore]
    public Post Post { get; set; }
}
