/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using Piranha.Extend.Fields;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for project defined content.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public abstract class Content<T> where T : Content<T>
    {
        /// <summary>
        /// Gets/sets the optional primary image.
        /// </summary>
        public ImageField PrimaryImage { get; set; } = new ImageField();

        /// <summary>
        /// Gets/sets the optional excerpt.
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Gets/sets the optional category.
        /// </summary>
        public Taxonomy Category { get; set; }

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();
    }
}