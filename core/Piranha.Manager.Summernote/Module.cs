/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Reflection;
using Piranha.Extend;

namespace Piranha.Manager.Summernote
{
    public sealed class Module : IModule
    {
        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha.Manager.Summernote";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Summernote WYSIWYG Editor for Piranha CMS.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/Piranha.Manager.Summernote";

        /// <summary>
        /// Gets the icon url.
        /// </summary>
        public string IconUrl => "http://piranhacms.org/assets/twitter-shield.png";

        /// <summary>
        /// The assembly.
        /// </summary>
        internal static readonly Assembly Assembly;

        /// <summary>
        /// Static initialization.
        /// </summary>
        static Module()
        {
            // Get assembly information
            Assembly = typeof(Module).GetTypeInfo().Assembly;
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init() { }
    }
}
