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
    /// <summary>
    /// Abstract class for an hierarchical item in a structure.
    /// </summary>
    [Serializable]
    public abstract class StructureItem<TStructure, T>
        where T : StructureItem<TStructure, T>
        where TStructure : Structure<TStructure, T>
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the level in the hierarchy.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets/sets the child items.
        /// </summary>
        public TStructure Items { get; set; }
    }
}