/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Models
{
    [Serializable]
    public class ContentTypeFieldInfoBase
    {
        /// <summary>
        /// Gets/sets the type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets/sets the assembly name.
        /// </summary>
        public string AssemblyName { get; set; }
    }
}