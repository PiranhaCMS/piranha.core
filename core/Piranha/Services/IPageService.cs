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
    public interface IPageService
    {
        /// <summary>
        /// Creates and initializes a new page of the specified type.
        /// </summary>
        /// <returns>The created page</returns>
        Task<T> CreateAsync<T>(string typeId = null) where T : Models.PageBase;

        /// <summary>
        /// Creates and initializes a copy of the given page.
        /// </summary>
        /// <param name="originalPage">The orginal page</param>
        /// <returns>The created copy</returns>
        Task<T> CopyAsync<T>(T originalPage) where T : Models.PageBase;

        /// <summary>
        /// Detaches a copy and initializes it as a standalone page
        /// </summary>
        /// <returns>The standalone page</returns>
        Task DetachAsync<T>(T model) where T : Models.PageBase;

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        Task<IEnumerable<DynamicPage>> GetAllAsync(Guid? siteId = null);

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        Task<IEnumerable<T>> GetAllAsync<T>(Guid? siteId = null) where T : PageBase;

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        Task<IEnumerable<DynamicPage>> GetAllBlogsAsync(Guid? siteId = null);

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The pages</returns>
        Task<IEnumerable<T>> GetAllBlogsAsync<T>(Guid? siteId = null) where T : Models.PageBase;

        /// <summary>
        /// Gets the id of all pages that have a draft for
        /// the specified site.
        /// </summary>
        /// <param name="siteId">The unique site id</param>
        /// <returns>The pages that have a draft</returns>
        Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid? siteId = null);

        /// <summary>
        /// Gets the comments available for the page with the specified id. If no page id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="page">The optional page number</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The available comments</returns>
        Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? pageId = null, bool onlyApproved = true,
            int? page = null, int? pageSize = null);

        /// <summary>
        /// Gets the pending comments available for the page with the specified id. If no page id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="page">The optional page number</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The available comments</returns>
        Task<IEnumerable<Comment>> GetAllPendingCommentsAsync(Guid? pageId = null,
            int? page = null, int? pageSize = null);

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        Task<DynamicPage> GetStartpageAsync(Guid? siteId = null);

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        Task<T> GetStartpageAsync<T>(Guid? siteId = null) where T : Models.PageBase;

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        Task<DynamicPage> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        Task<T> GetByIdAsync<T>(Guid id) where T : PageBase;

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        Task<DynamicPage> GetBySlugAsync(string slug, Guid? siteId = null);

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The page model</returns>
        Task<T> GetBySlugAsync<T>(string slug, Guid? siteId = null) where T : Models.PageBase;

        /// <summary>
        /// Gets the id for the page with the given slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional page id</param>
        /// <returns>The id</returns>
        Task<Guid?> GetIdBySlugAsync(string slug, Guid? siteId = null);

        /// <summary>
        /// Gets the draft for the page model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        Task<DynamicPage> GetDraftByIdAsync(Guid id);

        /// <summary>
        /// Gets the draft for the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        Task<T> GetDraftByIdAsync<T>(Guid id) where T : PageBase;

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        Task MoveAsync<T>(T model, Guid? parentId, int sortOrder) where T : Models.PageBase;

        /// <summary>
        /// Gets the comment with the given id.
        /// </summary>
        /// <param name="id">The comment id</param>
        /// <returns>The model</returns>
        Task<Comment> GetCommentByIdAsync(Guid id);

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        Task SaveAsync<T>(T model) where T : PageBase;

        /// <summary>
        /// Saves the given page model as a draft
        /// </summary>
        /// <param name="model">The page model</param>
        Task SaveDraftAsync<T>(T model) where T : PageBase;

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="model">The comment model</param>
        Task SaveCommentAsync(Guid pageId, Comment model);

        /// <summary>
        /// Saves the comment and verifies if should be approved or not.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="model">The comment model</param>
        Task SaveCommentAndVerifyAsync(Guid pageId, Comment model);

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        Task DeleteAsync<T>(T model) where T : Models.PageBase;

        /// <summary>
        /// Deletes the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteCommentAsync(Guid id);

        /// <summary>
        /// Deletes the given comment.
        /// </summary>
        /// <param name="model">The comment</param>
        Task DeleteCommentAsync(Comment model);
    }
}
