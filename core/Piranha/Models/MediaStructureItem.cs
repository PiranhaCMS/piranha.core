/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Models
{
    [Serializable]
    public class MediaStructureItem : StructureItem<MediaStructure, MediaStructureItem>
    {
        /// <summary>
        /// Gets/sets the folder name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaStructureItem()
        {
            Items = new MediaStructure();
        }
    }
}