/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Builder.Attribute
{
    /// <summary>
    /// Attribute for marking a class as a block type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BlockTypeAttribute : ContentTypeAttribute
    {
        /// <summary>
        /// Gets/sets the optional view.
        /// </summary>
        public string View { get; set; }
    }
}
