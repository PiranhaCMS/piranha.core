/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.Json;

namespace Piranha.Data.RavenDb.Data;

public abstract class ContentRevisionBase : Entity
{
    /// <summary>
    /// Gets/sets the data of the revision serialized
    /// as JSON.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets the revision data deserialized as the
    /// specified type.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <returns>The deserialized revision data</returns>
    public T GetData<T>()
    {
        if (!string.IsNullOrEmpty(Data))
            return JsonSerializer.Deserialize<T>(Data);
        return default(T);
    }
}
