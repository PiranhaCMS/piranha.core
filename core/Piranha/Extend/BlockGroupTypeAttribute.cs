/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Extend
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlockGroupTypeAttribute : BlockTypeAttribute
    {
        /// <summary>
        /// Gets/sets if the block group should use a custom
        /// view for rendering block global fields. The default
        /// value for this property is false.
        /// </summary>
        public bool UseCustomView { get; set; }
    }
}
