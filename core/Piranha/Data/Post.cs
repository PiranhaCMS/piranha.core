/*
 * Copyright (c) 2011-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Data
{
    public sealed class Post : RoutedContent<PostField>, IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the post type id.
        /// </summary>
        public string PostTypeId { get; set; }

        /// <summary>
        /// Gets/sets the id of the blog page this
        /// post belongs to.
        /// </summary>
        public Guid BlogId { get; set; }

        /// <summary>
        /// Gets/sets the category id.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        /// <returns></returns>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        /// <returns></returns>
        public Models.RedirectType RedirectType { get; set; }

        /// <summary>
        /// Gets/sets the associated post type.
        /// </summary>
        public PostType PostType { get; set; }

        /// <summary>
        /// Gets/sets the blog page this category belongs to.
        /// </summary>
        public Page Blog { get; set; }

        /// <summary>
        /// Gets/sets the post category.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IList<PostTag> Tags { get; set; } = new List<PostTag>();

        /// <summary>
        /// Gets/sets the available post blocks.
        /// </summary>
        public IList<PostBlock> Blocks { get; set; } = new List<PostBlock>();
    }
}
