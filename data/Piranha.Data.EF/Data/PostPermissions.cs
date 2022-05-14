/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Newtonsoft.Json;

namespace Piranha.Data
{
    [Serializable]
    public sealed class PostPermission
    {
        public Guid PostId { get; set; }
        public string Permission { get; set; }

        [JsonIgnore]
        public Post Post { get; set; }
    }
}
