/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Extend.Fields.Settings
{
    /// <summary>
    /// Settings for content fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ContentFieldSettingsAttribute : FieldSettingsAttribute
    {
        /// <summary>
        /// Gets/sets the currently allowed types
        /// </summary>
        public IList<string> Types { get; set; } = new List<string>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="types">The allowed content types</param>
        public ContentFieldSettingsAttribute(params Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(Models.ContentBase).IsAssignableFrom(type))
                {
                    var id = App.ContentTypes.FirstOrDefault(t => t.CLRType == type.AssemblyQualifiedName)?.Id;

                    if (!string.IsNullOrEmpty(id))
                    {
                        Types.Add(id);
                    }
                }
                else
                {
                    throw new ArgumentException("You can only allow content types on a Content Field");
                }
            }
        }
    }
}
