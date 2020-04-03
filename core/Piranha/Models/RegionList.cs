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
using System.Collections.Generic;

namespace Piranha.Models
{
    /// <summary>
    /// Region list for dynamic models.
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    [Serializable]
    public class RegionList<T> : List<T>, IRegionList
    {
        /// <summary>
        /// Gets/sets the page type id.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the region id.
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// Gets/sets the parent model.
        /// </summary>
        public IDynamicContent Model { get; set; }

        /// <summary>
        /// Adds a new item to the region list
        /// </summary>
        /// <param name="item">The item</param>
        public void Add(object item)
        {
            if (item.GetType() == typeof(T))
            {
                base.Add((T)item);
            }
            else
            {
                throw new ArgumentException("Item type does not match the list");
            }
        }
    }
}
