/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;

namespace Piranha.Data
{
    /// <summary>
    /// Interface for models tracking last modification date.
    /// </summary>
    public interface IModified
    {
        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        DateTime LastModified { get; set; }
    }
}
