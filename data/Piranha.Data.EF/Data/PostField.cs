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
    public sealed class PostField : ContentFieldBase
    {
        /// <summary>
        /// Gets/sets the post id.
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets/sets the post.
        /// </summary>
        [JsonIgnore]
        public Post Post { get; set; }
    }
}
