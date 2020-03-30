/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Editor
{
    /// <summary>
    /// A editor style item.
    /// </summary>
    public class EditorStyle
    {
        /// <summary>
        /// Gets/sets the display title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the tag the style should apply for.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets/sets the element type of the tag.
        /// </summary>
        public EditorStyleType Type { get; set; }

        /// <summary>
        /// Gets/sets the optional css classes that should be applied.
        /// </summary>
        public string Classes { get; set; }

        public bool HasClasses { get { return !string.IsNullOrWhiteSpace(Classes); }}
    }
}