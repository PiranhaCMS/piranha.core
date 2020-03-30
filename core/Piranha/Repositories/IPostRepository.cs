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
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    public interface IPostRepository
    {
        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="blogId">The unique id</param>
        /// <param name="index">The optional page to fetch</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<Guid>> GetAll(Guid blogId, int? index = null, int? pageSize = null);

        /// <summary>
        /// Gets the available posts for the specified site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The posts</returns>
        Task<IEnumerable<Guid>> GetAllBySiteId(Guid siteId);

        /// <summary>
        /// Gets all available categories for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available categories</returns>
        Task<IEnumerable<Taxonomy>> GetAllCategories(Guid blogId);

        /// <summary>
        /// Gets all available tags for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available tags</returns>
        Task<IEnumerable<Taxonomy>> GetAllTags(Guid blogId);

        /// <summary>
        /// Gets the id of all posts that have a draft for
        /// the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts that have a draft</returns>
        Task<IEnumerable<Guid>> GetAllDrafts(Guid blogId);

        /// <summary>
        /// Gets the comments available for the post with the specified id.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        Task<IEnumerable<Comment>> GetAllComments(Guid? postId, bool onlyApproved,
            int page, int pageSize);

        /// <summary>
        /// Gets the pending comments available for the post with the specified id.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        Task<IEnumerable<Comment>> GetAllPendingComments(Guid? postId,
            int page, int pageSize);

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        Task<T> GetById<T>(Guid id) where T : PostBase;

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        Task<T> GetBySlug<T>(Guid blogId, string slug) where T : PostBase;

        /// <summary>
        /// Gets the draft for the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        Task<T> GetDraftById<T>(Guid id) where T : PostBase;

        /// <summary>
        /// Gets the number of available posts in the specified archive.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <returns>The number of posts</returns>
        Task<int> GetCount(Guid archiveId);

        /// <summary>
        /// Gets the category with the id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        Task<Taxonomy> GetCategoryById(Guid id);

        /// <summary>
        /// Gets the category with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The category</returns>
        Task<Taxonomy> GetCategoryBySlug(Guid blogId, string slug);

        /// <summary>
        /// Gets the tag with the id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        Task<Taxonomy> GetTagById(Guid id);

        /// <summary>
        /// Gets the tag with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The tag</returns>
        Task<Taxonomy> GetTagBySlug(Guid blogId, string slug);

        /// <summary>
        /// Gets the comment with the given id.
        /// </summary>
        /// <param name="id">The comment id</param>
        /// <returns>The model</returns>
        Task<Comment> GetCommentById(Guid id);

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        Task Save<T>(T model) where T : PostBase;

        /// <summary>
        /// Saves the given model as a draft revision.
        /// </summary>
        /// <param name="model">The post model</param>
        Task SaveDraft<T>(T model) where T : PostBase;

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="model">The comment model</param>
        Task SaveComment(Guid postId, Comment model);

        /// <summary>
        /// Creates a revision from the current version
        /// of the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="revisions">The maximum number of revisions that should be stored</param>
        Task CreateRevision(Guid id, int revisions);

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task Delete(Guid id);

        /// <summary>
        /// Deletes the current draft revision for the post
        /// with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteDraft(Guid id);

        /// <summary>
        /// Deletes the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        Task DeleteComment(Guid id);
    }
}
