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
    /// Edit model for custom editors.
    /// </summary>
    public class EditorModel
    {
        /// <summary>
        /// Gets/sets the unique client id.
        /// </summary>
        public string Uid { get; set; } = "uid-" + Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        /// <summary>
        /// Gets/sets the editor component.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets/sets the optional icon css.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets/sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}