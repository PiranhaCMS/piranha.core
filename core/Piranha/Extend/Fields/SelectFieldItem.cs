/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Extend.Fields
{
    /// <summary>
    /// An available item to choose from for a SelectField.
    /// </summary>
    public class SelectFieldItem
    {
        /// <summary>
        /// Gets/sets the display title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the enum value.
        /// </summary>
        public Enum Value { get; set; }
    }
}