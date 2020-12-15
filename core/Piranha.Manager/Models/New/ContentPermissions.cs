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

namespace Piranha.Manager.Models
{
    public sealed class ContentPermissions
    {
        /// <summary>
        /// Gets/sets the currently selected permissions.
        /// </summary>
        public IList<string> SelectedPermissions { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets all of the available permissions.
        /// </summary>
        public IList<KeyValuePair<string, string>> Permissions { get; set; } = new List<KeyValuePair<string, string>>();
    }
}