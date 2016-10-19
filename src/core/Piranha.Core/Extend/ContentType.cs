/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Extend
{
    /// <summary>
    /// Abstract base class for content types.
    /// </summary>
    public abstract class ContentType
    {
        #region Properties
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the available regions.
        /// </summary>
        public IList<RegionType> Regions { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ContentType() {
            Regions = new List<RegionType>();
        }
    }
}
