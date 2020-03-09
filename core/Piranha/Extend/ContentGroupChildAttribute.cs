/*
 * Copyright (c) 2020 Piranha CMS
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;

namespace Piranha.Extend
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ContentGroupChildAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the allowed content group type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">The allowed child content group</param>
        public ContentGroupChildAttribute(Type type)
        {
            Type = type;
        }
    }
}