/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Extend
{
    public class ToolbarAction : IAction
    {
        /// <summary>
        /// Gets/sets the internal id of the action.
        /// </summary>
        public string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the name of the view that should be inserted
        /// into the action bar for the page.
        /// </summary>
        public string ActionView { get; set; }

        /// <summary>
        /// Gets/sets the name of the optional partial view that
        /// should be inserted at the bottom of the page.
        /// </summary>
        public string PartialView { get; set; }
    }
}