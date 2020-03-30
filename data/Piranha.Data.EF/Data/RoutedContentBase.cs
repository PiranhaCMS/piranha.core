/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;

namespace Piranha.Data
{
    [Serializable]
    public abstract class RoutedContentBase<T> : ContentBase<T> where T : ContentFieldBase
    {
        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
	    public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
	    public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
	    public string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the optional route.
        /// </summary>
	    public string Route { get; set; }

        /// <summary>
        /// Gets/sets the publishe date.
        /// </summary>
	    public DateTime? Published { get; set; }
    }
}