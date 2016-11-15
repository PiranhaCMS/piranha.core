/*
 * Copyright (c) 2016 Håkan Edling
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
    public class PostArchive
    {
        #region Properties
        /// <summary>
        /// Gets/sets the unique category id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the meta keywords.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the meta description.
        /// </summary>
        public string MetaDescription { get; set; }

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
        public int Page { get; set; }

        /// <summary>
        /// Gets/sets the total number of pages available.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets/sets the archive route.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets/sets the available posts.
        /// </summary>
        public IList<Post> Posts { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostArchive() {
            Posts = new List<Post>();
        }
    }
}
