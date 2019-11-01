/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    public class PostListModel
    {
        public class PostItem
        {
            public static readonly string Draft = "Draft";
            public static readonly string Unpublished = "Unpublished";

            public string Id { get; set; }
            public string Title { get; set; }
            public string TypeName { get; set; }
            public string Category { get; set; }
            public string Published { get; set; }
            public string Status { get; set; }
            public string EditUrl { get; set; }
            public bool isScheduled { get; set; }
        }

        public class PostTypeItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string AddUrl { get; set; }
        }

        public class CategoryItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }

        public IList<PostItem> Posts { get; set; } = new List<PostItem>();
        public IList<PostTypeItem> PostTypes { get; set; } = new List<PostTypeItem>();
        public IList<CategoryItem> Categories { get; set; } = new List<CategoryItem>();
        public int TotalPosts { get; set; }
        public int TotalPages { get; set; }
        public int Index { get; set; }
    }
}