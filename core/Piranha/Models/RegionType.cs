/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Models
{
    [Serializable]
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
        public bool ListExpand { get; set; } = true;

        /// <summary>
        /// Gets/sets the optional description to be shown in
        /// the manager interface.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the optional icon css.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/sets how the region should be displayed in
        /// the manager interface.
        /// </summary>
        public RegionDisplayMode Display { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<FieldType> Fields { get; set; } = new List<FieldType>();
    }
}
