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
using System.Linq;

namespace Piranha.Manager
{
    public class MenuItemList : List<MenuItem>
    {
        /// <summary>
        /// Gets the menu item with the given internal id.
        /// </summary>
        public MenuItem this[string internalId] {
            get
            {
                return this.FirstOrDefault(i => i.InternalId == internalId);
            }
        }
    }
}