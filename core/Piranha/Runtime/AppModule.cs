/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;

namespace Piranha.Runtime
{
    public sealed class AppModule : AppDataItem
    {
        /// <summary>
        /// Gets/sets the module instance.
        /// </summary>
        public IModule Instance { get; set; }
    }
}
