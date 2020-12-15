/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend.Fields;

namespace Piranha.Manager.Models
{
    public sealed class ContentMeta
    {
        /// <summary>
        /// Gets/sets the optional meta title.
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the meta index.
        /// </summary>
        public bool MetaIndex { get; set; }

        /// <summary>
        /// Gets/sets the meta follow.
        /// </summary>
        public bool MetaFollow { get; set; }

        /// <summary>
        /// Gets/sets the meta priority.
        /// </summary>
        public double MetaPriority { get; set; }

        /// <summary>
        /// Gets/sets the optional og title.
        /// </summary>
        public string OgTitle { get; set; }

        /// <summary>
        /// Gets/sets the optional og description.
        /// </summary>
        public string OgDescription { get; set; }

        /// <summary>
        /// Gets/sets the optional og image.
        /// </summary>
        public ImageField OgImage { get; set; }
    }
}