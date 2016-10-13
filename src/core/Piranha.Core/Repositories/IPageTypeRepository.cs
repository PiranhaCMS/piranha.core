/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Repositories
{
    /// <summary>
    /// The client page type repository.
    /// </summary>
    public interface IPageTypeRepository
    {
        /// <summary>
        /// Gets the page type with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page type</returns>
        PageType GetById(string id);

        /// <summary>
        /// Gets all available page types.
        /// </summary>
        /// <returns>The page types</returns>
        IList<PageType> Get();

        /// <summary>
        /// Saves the given page type.
        /// </summary>
        /// <param name="pageType">The page type</param>
        void Save(PageType pageType);

        /// <summary>
        /// Deletes the given page type.
        /// </summary>
        /// <param name="pageType">The page type</param>
        void Delete(PageType pageType);

        /// <summary>
        /// Deletes the page type with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        void Delete(string id);
    }
}
