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

[Serializable]
public sealed class ContentGroup : Models.ContentGroup
{
    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
