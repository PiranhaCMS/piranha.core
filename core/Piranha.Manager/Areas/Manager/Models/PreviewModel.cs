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

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class PreviewModel
    {
        /// <summary>
        /// Gets/sets the content id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the permalink.
        /// </summary>
        public string Permalink { get; set; }
    }
}