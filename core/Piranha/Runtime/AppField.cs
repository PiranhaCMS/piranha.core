﻿/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Runtime
{
    public sealed class AppField : AppDataItem
    {
        /// <summary>
        /// Gets/sets the display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the shorthand name.
        /// </summary>
        public string Shorthand { get; set; }

        /// <summary>
        /// Gets/sets the name of the component that should be
        /// used to render the field in the manager interface.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets/sets the optional init methods.
        /// </summary>
        public AppInitMethod Init { get; set; } = new AppInitMethod();
    }
}
