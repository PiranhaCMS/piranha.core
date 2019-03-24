/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Page list model.
    /// </summary>
    public class PageListModel
    {
        public class ListItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string TypeName { get; set; }
            public string Published { get; set; }
            public List<ListItem> Items { get; set; } = new List<ListItem>();
        }

        /// <summary>
        /// Gets/set the available items.
        /// </summary>
        public IList<ListItem> Items { get; set; } = new List<ListItem>();
    }
}