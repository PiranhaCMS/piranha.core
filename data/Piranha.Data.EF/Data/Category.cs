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
    public sealed class Category : TaxonomyBase
    {
        /// <summary>
        /// Gets/sets the id of the blog page this
        /// category belongs to.
        /// </summary>
        public Guid BlogId { get; set; }

        /// <summary>
        /// Gets/sets the blog page this category belongs to.
        /// </summary>
        [JsonIgnore]
        public Page Blog { get; set; }
    }
}
