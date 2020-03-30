/*
 * Copyright (c) .NET Foundation and Contributors
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
    public class MediaStructure : Structure<MediaStructure, MediaStructureItem>
    {
        /// <summary>
        /// Gets/sets the number of media items in the root folder.
        /// </summary>
        public int MediaCount { get; set; }

        /// <summary>
        /// Gets/sets the total amount of media items in structure.
        /// </summary>
        public int TotalCount { get; set; }
    }
}