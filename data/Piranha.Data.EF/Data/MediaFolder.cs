/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Data
{
    [Serializable]
    public sealed class MediaFolder : Models.MediaFolder<Guid>
    {
        /// <summary>
        /// Gets/sets the optional parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the available media.
        /// </summary>
        public IList<Media> Media { get; set; } = new List<Media>();
    }
}