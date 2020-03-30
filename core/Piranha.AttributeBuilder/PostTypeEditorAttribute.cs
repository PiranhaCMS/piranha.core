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

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Attribute for adding a custom editor to a post type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PostTypeEditorAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the editor component.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets/sets the optional icon css.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }
    }
}
