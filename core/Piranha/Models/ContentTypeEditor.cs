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

namespace Piranha.Models
{
    /// <summary>
    /// Custom editor for a content type.
    /// </summary>
    [Serializable]
    public sealed class ContentTypeEditor
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
