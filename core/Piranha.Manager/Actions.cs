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
        /// Gets the available actions for the alias view.
        /// </summary>
        public static ActionList AliasEdit { get; private set; } = new ActionList();

        /// <summary>
        /// Gets the available actions for the config view.
        /// </summary>
        public static ActionList ConfigEdit { get; private set; } = new ActionList();

        /// <summary>
        /// Gets the available actions for the page edit view.
        /// </summary>
        public static ActionList PageEdit { get; private set; } = new ActionList
        {
            new ActionItem
            {
                InternalId = "Preview",
                ActionView = "Partial/Actions/_PagePreview"
            },
            new ActionItem
            {
                InternalId = "Save",
                ActionView = "Partial/Actions/_PageSave"
            },
            new ActionItem
            {
                InternalId = "Delete",
                ActionView = "Partial/Actions/_PageDelete"
            }
        };

        /// <summary>
        /// Gets the actions available for the page list view.
        /// </summary>
        public static ActionList PageList { get; private set; } = new ActionList();

        /// <summary>
        /// Gets the available actions for the page edit view.
        /// </summary>
        public static ActionList PostEdit { get; private set; } = new ActionList
        {
            new ActionItem
            {
                InternalId = "Preview",
                ActionView = "Partial/Actions/_PostPreview"
            },
            new ActionItem
            {
                InternalId = "Save",
                ActionView = "Partial/Actions/_PostSave"
            },
            new ActionItem
            {
                InternalId = "Delete",
                ActionView = "Partial/Actions/_PostDelete"
            }
        };
    }
}