/*
 * Copyright (c) 2016-2019 Håkan Edling
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
    public class PostArchive : PostArchive<DynamicPost> { }

    public class PostArchive<T> where T : PostBase
    {
        /// <summary>
        /// Gets/sets the optionally requested year.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Gets/sets the optionally requested month.
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Gets/sets the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets/sets the total number of pages available.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets/sets the total number of posts available.
        /// </summary>
        public int TotalPosts { get; set; }

        /// <summary>
        /// Gets/sets the optionally selected category.
        /// </summary>
        public Taxonomy Category { get; set; }

        /// <summary>
        /// Gets/sets the optionally selected tag.
        /// </summary>
        public Taxonomy Tag { get; set; }

        /// <summary>
        /// Gets/sets the available posts.
        /// </summary>
        public IList<T> Posts { get; set; } = new List<T>();
    }
}
