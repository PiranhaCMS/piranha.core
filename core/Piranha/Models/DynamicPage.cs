/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Dynamic;

namespace Piranha.Models
{
    /// <summary>
    /// Dynamic page model.
    /// </summary>
    [Serializable]
    public class DynamicPage : GenericPage<DynamicPage>, IDynamicContent, IDynamicBlockContent
    {
        /// <summary>
        /// Gets/sets the regions.
        /// </summary>
        public dynamic Regions { get; set; } = new ExpandoObject();

        /// <summary>
        /// Gets/sets the available block sections.
        /// </summary>
        public IDictionary<string, IList<Extend.Block>> Sections { get; set; } =
            new Dictionary<string, IList<Extend.Block>>();
    }
}