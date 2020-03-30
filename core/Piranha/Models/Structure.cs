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
    /// Abstract class for building a hierarchical structure.
    /// </summary>
    [Serializable]
    public abstract class Structure<TThis, T> : List<T> where T : StructureItem<TThis, T> where TThis : Structure<TThis, T>
    {
        /// <summary>
        /// Gets the partial structure with the items positioned
        /// below the item with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="includeRootNode">If the root node should be included</param>
        /// <returns>The partial structure</returns>
        public TThis GetPartial(Guid? id, bool includeRootNode = false)
        {
            if (id.HasValue)
            {
                return GetPartialRecursive(this, id.Value, includeRootNode);
            }
            return (TThis)this;
        }

        /// <summary>
        /// Gets the breadcrumb for the item with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The breadcrumb</returns>
        public IList<T> GetBreadcrumb(Guid? id)
        {
            if (id.HasValue)
            {
                return GetBreadcrumbRecursive(this, id.Value);
            }
            return new List<T>();
        }

        /// <summary>
        /// Gets the partial structure by going through the
        /// items recursively.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="id">The unique id</param>
        /// <param name="includeRootNode">If the root node should be included</param>
        /// <returns>The partial structure if found</returns>
        private TThis GetPartialRecursive(IList<T> items, Guid id, bool includeRootNode)
        {
            foreach (var item in items)
            {
                if (item.Id == id)
                {
                    if (includeRootNode)
                    {
                        var structure = Activator.CreateInstance<TThis>();
                        structure.Add(item);

                        return structure;
                    }
                    else
                    {
                        return item.Items;
                    }
                }

                var partial = GetPartialRecursive(item.Items, id, includeRootNode);

                if (partial != null)
                {
                    return partial;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the breadcrumb items by going through the
        /// items recursively.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="id">The unique id</param>
        /// <returns>The breadcrumb items</returns>
        private IList<T> GetBreadcrumbRecursive(IList<T> items, Guid id)
        {
            foreach (var item in items)
            {
                if (item.Id == id)
                {
                    return new List<T> {
                        item
                    };
                }

                var crumb = GetBreadcrumbRecursive(item.Items, id);

                if (crumb != null)
                {
                    crumb.Insert(0, item);

                    return crumb;
                }
            }
            return null;
        }
    }
}