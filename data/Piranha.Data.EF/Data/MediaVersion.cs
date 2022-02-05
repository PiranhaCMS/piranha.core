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
    public sealed class MediaVersion : Models.MediaVersion
    {
        /// <summary>
        /// Gets/sets the id of the media this is
        /// a version of.
        /// </summary>
        public Guid MediaId { get; set; }

        /// <summary>
        /// Gets/sets the media this is a version of.
        /// </summary>
        [JsonIgnore]
        public Media Media { get; set; }
    }
}