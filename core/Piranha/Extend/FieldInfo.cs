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

namespace Piranha.Extend
{
    public sealed class FieldInfo
    {
        /// <summary>
        /// Gets/sets the field name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the optional shorthand.
        /// </summary>
        public string Shorthand { get; set; }

        /// <summary>
        /// Gets/sets the CLRType.
        /// </summary>
        public string CLRType { get; set; }

        /// <summary>
        /// Gets/sets the type.
        /// </summary>
        public Type Type { get; set; }
    }
}
