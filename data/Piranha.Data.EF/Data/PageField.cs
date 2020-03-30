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
using Newtonsoft.Json;

namespace Piranha.Data
{
    [Serializable]
    public sealed class PageField : ContentFieldBase
    {
        /// <summary>
        /// Gets/sets the page id.
        /// </summary>
        public Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the page.
        /// </summary>
        [JsonIgnore]
        public Page Page { get; set; }
    }
}
