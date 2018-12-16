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

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class PostEditModel : Piranha.Models.PostBase
    {
        /// <summary>
        /// Gets/sets the post type.
        /// </summary>
        public Piranha.Models.PostType PostType { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public new IList<ContentEditBlock> Blocks { get; set; } = new List<ContentEditBlock>();

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<PageEditRegionBase> Regions { get; set; } = new List<PageEditRegionBase>();

        /// <summary>
        /// Gets/sets the available categories.
        /// </summary>
        public IEnumerable<Data.Category> AllCategories { get; set; } = new List<Data.Category>();

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IEnumerable<Data.Tag> AllTags { get; set; } = new List<Data.Tag>();

        /// <summary>
        /// Gets/sets the currently selected category.
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// Gets/sets the currently selected tags.
        /// </summary>
        public IList<string> SelectedTags { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets the base slug of the blog.
        /// </summary>
        public string BlogSlug { get; set; }
    }
}