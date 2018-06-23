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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BlockItemTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the type of the accepted child item.
        /// </summary>
        public Type Type { get; set; }
    }
}
