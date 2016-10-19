/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    /// <summary>
    /// Region list for dynamic page models.
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    public class PageRegionList<T> : List<T>, IRegionList
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
        /// Creates a new item instance
        /// </summary>
        /// <returns>The new item</returns>
        public T Create() {
            return (T)PageModel.CreateRegion(TypeId, RegionId);
        }

        /// <summary>
        /// Creates and adds a new item instance to the list.
        /// </summary>
        /// <returns>The new item</returns>
        public T CreateAndAdd() {
            var item = Create();

            Add(item);

            return item;
        }
    }
}
