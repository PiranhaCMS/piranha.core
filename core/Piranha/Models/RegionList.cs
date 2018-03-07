﻿/*
 * Copyright (c) 2016-2017 Håkan Edling
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
        public IDynamicModel Model { get; set; }

        /// <summary>
        /// Creates a new item instance
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The new item</returns>
        public T Create(IApi api)
        {
            return (T)Model.CreateRegion(api, RegionId);
        }

        /// <summary>
        /// Creates and adds a new item instance to the list.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The new item</returns>
        public T CreateAndAdd(IApi api)
        {
            var item = Create(api);

            Add(item);

            return item;
        }

        /// <summary>
        /// Adds a new item to the region list
        /// </summary>
        /// <param name="item">The item</param>
        public void Add(object item)
        {
            if (item.GetType() == typeof(T))
                base.Add((T)item);
            else throw new ArgumentException();
        }
    }
}
