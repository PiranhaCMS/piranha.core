/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Models
{
    /// <summary>
    /// Abstract class for an hierarchical item in a structure.
    /// </summary>
    public abstract class StructureItem<T> where T : StructureItem<T>
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
        public IList<T> Items { get; set; } = new List<T>();
    }
}