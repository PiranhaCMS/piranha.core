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

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Attribute for marking a class as a page type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PostTypeAttribute : ContentTypeAttribute
    {
        /// <summary>
        /// Gets/sets if the post type should use the block editor
        /// for its main content. The default value is True.
        /// </summary>
        public bool UseBlocks { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostTypeAttribute() : base()
        {
            UseBlocks = true;
        }
    }
}
