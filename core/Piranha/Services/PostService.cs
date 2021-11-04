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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Repositories;

namespace Piranha.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        private readonly IContentFactory _factory;
        private readonly ISiteService _siteService;
        private readonly IPageService _pageService;
        private readonly IParamService _paramService;
        private readonly IMediaService _mediaService;
        private readonly ICache _cache;
        private readonly ISearch _search;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="repo">The main repository</param>
        /// <param name="factory">The content factory</param>
        /// <param name="siteService">The site service</param>
        /// <param name="pageService">The page service</param>
        /// <param name="paramService">The param service</param>
        /// <param name="mediaService">The media service</param>
        /// <param name="cache">The optional model cache</param>
        /// <param name="search">The optional search service</param>
        public PostService(IPostRepository repo, IContentFactory factory, ISiteService siteService, IPageService pageService,
            IParamService paramService, IMediaService mediaService, ICache cache = null, ISearch search = null)
        {
            _repo = repo;
            _factory = factory;
            _siteService = siteService;
            _pageService = pageService;
            _paramService = paramService;
            _mediaService = mediaService;
            _search = search;

            if ((int)App.CacheLevel > 2)
            {
                _cache = cache;
            }
        }

        /// <summary>
        /// Creates and initializes a new post of the specified type.
        /// </summary>
        /// <returns>The created post</returns>
        public async Task<T> CreateAsync<T>(string typeId = null) where T : PostBase
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(T).Name;
            }

            var type = App.PostTypes.GetById(typeId);

            if (type != null)
            {
                var model = await _factory.CreateAsync<T>(type).ConfigureAwait(false);

                using (var config = new Config(_paramService))
                {
                    model.EnableComments = config.CommentsEnabledForPosts;
                    model.CloseCommentsAfterDays = config.CommentsCloseAfterDays;
                }
                return model;
            }
            return null;
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <param name="index">The optional page to fetch</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The posts</returns>
        public Task<IEnumerable<DynamicPost>> GetAllAsync(Guid blogId, int? index = null, int? pageSize = null)
        {
            return GetAllAsync<DynamicPost>(blogId, index, pageSize);
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="blogId">The unique id</param>
        /// <param name="index">The optional page to fetch</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The posts</returns>
        public async Task<IEnumerable<T>> GetAllAsync<T>(Guid blogId, int? index = null, int? pageSize = null) where T : PostBase
        {
            if (index.HasValue && !pageSize.HasValue)
            {
                // No page size provided, use default archive size
                using (var config = new Config(_paramService))
                {
                    pageSize = config.ArchivePageSize;
                }
            }

            var models = new List<T>();
            var posts = await _repo.GetAll(blogId, index, pageSize).ConfigureAwait(false);
            var pages = new List<PageInfo>();

            foreach (var postId in posts)
            {
                var post = await GetByIdAsync<T>(postId, pages).ConfigureAwait(false);

                if (post != null)
                {
                    models.Add(post);
                }
            }
            return models;
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        public Task<IEnumerable<DynamicPost>> GetAllBySiteIdAsync(Guid? siteId = null)
        {
            return GetAllBySiteIdAsync<DynamicPost>(siteId);
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        public async Task<IEnumerable<T>> GetAllBySiteIdAsync<T>(Guid? siteId = null) where T : PostBase
        {
            var models = new List<T>();
            var posts = await _repo.GetAllBySiteId((await EnsureSiteIdAsync(siteId).ConfigureAwait(false)).Value)
                .ConfigureAwait(false);
            var pages = new List<PageInfo>();

            foreach (var postId in posts)
            {
                var post = await GetByIdAsync<T>(postId, pages).ConfigureAwait(false);

                if (post != null)
                {
                    models.Add(post);
                }
            }
            return models;
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        public Task<IEnumerable<DynamicPost>> GetAllAsync(string slug, Guid? siteId = null)
        {
            return GetAllAsync<DynamicPost>(slug, siteId);
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        public async Task<IEnumerable<T>> GetAllAsync<T>(string slug, Guid? siteId = null) where T : PostBase
        {
            siteId = await EnsureSiteIdAsync(siteId).ConfigureAwait(false);
            var blogId = await _pageService.GetIdBySlugAsync(slug, siteId).ConfigureAwait(false);

            if (blogId.HasValue)
            {
                return await GetAllAsync<T>(blogId.Value).ConfigureAwait(false);
            }
            return new List<T>();
        }

        /// <summary>
        /// Gets all available categories for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available categories</returns>
        public Task<IEnumerable<Taxonomy>> GetAllCategoriesAsync(Guid blogId)
        {
            return _repo.GetAllCategories(blogId);
        }

        /// <summary>
        /// Gets all available tags for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <returns>The available tags</returns>
        public Task<IEnumerable<Taxonomy>> GetAllTagsAsync(Guid blogId)
        {
            return _repo.GetAllTags(blogId);
        }

        /// <summary>
        /// Gets the id of all posts that have a draft for
        /// the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts that have a draft</returns>
        public Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid blogId)
        {
            return _repo.GetAllDrafts(blogId);
        }

        /// <summary>
        /// Gets the comments available for the post with the specified id.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="page">The optional page number</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The available comments</returns>
        public Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? postId = null, bool onlyApproved = true,
            int? page = null, int? pageSize = null)
        {
            return GetAllCommentsAsync(postId, onlyApproved, false, page, pageSize);
        }

        /// <summary>
        /// Gets the pending comments available for the post with the specified id. If no post id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="page">The optional page number</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The available comments</returns>
        public Task<IEnumerable<Comment>> GetAllPendingCommentsAsync(Guid? postId = null,
            int? page = null, int? pageSize = null)
        {
            return GetAllCommentsAsync(postId, false, true, page, pageSize);
        }

        /// <summary>
        /// Gets the number of available posts in the specified archive.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <returns>The number of posts</returns>
        public Task<int> GetCountAsync(Guid archiveId)
        {
            return _repo.GetCount(archiveId);
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public Task<DynamicPost> GetByIdAsync(Guid id)
        {
            return GetByIdAsync<DynamicPost>(id);
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public Task<T> GetByIdAsync<T>(Guid id) where T : PostBase
        {
            return GetByIdAsync<T>(id, new List<PageInfo>());
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        public Task<DynamicPost> GetBySlugAsync(string blog, string slug, Guid? siteId = null)
        {
            return GetBySlugAsync<DynamicPost>(blog, slug, siteId);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        public async Task<T> GetBySlugAsync<T>(string blog, string slug, Guid? siteId = null) where T : PostBase
        {
            siteId = await EnsureSiteIdAsync(siteId).ConfigureAwait(false);

            var blogId = await _pageService.GetIdBySlugAsync(blog, siteId).ConfigureAwait(false);

            if (blogId.HasValue)
            {
                return await GetBySlugAsync<T>(blogId.Value, slug).ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blogId">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public Task<DynamicPost> GetBySlugAsync(Guid blogId, string slug)
        {
            return GetBySlugAsync<DynamicPost>(blogId, slug);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blogId">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public async Task<T> GetBySlugAsync<T>(Guid blogId, string slug) where T : PostBase
        {
            PostBase model = null;

            // Lets see if we can resolve the slug from cache
            var postId = _cache?.Get<Guid?>($"PostId_{blogId}_{slug}");

            if (postId.HasValue)
            {
                if (typeof(T) == typeof(PostInfo))
                {
                    model = _cache?.Get<PostInfo>($"PostInfo_{postId.ToString()}");
                }
                else if (!typeof(DynamicPost).IsAssignableFrom(typeof(T)))
                {
                    model = _cache?.Get<PostBase>(postId.ToString());
                }
            }

            if (model == null)
            {
                model = await _repo.GetBySlug<T>(blogId, slug).ConfigureAwait(false);

                if (model != null)
                {
                    var blog = await _pageService.GetByIdAsync<PageInfo>(model.BlogId).ConfigureAwait(false);

                    await OnLoadAsync(model, blog).ConfigureAwait(false);
                }
            }

            if (model != null && model is T)
            {
                return (T)model;
            }
            return null;
        }

        /// <summary>
        /// Gets the draft for the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        public Task<DynamicPost> GetDraftByIdAsync(Guid id)
        {
            return GetDraftByIdAsync<DynamicPost>(id);
        }

        /// <summary>
        /// Gets the draft for the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        public async Task<T> GetDraftByIdAsync<T>(Guid id) where T : PostBase
        {
            var draft = await _repo.GetDraftById<T>(id).ConfigureAwait(false);

            if (draft != null)
            {
                var blog = await _pageService.GetByIdAsync<PageInfo>(draft.BlogId).ConfigureAwait(false);

                await OnLoadAsync(draft, blog, true).ConfigureAwait(false);
            }
            return draft;
        }

        /// <summary>
        /// Gets the category with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public async Task<Taxonomy> GetCategoryBySlugAsync(Guid blogId, string slug)
        {
            var id = _cache?.Get<Guid?>($"Category_{blogId}_{slug}");
            Taxonomy model = null;

            if (id.HasValue)
            {
                model = _cache.Get<Taxonomy>(id.Value.ToString());
            }

            if (model == null)
            {
                model = await _repo.GetCategoryBySlug(blogId, slug).ConfigureAwait(false);

                if (model != null && _cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                    _cache.Set($"Category_{blogId}_{slug}", model.Id);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the category with the id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        public async Task<Taxonomy> GetCategoryByIdAsync(Guid id)
        {
            Taxonomy model = _cache?.Get<Taxonomy>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetCategoryById(id).ConfigureAwait(false);

                if (model != null && _cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the tag with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public async Task<Taxonomy> GetTagBySlugAsync(Guid blogId, string slug)
        {
            var id = _cache?.Get<Guid?>($"Tag_{blogId}_{slug}");
            Taxonomy model = null;

            if (id.HasValue)
            {
                model = _cache.Get<Taxonomy>(id.Value.ToString());
            }

            if (model == null)
            {
                model = await _repo.GetTagBySlug(blogId, slug).ConfigureAwait(false);

                if (model != null && _cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                    _cache.Set($"Tag_{blogId}_{slug}", model.Id);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the category with the id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        public async Task<Taxonomy> GetTagByIdAsync(Guid id)
        {
            Taxonomy model = _cache?.Get<Taxonomy>(id.ToString());

            if (model == null)
            {
                model = await _repo.GetTagById(id).ConfigureAwait(false);

                if (model != null && _cache != null)
                {
                    _cache.Set(model.Id.ToString(), model);
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the comment with the given id.
        /// </summary>
        /// <param name="id">The comment id</param>
        /// <returns>The model</returns>
        public Task<Comment> GetCommentByIdAsync(Guid id)
        {
            return _repo.GetCommentById(id);
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        public Task SaveAsync<T>(T model) where T : PostBase
        {
            return SaveAsync(model, false);
        }

        /// <summary>
        /// Saves the given post model as a draft
        /// </summary>
        /// <param name="model">The post model</param>
        public Task SaveDraftAsync<T>(T model) where T : PostBase
        {
            return SaveAsync(model, true);
        }

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="model">The comment model</param>
        /// <param name="postId">The unique post id</param>
        public Task SaveCommentAsync(Guid postId, Comment model)
        {
            return SaveCommentAsync(postId, model, false);
        }

        /// <summary>
        /// Saves the comment and verifies if should be approved or not.
        /// </summary>
        /// <param name="model">The comment model</param>
        /// <param name="postId">The unique post id</param>
        public Task SaveCommentAndVerifyAsync(Guid postId, Comment model)
        {
            return SaveCommentAsync(postId, model, true);
        }

        /// <summary>
        /// Gets the comments available for the post with the specified id.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="onlyPending">If only pending comments should be fetched</param>
        /// <param name="page">The optional page number</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The available comments</returns>
        private async Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? postId = null, bool onlyApproved = true,
            bool onlyPending = false, int? page = null, int? pageSize = null)
        {
            // Ensure page number
            if (!page.HasValue)
            {
                page = 0;
            }

            // Ensure page size
            if (!pageSize.HasValue)
            {
                using (var config = new Config(_paramService))
                {
                    pageSize = config.CommentsPageSize;
                }
            }

            // Get the comments
            IEnumerable<Comment> comments = null;

            if (onlyPending)
            {
                comments = await _repo.GetAllPendingComments(postId, page.Value, pageSize.Value).ConfigureAwait(false);
            }
            else
            {
                comments = await _repo.GetAllComments(postId, onlyApproved, page.Value, pageSize.Value).ConfigureAwait(false);
            }

            // Execute hook
            foreach (var comment in comments)
            {
                App.Hooks.OnLoad<Comment>(comment);
            }
            return comments;
        }

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="model">The comment model</param>
        /// <param name="postId">The unique post id</param>
        /// <param name="verify">If default moderation settings should be applied</param>
        private async Task SaveCommentAsync(Guid postId, Comment model, bool verify)
        {
            // Make sure we have a post
            var post = await GetByIdAsync<PostInfo>(postId).ConfigureAwait(false);

            if (post != null)
            {
                // Ensure id
                if (model.Id == Guid.Empty)
                {
                    model.Id = Guid.NewGuid();
                }

                // Ensure created date
                if (model.Created == DateTime.MinValue)
                {
                    model.Created = DateTime.Now;
                }

                // Ensure content id
                if (model.ContentId == Guid.Empty)
                {
                    model.ContentId = postId;
                }

                // Validate model
                var context = new ValidationContext(model);
                Validator.ValidateObject(model, context, true);

                // Set approved according to config if we should verify
                if (verify)
                {
                    using (var config = new Config(_paramService))
                    {
                        model.IsApproved = config.CommentsApprove;
                    }
                    App.Hooks.OnValidate<Comment>(model);
                }

                // Call hooks & save
                App.Hooks.OnBeforeSave<Comment>(model);
                await _repo.SaveComment(postId, model).ConfigureAwait(false);
                App.Hooks.OnAfterSave<Comment>(model);

                // Invalidate parent post from cache
                RemoveFromCache(post);
            }
            else
            {
                throw new ArgumentException($"Could not find post with id { postId.ToString() }");
            }
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        /// <param name="isDraft">If the model should be saved as a draft</param>
        private async Task SaveAsync<T>(T model, bool isDraft) where T : PostBase
        {
            // Ensure id
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            // Validate model
            var context = new ValidationContext(model);
            Validator.ValidateObject(model, context, true);

            // Ensure title since this field isn't required in
            // the Content base class
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ValidationException("The Title field is required");
            }

            // Ensure type id since this field isn't required in
            // the Content base class
            if (string.IsNullOrWhiteSpace(model.TypeId))
            {
                throw new ValidationException("The TypeId field is required");
            }

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                model.Slug = Utils.GenerateSlug(model.Title, false);
            }
            else
            {
                model.Slug = Utils.GenerateSlug(model.Slug, false);
            }

            // Ensure slug is not null or empty
            // after removing unwanted characters
            if (string.IsNullOrWhiteSpace(model.Slug))
            {
                throw new ValidationException("The generated slug is empty as the title only contains special characters, please specify a slug to save the post.");
            }

            // Ensure that the slug is unique
            var duplicate = await GetBySlugAsync(model.BlogId, model.Slug);
            if (duplicate != null && duplicate.Id != model.Id)
            {
                throw new ValidationException("The specified slug already exists, please create a unique slug");
            }

            // Ensure category
            if (model.Category == null || (string.IsNullOrWhiteSpace(model.Category.Title) && string.IsNullOrWhiteSpace(model.Category.Slug)))
            {
                throw new ValidationException("The Category field is required");
            }

            // Call hooks & save
            App.Hooks.OnBeforeSave<PostBase>(model);

            // Handle revisions and save
            var current = await _repo.GetById<PostInfo>(model.Id).ConfigureAwait(false);

            if ((IsPublished(current) || IsScheduled(current)) && isDraft)
            {
                // We're saving a draft since we have a previously
                // published version of the post
                await _repo.SaveDraft(model).ConfigureAwait(false);
            }
            else
            {
                if (current == null && isDraft)
                {
                    // If we're saving a draft as a normal post instance, make
                    // sure we remove the published date as this sould effectively
                    // publish the post.
                    model.Published = null;
                }
                else if (current != null && !isDraft)
                {
                    using (var config = new Config(_paramService))
                    {
                        // Save current as a revision before saving the model
                        // and if a draft revision exists, remove it.
                        await _repo.DeleteDraft(model.Id).ConfigureAwait(false);
                        await _repo.CreateRevision(model.Id, config.PostRevisions).ConfigureAwait(false);
                    }
                }

                // Save the main post
                await _repo.Save(model).ConfigureAwait(false);
            }

            App.Hooks.OnAfterSave<PostBase>(model);

            // Update search document
            if (!isDraft && _search != null)
            {
                await _search.SavePostAsync(model);
            }

            // Remove the post from cache
            RemoveFromCache(model);

            if (!isDraft && _cache != null)
            {
                // Clear all categories from cache in case some
                // unused where deleted.
                var categories = await _repo.GetAllCategories(model.BlogId).ConfigureAwait(false);
                foreach (var category in categories)
                {
                    _cache.Remove(category.Id.ToString());
                    _cache.Remove($"Category_{model.BlogId}_{category.Slug}");
                }

                // Clear all tags from cache in case some
                // unused where deleted.
                var tags = await _repo.GetAllTags(model.BlogId).ConfigureAwait(false);
                foreach (var tag in tags)
                {
                    _cache.Remove(tag.Id.ToString());
                    _cache.Remove($"Tag_{model.BlogId}_{tag.Slug}");
                }
            }
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteAsync(Guid id)
        {
            var model = await GetByIdAsync<PostInfo>(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task DeleteAsync<T>(T model) where T : PostBase
        {
            // Call hooks & save
            App.Hooks.OnBeforeDelete<PostBase>(model);
            await _repo.Delete(model.Id).ConfigureAwait(false);
            App.Hooks.OnAfterDelete<PostBase>(model);

            // Delete search document
            if (_search != null)
            {
                await _search.DeletePostAsync(model);
            }

            // Remove from cache & invalidate sitemap
            RemoveFromCache(model);
        }

        /// <summary>
        /// Deletes the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteCommentAsync(Guid id)
        {
            var model = await GetCommentByIdAsync(id).ConfigureAwait(false);

            if (model != null)
            {
                await DeleteCommentAsync(model).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the given comment.
        /// </summary>
        /// <param name="model">The comment</param>
        public async Task DeleteCommentAsync(Comment model)
        {
            var post = await GetByIdAsync<PostInfo>(model.ContentId).ConfigureAwait(false);

            if (post != null)
            {
                // Call hooks & delete
                App.Hooks.OnBeforeDelete<Comment>(model);
                await _repo.DeleteComment(model.Id);
                App.Hooks.OnAfterDelete<Comment>(model);

                // Remove parent post from cache
                RemoveFromCache(post);
            }
            else
            {
                throw new ArgumentException($"Could not find post with id { model.ContentId.ToString() }");
            }
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <param name="blogPages">The blog pages</param>
        /// <returns>The post model</returns>
        private async Task<T> GetByIdAsync<T>(Guid id, IList<PageInfo> blogPages) where T : PostBase
        {
            PostBase model = null;

            if (typeof(T) == typeof(PostInfo))
            {
                model = _cache?.Get<PostInfo>($"PostInfo_{id.ToString()}");
            }
            else if (!typeof(DynamicPost).IsAssignableFrom(typeof(T)))
            {
                model = _cache?.Get<PostBase>(id.ToString());

                if (model != null)
                {
                    await _factory.InitAsync(model, App.PostTypes.GetById(model.TypeId));
                }
            }

            if (model == null)
            {
                model = await _repo.GetById<T>(id).ConfigureAwait(false);

                if (model != null)
                {
                    var blog = blogPages.FirstOrDefault(p => p.Id == model.BlogId);

                    if (blog == null)
                    {
                        blog = await _pageService.GetByIdAsync<PageInfo>(model.BlogId).ConfigureAwait(false);
                        blogPages.Add(blog);
                    }

                    await OnLoadAsync(model, blog).ConfigureAwait(false);
                }
            }

            if (model != null && model is T)
            {
                return (T)model;
            }
            return null;
        }

        /// <summary>
        /// Checks if the given site id is empty, and if so
        /// gets the site id of the default site.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The site id</returns>
        private async Task<Guid?> EnsureSiteIdAsync(Guid? siteId)
        {
            if (!siteId.HasValue)
            {
                var site = await _siteService.GetDefaultAsync().ConfigureAwait(false);

                if (site != null)
                {
                    return site.Id;
                }
            }
            return siteId;
        }

        /// <summary>
        /// Processes the model on load.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="blog">The blog page the post belongs to</param>
        /// <param name="isDraft">If this is a draft</param>
        private async Task OnLoadAsync(PostBase model, PageInfo blog, bool isDraft = false)
        {
            if (model != null)
            {
                // Format permalink
                model.Permalink = $"/{blog.Slug}/{model.Slug}";

                // Initialize model
                if (model is IDynamicContent dynamicModel)
                {
                    await _factory.InitDynamicAsync(dynamicModel, App.PostTypes.GetById(model.TypeId));
                }
                else
                {
                    await _factory.InitAsync(model, App.PostTypes.GetById(model.TypeId));
                }

                // Initialize primary image
                if (model.PrimaryImage == null)
                {
                    model.PrimaryImage = new Extend.Fields.ImageField();
                }

                if (model.PrimaryImage.Id.HasValue)
                {
                    await _factory.InitFieldAsync(model.PrimaryImage).ConfigureAwait(false);
                }

                // Initialize og image
                if (model.OgImage == null)
                {
                    model.OgImage = new Extend.Fields.ImageField();
                }

                if (model.OgImage.Id.HasValue)
                {
                    await _factory.InitFieldAsync(model.OgImage).ConfigureAwait(false);
                }

                App.Hooks.OnLoad(model);

                // Never cache drafts, dynamic or simple instances
                if (!isDraft && _cache != null && !(model is DynamicPost))
                {
                    if (model is PostInfo)
                    {
                        _cache.Set($"PostInfo_{model.Id.ToString()}", model);
                    }
                    else
                    {
                        _cache.Set(model.Id.ToString(), model);
                    }
                    _cache.Set($"PostId_{model.BlogId}_{model.Slug}", model.Id);
                }
            }
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="post">The post</param>
        private void RemoveFromCache(PostBase post)
        {
            if (_cache != null)
            {
                _cache.Remove(post.Id.ToString());
                _cache.Remove($"PostId_{post.BlogId}_{post.Slug}");
                _cache.Remove($"PostInfo_{post.Id.ToString()}");
            }
        }

        /// <summary>
        /// Checks if the given post is published
        /// </summary>
        /// <param name="model">The posts model</param>
        /// <returns>If the post is published</returns>
        private bool IsPublished (PostBase model)
        {
            return model != null && model.Published.HasValue && model.Published.Value <= DateTime.Now;
        }

        /// <summary>
        /// Checks if the given post is scheduled
        /// </summary>
        /// <param name="model">The post model</param>
        /// <returns>If the post is scheduled</returns>
        private bool IsScheduled(PostBase model)
        {
            return model != null && model.Published.HasValue && model.Published.Value > DateTime.Now;
        }

    }
}
