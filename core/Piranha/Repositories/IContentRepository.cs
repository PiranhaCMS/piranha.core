/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IContentRepository
    {
        /// <summary>
        /// Gets all of the available content for the optional
        /// group id.
        /// </summary>
        /// <param name="groupId">The optional group id</param>
        /// <returns>The available content</returns>
        Task<IEnumerable<Guid>> GetAll(string groupId = null);

        /// <summary>
        /// Gets the content model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <param name="languageId">The selected language id</param>
        /// <returns>The content model</returns>
        Task<T> GetById<T>(Guid id, Guid languageId) where T : GenericContent;

        /// <summary>
        /// Gets all available categories for the specified group.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The available categories</returns>
        Task<IEnumerable<Taxonomy>> GetAllCategories(string groupId);

        /// <summary>
        /// Gets all available tags for the specified groupd.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The available tags</returns>
        Task<IEnumerable<Taxonomy>> GetAllTags(string groupId);

        /// <summary>
        /// Gets the current translation status for the content model
        /// with the given id.
        /// </summary>
        /// <param name="contentId">The unique content id</param>
        /// <returns>The translation status</returns>
        Task<TranslationStatus> GetTranslationStatusById(Guid contentId);

        /// <summary>
        /// Gets the translation summary for the content group with
        /// the given id.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The translation summary</returns>
        Task<TranslationSummary> GetTranslationStatusByGroup(string groupId);

        /// <summary>
        /// Saves the given content model
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="languageId">The selected language id</param>
        Task Save<T>(T model, Guid languageId) where T : GenericContent;

        /// <summary>
        /// Deletes the content model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);
    }
}
