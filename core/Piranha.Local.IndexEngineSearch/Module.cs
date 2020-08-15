/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Extend;

namespace Piranha.Local
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class IndexEngineSearchModule : IModule
    {
        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha.Local.IndexEngineSearch";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Search module for Piranha CMS using IndexEngine.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/Piranha.Local.IndexEngineSearch";

        /// <summary>
        /// Gets the icon url.
        /// </summary>
        public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init() { }
    }
}