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
    /// Interface for defining a Piranha module.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Get the author for this module
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Get the name of the module
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the module version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Get the module description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get the package url for the module
        /// </summary>
        string PackageUrl { get; }

        /// <summary>
        /// Gets the logo url for the module.
        /// </summary>
        string IconUrl { get; }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        void Init();
    }
}
