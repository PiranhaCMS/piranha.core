/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using System;

namespace Piranha.Extend
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentGroupAttribute : Attribute
    {
        private string _title;

        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;

                if (string.IsNullOrWhiteSpace(Id))
                {
                    Id = Utils.GenerateInteralId(value);
                }
            }
        }

        /// <summary>
        /// Gets/set the icon css.
        /// </summary>
        public string Icon { get; set; }
    }
}