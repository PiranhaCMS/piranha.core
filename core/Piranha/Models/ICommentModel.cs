/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Models
{
    /// <summary>
    /// Interface for a content object that supports comments.
    /// </summary>
    public interface ICommentModel
    {
        /// <summary>
        /// Gets/sets if comments should be enabled.
        /// </summary>
        /// <value></value>
        bool EnableComments { get; set; }
    }
}