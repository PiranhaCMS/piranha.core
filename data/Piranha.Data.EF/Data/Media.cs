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
    public sealed class Media : Models.Media<Guid>
    {
        /// <summary>
        /// Gets/sets the optional folder id.
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// Gets/sets the optional folder.
        /// </summary>
        public MediaFolder Folder { get; set; }

        /// <summary>
        /// Gets/sets the available versions.
        /// </summary>
        public IList<MediaVersion> Versions { get; set; } = new List<MediaVersion>();
    }
}