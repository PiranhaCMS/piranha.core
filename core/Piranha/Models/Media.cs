/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.ComponentModel.DataAnnotations;

namespace Piranha.Models;

[Serializable]
public sealed class Media : MediaBase
{
    /// <summary>
    /// Gets/sets the user defined properties.
    /// </summary>
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets/sets the available versions.
    /// </summary>
    public IList<MediaVersion> Versions { get; set; } = new List<MediaVersion>();
}
