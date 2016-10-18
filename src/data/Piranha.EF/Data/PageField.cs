/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.EF.Data
{
    public sealed class PageField : IModel
    {
        #region Properties
        /// <summary>
        /// Gets/sets the page id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the page id.
        /// </summary>
        public Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the region id.
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// Gets/sets the field id.
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// Gets/sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the sort order of the value.
        /// </summary>
        public string CLRType { get; set; }

        /// <summary>
        /// Gets/sets the JSON serialized value.
        /// </summary>
        public string Value { get; set; }
        #endregion

        #region Navigation properties
        /// <summary>
        /// Gets/sets the page
        /// </summary>
        public Page Page { get; set; }
        #endregion
    }
}
