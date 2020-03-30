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
using System.Threading.Tasks;

namespace Piranha.Models
{
    /// <summary>
    /// Generic page model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    [Serializable]
    public class GenericPage<T> : PageBase where T : GenericPage<T>
    {
        /// <summary>
        /// Gets if this is the startpage of the site.
        /// </summary>
        public bool IsStartPage {
            get { return !ParentId.HasValue && SortOrder == 0; }
        }

        /// <summary>
        /// Creates a new page model using the given page type id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The unique page type id</param>
        /// <returns>The new model</returns>
        public static Task<T> CreateAsync(IApi api, string typeId = null)
        {
            return api.Pages.CreateAsync<T>(typeId);
        }
    }
}
