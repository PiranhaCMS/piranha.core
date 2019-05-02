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
    /// Block list model.
    /// </summary>
    public class BlockListModel
    {
        public class ListCategory
        {
            public string Name { get; set; }
            public IList<ListItem> Items { get; set; } = new List<ListItem>();
        }

        public class ListItem
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Type { get; set; }
        }

        public IList<ListCategory> Categories { get; set; } = new List<ListCategory>();
        public int TypeCount { get; set; }
    }
}