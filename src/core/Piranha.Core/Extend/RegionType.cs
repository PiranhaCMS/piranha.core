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
    public sealed class RegionType
    {
        #region Properties
        /// <summary>
        /// Gets/sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the optional title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets if this region has a collection of values.
        /// </summary>
        public bool Collection { get; set; }

        /// <summary>
        /// Gets/sets the maximum number of items if this is a collection.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Gets/sets the minimum number of items if this is a collection.
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<FieldType> Fields { get; set; }
        #endregion

        /// <summary>
        /// Default internal constructor.
        /// </summary>
        internal RegionType() {
            Fields = new List<FieldType>();
        }
    }
}
