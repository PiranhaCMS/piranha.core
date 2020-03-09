/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Extend;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for all archives.
    /// </summary>
    [ContentGroup(Title = "Archive")]
    [ContentGroupChild(typeof(Post))]
    public abstract class Archive : RoutedContent
    {
    }
}