/*
 * Copyright (c) 2016-2017 Håkan Edling
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
    /// An item in an app data list.
    /// </summary>
    public class AppDataItem
    {
        /// <summary>
        /// Gets/sets the type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets/sets the full type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets/sets the short assembly name.
        /// </summary>
        public string AssemblyName { get; set; }
    }
}
