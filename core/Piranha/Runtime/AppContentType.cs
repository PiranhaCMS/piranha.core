/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Runtime
{
    /// <summary>
    /// Class for a registered content type.
    /// </summary>
    public sealed class AppContentType
    {
        /// <summary>
        /// Gets/sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the base type or interface type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets/sets the display title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets if a custom editor should be inserted.
        /// </summary>
        public bool CustomEditor { get; set; }
    }
}
