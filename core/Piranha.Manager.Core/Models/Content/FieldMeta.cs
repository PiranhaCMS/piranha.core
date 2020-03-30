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

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Meta information for fields.
    /// </summary>
    public class FieldMeta : ContentMeta
    {
        /// <summary>
        /// Gets/sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets if the field should be displayed half width.
        /// </summary>
        public bool IsHalfWidth { get; set; }

        /// <summary>
        /// Gets/sets if this field should notify parent on change.
        /// </summary>
        public bool NotifyChange { get; set; }

        /// <summary>
        /// Gets/sets the field options
        /// </summary>
        public IDictionary<int, string> Options { get; set; } = new Dictionary<int, string>();
    }
}