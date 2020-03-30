/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

namespace Piranha.Security
{
    /// <summary>
    /// An item in the permission manager.
    /// </summary>
    public class PermissionItem
    {
        /// <summary>
        /// The name of the claim.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional category for grouping.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets/sets if this is an internal permissions used
        /// by Piranha.
        /// </summary>
        public bool IsInternal { get; set; }
    }
}