/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Models
{
    [Serializable]
    public sealed class ContentTypeField
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
        /// Gets/sets the type information.
        /// </summary>
        public ContentTypeFieldInfo Type { get; set; } = new ContentTypeFieldInfo();

        /// <summary>
        /// Gets/sets the options.
        /// </summary>
        public FieldOption Options { get; set; }

        /// <summary>
        /// Gets/sets the optional placeholder for
        /// text based fields.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Gets/sets the optional description to be shown in
        /// the manager interface.
        /// </summary>
        public string Description { get; set; }
    }
}
