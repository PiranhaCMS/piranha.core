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
using System.Collections.Generic;

namespace Piranha.Models
{
    [Serializable]
    public sealed class ContentTypeFieldInfo : ContentTypeFieldInfoBase
    {
        /// <summary>
        /// Gets/sets if this is a generic type.
        /// </summary>
        public bool IsGeneric { get; set; }

        /// <summary>
        /// Gets/sets the available generic type arguments
        /// if this is a generic type.
        /// </summary>
        public IList<ContentTypeFieldInfoBase> TypeArguments { get; set; } = new List<ContentTypeFieldInfoBase>();
    }
}