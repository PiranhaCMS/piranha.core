/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    public sealed class RegionType
    {
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
        //public int Max { get; set; }

        /// <summary>
        /// Gets/sets the minimum number of items if this is a collection.
        /// </summary>
        //public int Min { get; set; }

        /// <summary>
        /// Gets/sets the field that should be used to render list item title.
        /// </summary>
        public string ListTitleField { get; set; }

        /// <summary>
        /// Gets/sets the placeholder title that should be used for new items.
        /// </summary>
        public string ListTitlePlaceholder { get; set; }

        /// <summary>
        /// Gets/sets if list items should be expandable. If not, the
        /// content is placed directly in the title.
        /// </summary>
        public bool ListExpand { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<FieldType> Fields { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegionType() {
            Fields = new List<FieldType>();
            ListExpand = true;
        }
    }
}
