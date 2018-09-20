/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Models
{
    public sealed class FieldType
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
        /// Gets/sets the value type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets/sets the options.
        /// </summary>
        public FieldOption Options { get; set; }

        /// <summary>
        /// Gets/sets the optional placeholder for
        /// text based fields.
        /// </summary>
        public string Placeholder { get; set; }
    }
}
