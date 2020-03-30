/*
 * Copyright (c) .NET Foundation and Contributors
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
    /// Interface for fields.
    /// </summary>
    public interface IField
    {
        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        string GetTitle();
    }
}
