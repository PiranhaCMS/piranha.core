/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend
{
    /// <summary>
    /// Base class for fields.
    /// </summary>
    public abstract class Field : IField
    {
        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public abstract string GetTitle();
    }
}
