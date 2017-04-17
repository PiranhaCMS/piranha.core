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

namespace Piranha.Areas.Manager.Models
{
    public class PageStructureModel
    {
        public class PageStructureItem
        {
            /// <summary>
            /// Gets/sets the unique page id.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets/sets the available children.
            /// </summary>
            public IList<PageStructureItem> Children { get; set; }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public PageStructureItem() {
                Children = new List<PageStructureItem>();
            }
        }

        /// <summary>
        /// Gets/sets the structure items.
        /// </summary>
        public IList<PageStructureItem> Items { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageStructureModel() {
            Items = new List<PageStructureItem>();
        }
    }
}