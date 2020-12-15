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

namespace Piranha.Manager.Models
{
    public sealed class ContentTaxonomies
    {
        /// <summary>
        /// Gets/sets the selected category.
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// Gets/sets the selected tags.
        /// </summary>
        public IList<string> SelectedTags { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets the available categories.
        /// </summary>
        public IList<string> Categories { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets the available tags.
        /// </summary>
        public IList<string> Tags { get; set; } = new List<string>();
    }
}