/*
 * Copyright (c) 2017 Håkan Edling
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
    /// Abstract class for building a hierarchical structure.
    /// </summary>
    public abstract class Structure<TThis, T> : List<T> where T : StructureItem<T> where TThis : Structure<TThis, T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Structure() : base() {}

        /// <summary>
        /// Gets the partial structure with the items positioned
        /// below the item with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The partial structure</returns>
        public TThis GetPartial(string id) {
            if (!string.IsNullOrWhiteSpace(id))
                return GetPartialRecursive(this, id);
            return (TThis)this;
        }

        /// <summary>
        /// Gets the breadcrumb for the item with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The breadcrumb</returns>
        public IList<T> GetBreadcrumb(string id) {
            if (!string.IsNullOrWhiteSpace(id))
                return GetBreadcrumbRecursive(this, id);
            return new List<T>();
        }        

        /// <summary>
        /// Gets the partial structure by going through the
        /// items recursively.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="pageId">The unique id</param>
        /// <returns>The partial structure if found</returns>
        private TThis GetPartialRecursive(IList<T> items, string id) {
            foreach (var item in items) {
                if (item.Id == id) {
                    return (TThis)item.Items;
                } else {
                    var partial = GetPartialRecursive(item.Items, id);

                    if (partial != null)
                        return (TThis)partial;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the breadcrumb items by going through the
        /// items recursively.
        /// </summary>
        /// <param name="items">The items</param>
        /// <param name="pageId">The unique id</param>
        /// <returns>The breadcrumb items</returns>
        private IList<T> GetBreadcrumbRecursive(IList<T> items, string id) {
            foreach (var item in items) {
                if (item.Id == id) {
                    return new List<T>() {
                        item
                    };
                } else {
                    var crumb = GetBreadcrumbRecursive(item.Items, id);

                    if (crumb != null) {
                        crumb.Insert(0, item);

                        return crumb;
                    }
                }
            }
            return null;
        }
    }
}