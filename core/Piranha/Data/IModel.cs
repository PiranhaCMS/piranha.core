/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

namespace Piranha.Data
{
    /// <summary>
    /// Interface for models with a Guid id.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        string Id { get; set; }
    }
}
