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
        public class SiteItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public string EditUrl { get; set; }
            public IList<PageItem> Pages { get; set; } = new List<PageItem>();
        }

        public class PageItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string TypeName { get; set; }
            public string Published { get; set; }
            public string EditUrl { get; set; }
            public List<PageItem> Items { get; set; } = new List<PageItem>();
        }

        public class PageTypeItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }

        public IList<SiteItem> Sites { get; set; } = new List<SiteItem>();
        public IList<PageTypeItem> PageTypes { get; set; } = new List<PageTypeItem>();
    }
}