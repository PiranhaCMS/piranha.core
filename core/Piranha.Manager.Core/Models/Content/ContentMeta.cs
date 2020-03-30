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

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Meta information for content.
    /// </summary>
    public class ContentMeta
    {
        /// <summary>
        /// Gets/sets the unique client id.
        /// </summary>
        public string Uid { get; set; } = "uid-" + Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        /// <summary>
        /// Gets/sets the type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the title if used in a list.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the type icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the optional placeholder.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Gets/sets the client component.
        /// </summary>
        public string Component { get; set; }
    }
}