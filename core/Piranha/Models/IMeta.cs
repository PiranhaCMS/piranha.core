/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Models
{
    public interface IMeta
    {
        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional meta keywords.
        /// </summary>
        string MetaKeywords { get; set; }

        /// <summary>
        /// Gets/sets the optional meta description.
        /// </summary>
        string MetaDescription { get; set; }

        /// <summary>
        /// Gets/sets the optional robots information
        /// </summary>
        string MetaRobots { get; set; }
	}
}
