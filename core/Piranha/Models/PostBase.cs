/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for post models.
    /// </summary>
    public abstract class PostBase : RoutedContent, IMeta
    {
        #region Properties
        /// <summary>
        /// Gets/sets the category id.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Gets/sets the category name.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets/sets the optional redirect.
        /// </summary>
        /// <returns></returns>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect type.
        /// </summary>
        /// <returns></returns>
        public RedirectType RedirectType { get; set; }
        #endregion
    }
}
