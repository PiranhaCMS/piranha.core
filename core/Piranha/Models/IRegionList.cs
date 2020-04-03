/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Models
{
    /// <summary>
    /// Interface for accessing the meta data of a region list.
    /// </summary>
    public interface IRegionList
    {
        /// <summary>
        /// Gets/sets the page type id.
        /// </summary>
        string TypeId { get; set; }

        /// <summary>
        /// Gets/sets the region id.
        /// </summary>
        string RegionId { get; set; }

        /// <summary>
        /// Gets/sets the parent model.
        /// </summary>
        IDynamicContent Model { get; set; }

        /// <summary>
        /// Clears the list
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a new item to the region list
        /// </summary>
        /// <param name="item">The item</param>
        void Add(object item);
    }
}
