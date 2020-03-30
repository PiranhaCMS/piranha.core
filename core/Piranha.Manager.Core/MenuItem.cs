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

namespace Piranha.Manager
{
    /// <summary>
    /// An item in the manager menu.
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Gets/sets the internal id.
        /// </summary>
        public string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the optional css class.
        /// </summary>
        public string Css { get; set; }

        /// <summary>
        /// Gets/sets the route for the menu item.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Gets/sets the policy needed to see this item.
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// Gets/sets the optional menu item params.
        /// </summary>
        public object Params { get; set; }

        /// <summary>
        /// Gets/sets the available items.
        /// </summary>
        public MenuItemList Items { get; set; } = new MenuItemList();
    }
}