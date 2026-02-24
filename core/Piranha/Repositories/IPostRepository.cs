/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Repositories;

public interface IPostRepository
{
    /// <summary>
    /// Gets the available posts for the specified blog.
    /// </summary>
    /// <param name="blogId">The unique id</param>
    /// <param name="index">The optional page to fetch</param>
    /// <param name="pageSize">The optional page size</param>
    /// <returns>The posts</returns>
    Task<IEnumerable<string>> GetAll(string blogId, int? index = null, int? pageSize = null);

    /// <summary>
    /// Gets the available posts for the specified site.
    /// </summary>
    /// <param name="siteId">The site id</param>
    /// <returns>The posts</returns>
    Task<IEnumerable<string>> GetAllBySiteId(string siteId);

    /// <summary>
    /// Gets all available categories for the specified blog.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <returns>The available categories</returns>
    Task<IEnumerable<Taxonomy>> GetAllCategories(string blogId);

    /// <summary>
    /// Gets all available tags for the specified blog.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <returns>The available tags</returns>
    Task<IEnumerable<Taxonomy>> GetAllTags(string blogId);

    /// <summary>
    /// Gets the id of all posts that have a draft for
    /// the specified blog.
    /// </summary>
    /// <param name="blogId">The unique blog id</param>
    /// <returns>The posts that have a draft</returns>
    Task<IEnumerable<string>> GetAllDrafts(string blogId);

    /// <summary>
    /// Gets the comments available for the post with the specified id.
    /// </summary>
    /// <param name="postId">The unique post id</param>
    /// <param name="onlyApproved">If only approved comments should be fetched</param>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The available comments</returns>
    Task<IEnumerable<Comment>> GetAllComments(string postId, bool onlyApproved,
        int page, int pageSize);

    /// <summary>
    /// Gets the pending comments available for the post with the specified id.
    /// </summary>
    /// <param name="postId">The unique post id</param>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>The available comments</returns>
    Task<IEnumerable<Comment>> GetAllPendingComments(string postId,
        int page, int pageSize);

    /// <summary>
    /// Gets the post model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <returns>The post model</returns>
    Task<T> GetById<T>(string id) where T : PostBase;

    /// <summary>
    /// Gets the post model with the specified slug.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="blogId">The blog id</param>
    /// <param name="slug">The unique slug</param>
    /// <returns>The post model</returns>
    Task<T> GetBySlug<T>(string blogId, string slug) where T : PostBase;

    /// <summary>
    /// Gets the draft for the post model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <returns>The draft, or null if no draft exists</returns>
    Task<T> GetDraftById<T>(string id) where T : PostBase;

    /// <summary>
    /// Gets the number of available posts in the specified archive.
    /// </summary>
    /// <param name="archiveId">The archive id</param>
    /// <returns>The number of posts</returns>
    Task<int> GetCount(string archiveId);

    /// <summary>
    /// Gets the category with the id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model</returns>
    Task<Taxonomy> GetCategoryById(string id);

    /// <summary>
    /// Gets the category with the given slug.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <param name="slug">The unique slug</param>
    /// <returns>The category</returns>
    Task<Taxonomy> GetCategoryBySlug(string blogId, string slug);

    /// <summary>
    /// Gets the tag with the id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model</returns>
    Task<Taxonomy> GetTagById(string id);

    /// <summary>
    /// Gets the tag with the given slug.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <param name="slug">The unique slug</param>
    /// <returns>The tag</returns>
    Task<Taxonomy> GetTagBySlug(string blogId, string slug);

    /// <summary>
    /// Gets the comment with the given id.
    /// </summary>
    /// <param name="id">The comment id</param>
    /// <returns>The model</returns>
    Task<Comment> GetCommentById(string id);

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
    Task SaveComment(string postId, Comment model);

    /// <summary>
    /// Creates a revision from the current version
    /// of the post with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="revisions">The maximum number of revisions that should be stored</param>
    Task CreateRevision(string id, int revisions);

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task Delete(string id);

    /// <summary>
    /// Deletes the current draft revision for the post
    /// with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteDraft(string id);

    /// <summary>
    /// Deletes the comment with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    Task DeleteComment(string id);
}
