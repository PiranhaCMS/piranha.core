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
public sealed class MediaFolder : Models.MediaFolder
{
    /// <summary>
    /// Gets/sets the available media.
    /// </summary>
    public IList<Media> Media { get; set; } = new List<Media>();
}
