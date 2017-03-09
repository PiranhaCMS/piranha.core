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
    /// Interface for defining a Piranha module.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        void Init();
    }
}
