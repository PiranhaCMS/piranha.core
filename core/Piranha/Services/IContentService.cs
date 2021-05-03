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

namespace Piranha.Services
{
    public interface IContentService
    {
        /// <summary>
        /// Creates and initializes a new content model.
        /// </summary>
        /// <param name="typeId">The content type id</param>
        /// <returns>The created page</returns>
        Task<T> CreateAsync<T>(string typeId) where T : GenericContent;

        /// <summary>
        /// Gets all of the available content for the optional
        /// group id.
        /// </summary>
        /// <param name="groupId">The optional group id</param>
        /// <returns>The available content</returns>
        Task<IEnumerable<DynamicContent>> GetAllAsync(string groupId = null);

        /// <summary>
        /// Gets all of the available content for the optional
        /// group id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="groupId">The optional group id</param>
        /// <returns>The available content</returns>
        Task<IEnumerable<T>> GetAllAsync<T>(string groupId = null) where T : GenericContent;

        /// <summary>
        /// Gets the content model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="languageId">The optional language id</param>
        /// <returns>The content model</returns>
        Task<DynamicContent> GetByIdAsync(Guid id, Guid? languageId = null);

        /// <summary>
        /// Gets the content model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <param name="languageId">The optional language id</param>
        /// <returns>The content model</returns>
        Task<T> GetByIdAsync<T>(Guid id, Guid? languageId = null) where T : GenericContent;

        /// <summary>
        /// Gets all available categories for the specified group.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The available categories</returns>
        Task<IEnumerable<Taxonomy>> GetAllCategoriesAsync(string groupId);

        /// <summary>
        /// Gets all available tags for the specified groupd.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The available tags</returns>
        Task<IEnumerable<Taxonomy>> GetAllTagsAsync(string groupId);

        /// <summary>
        /// Gets the current translation status for the content model
        /// with the given id.
        /// </summary>
        /// <param name="contentId">The unique content id</param>
        /// <returns>The translation status</returns>
        Task<TranslationStatus> GetTranslationStatusByIdAsync(Guid contentId);

        /// <summary>
        /// Gets the translation summary for the content group with
        /// the given id.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The translation summary</returns>
        Task<TranslationSummary> GetTranslationStatusByGroupAsync(string groupId);

        /// <summary>
        /// Saves the given content model
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="languageId">The optional language id</param>
        Task SaveAsync<T>(T model, Guid? languageId = null) where T : GenericContent;

        /// <summary>
        /// Deletes the content model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given content model.
        /// </summary>
        /// <param name="model">The content model</param>
        Task DeleteAsync<T>(T model) where T : GenericContent;
    }
}
