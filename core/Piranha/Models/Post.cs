/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Piranha.Extend;
using Piranha.Extend.Fields;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for all posts.
    /// </summary>
    [ContentGroup(Title = "Post", DefaultRoute = "/post", IsPrimaryContent = false)]
    public abstract class Post : RoutedContent, IBlockContent
    {
        /// <summary>
        /// Gets/sets the category.
        /// </summary>
        [Required]
        public Taxonomy Category { get; set; }

        /// <summary>
        /// Gets/sets the primary image.
        /// </summary>
        [Region]
        public ImageField PrimaryImage { get; set; }

        /// <summary>
        /// Gets/sets the excerpt.
        /// </summary>
        [Region]
        public TextField Excerpt { get; set; }

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Block> Blocks { get; set; }

        /// <summary>
        /// Gets/sets the availabel tags.
        /// </summary>
        public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();
    }
}