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

namespace Piranha.Web
{
    public class HttpCacheInfo
    {
        /// <summary>
        /// Gets/sets the entity tag.
        /// </summary>
        public string EntityTag { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime? LastModified { get; set; }
    }
}