/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using Piranha;
using Piranha.Extend;

namespace SimpleModule
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class Module : IModule
    {
        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Test Author";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "SimpleModule";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Simple Module";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/SimpleModule";

        /// <summary>
        /// Gets the icon url.
        /// </summary>
        public string IconUrl => "http://piranhacms.org/assets/twitter-shield.png";

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init()
        {
            App.Blocks.Register<Blocks.HeaderBlock>();
        }
    }
}