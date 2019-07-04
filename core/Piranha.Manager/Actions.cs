/*
 * Copyright (c) 2019 Håkan Edling
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
    /// Static class for defining the manager menu.
    /// </summary>
    public static class Actions
    {
        /// <summary>
        /// Gets/sets the available actions for the page edit view.
        /// </summary>
        public static ActionList PageEdit { get; set; } = new ActionList
        {
            new ActionItem
            {
                InternalId = "Save",
                ActionView = "Partial/Actions/_PageSave"
            }
        };

        /// <summary>
        /// Gets/sets the available actions for the page edit view.
        /// </summary>
        public static ActionList PostEdit { get; set; } = new ActionList
        {
            new ActionItem
            {
                InternalId = "Save",
                ActionView = "Partial/Actions/_PostSave"
            }
        };

        public static ActionList PageList { get; set; } = new ActionList();
    }
}