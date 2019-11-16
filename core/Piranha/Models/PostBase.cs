/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for post models.
    /// </summary>
    public abstract class PostBase : RoutedContent, IBlockModel, IMeta
    {
        /// <summary>
        /// Gets/sets the blog page id.
        /// </summary>
        [Required]
        public Guid BlogId { get; set; }

        /// <summary>
        /// Gets/sets the category.
        /// </summary>
        [Required]
        public Taxonomy Category { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        [StringLength(256)]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        public RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

        /// <summary>
        /// Gets/sets the available blocks.
        /// </summary>
        public IList<Extend.Block> Blocks { get; set; } = new List<Extend.Block>();
    }
}
