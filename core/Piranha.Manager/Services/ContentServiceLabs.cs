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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Manager.Models;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public sealed class ContentServiceLabs
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;
        private readonly TransformationService _transform;
        private readonly Config _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content factory</param>
        /// <param name="transform">The transformation service</param>
        /// <param name="config">The piranha configuration</param>
        public ContentServiceLabs(IApi api, IContentFactory factory, TransformationService transform, Config config)
        {
            _api = api;
            _factory = factory;
            _transform = transform;
            _config = config;
        }

        /// <summary>
        /// Creates a new page of the given type
        /// </summary>
        /// <param name="typeId">The page type</param>
        /// <returns>The new page</returns>
        public Task<PageBase> CreatePage(string typeId)
        {
            var type = App.PageTypes.GetById(typeId);
            var modelType = Type.GetType(type.CLRType);

            return CreateModel<PageBase>(modelType);
        }

        /// <summary>
        /// Creates a new post of the given type
        /// </summary>
        /// <param name="typeId">The post type</param>
        /// <returns>The new post</returns>
        public Task<PostBase> CreatePost(string typeId)
        {
            var type = App.PostTypes.GetById(typeId);
            var modelType = Type.GetType(type.CLRType);

            return CreateModel<PostBase>(modelType);
        }

        /// <summary>
        /// Creates a new content of the given type
        /// </summary>
        /// <param name="typeId">The post type</param>
        /// <returns>The new post</returns>
        public Task<GenericContent> CreateContent(string typeId)
        {
            var type = App.ContentTypes.GetById(typeId);
            var modelType = Type.GetType(type.CLRType);

            return CreateModel<GenericContent>(modelType);
        }

        public async Task<PageBase> ToPage(ContentModel model)
        {
            var page = await CreatePage(model.TypeId).ConfigureAwait(false);
            return _transform.ToPage(model, page);
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        public async Task<ContentModel> GetPageByIdAsync(Guid id)
        {
            var isDraft = true;

            // Get the page from the api
            var page = await _api.Pages.GetDraftByIdAsync<PageBase>(id).ConfigureAwait(false);
            if (page == null)
            {
                page = await _api.Pages.GetByIdAsync<PageBase>(id).ConfigureAwait(false);
                isDraft = false;
            }

            if (page != null)
            {
                // Get the site
                var site = await _api.Sites.GetByIdAsync(page.SiteId).ConfigureAwait(false);

                // Get the page type
                var type = App.PageTypes.GetById(page.TypeId);

                // Initialize the page for manager use
                await _factory.InitManagerAsync(page, type).ConfigureAwait(false);

                // Transform the page
                var model = _transform.ToModel(page, type, isDraft);

                // Get the number of pending comments
                model.Comments.PendingCommentCount = (await _api.Posts.GetAllPendingCommentsAsync(id).ConfigureAwait(false))
                    .Count();

                // Set selected language
                model.LanguageId = site.LanguageId;

                return model;
            }
            return null;
        }

        /// <summary>
        /// Saves the given page.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>An awaitable task</returns>
        public async Task<ContentModel> SavePageAsync(ContentModel model)
        {
            // Create a new page of the correct type
            var page = await CreatePage(model.TypeId);

            // Transform the page
            _transform.ToPage(model, page);

            // Save the page
            await _api.Pages.SaveAsync(page);

            // Return the updated page
            return await GetPageByIdAsync(model.Id);
        }

        /// <summary>
        /// /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        public async Task<ContentModel> GetPostByIdAsync(Guid id)
        {
            var isDraft = true;

            // Get the post from the api
            var post = await _api.Posts.GetDraftByIdAsync<PostBase>(id).ConfigureAwait(false);
            if (post == null)
            {
                post = await _api.Posts.GetByIdAsync<PostBase>(id).ConfigureAwait(false);
                isDraft = false;
            }

            if (post != null)
            {
                // Get the archive page
                var page = await _api.Pages.GetByIdAsync<PageInfo>(post.BlogId).ConfigureAwait(false);

                // Get the site
                var site = await _api.Sites.GetByIdAsync(page.SiteId).ConfigureAwait(false);

                // Get the post type
                var type = App.PostTypes.GetById(post.TypeId);

                // Initialize the post for manager use
                await _factory.InitManagerAsync(post, type);

                // Transform the post
                var model = _transform.ToModel(post, type, isDraft);

                // Get the number of pending comments
                model.Comments.PendingCommentCount = (await _api.Posts.GetAllPendingCommentsAsync(id).ConfigureAwait(false))
                    .Count();

                // Set the available taxonomies
                model.Taxonomies.Categories = (await _api.Posts.GetAllCategoriesAsync(post.BlogId).ConfigureAwait(false))
                    .Select(p => p.Title).ToList();
                model.Taxonomies.Tags = (await _api.Posts.GetAllTagsAsync(post.BlogId).ConfigureAwait(false))
                    .Select(p => p.Title).ToList();

                // Set selected language
                model.LanguageId = site.LanguageId;

                return model;
            }
            return null;
        }

        /// <summary>
        /// Saves the given post.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>An awaitable task</returns>
        public async Task<ContentModel> SavePostAsync(ContentModel model)
        {
            // Create a new post of the correct type
            var post = await CreatePost(model.TypeId);

            // Transform the post
            _transform.ToPost(model, post);

            // Save the post
            await _api.Posts.SaveAsync(post);

            // Return the updated post
            return await GetPostByIdAsync(model.Id);
        }

        /// <summary>
        /// Gets the content with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="languageId">The optional language id</param>
        /// <returns>The content model</returns>
        public async Task<ContentModel> GetContentByIdAsync(Guid id, Guid? languageId = null)
        {
            Language language = null;

            // Ensure language
            if (languageId.HasValue)
            {
                language = await _api.Languages.GetByIdAsync(languageId.Value).ConfigureAwait(false);
            }

            if (language == null)
            {
                language = await _api.Languages.GetDefaultAsync().ConfigureAwait(false);
            }

            var content = await _api.Content.GetByIdAsync<GenericContent>(id, language.Id).ConfigureAwait(false);

            if (content != null)
            {
                // Get the content type & group
                var type = App.ContentTypes.GetById(content.TypeId);
                var group = App.ContentGroups.GetById(type.Group);

                // Initialize the content for manager use
                await _factory.InitManagerAsync(content, type).ConfigureAwait(false);

                // Transform the content
                var model = _transform.ToModel(content, type, group);

                // Transform taxonomies if applicable
                if (model.Features.UseCategory || model.Features.UseTags)
                {
                    model.Taxonomies.Categories = (await _api.Content.GetAllCategoriesAsync(model.GroupId).ConfigureAwait(false))
                        .Select(p => p.Title).ToList();
                    model.Taxonomies.Tags = (await _api.Content.GetAllTagsAsync(model.GroupId).ConfigureAwait(false))
                        .Select(p => p.Title).ToList();
                }

                // Set selected language
                model.LanguageId = language.Id;
                model.LanguageTitle = language.Title;

                // Get the available languages
                model.Languages = await _api.Languages.GetAllAsync().ConfigureAwait(false);

                return model;
            }
            return null;
        }

        /// <summary>
        /// Saves the given content.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <returns>An awaitable task</returns>
        public async Task<ContentModel> SaveContentAsync(ContentModel model)
        {
            // Create a new content of the correct type
            var content = await CreateContent(model.TypeId);

            // Transform the content
            _transform.ToContent(model, content);

            // Save the content
            await _api.Content.SaveAsync(content, model.LanguageId);

            // Return the updated content
            return await GetContentByIdAsync(model.Id);
        }

        private async Task<T> CreateModel<T>(Type modelType)
        {
            var create = modelType.GetMethod("CreateAsync", BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy);
            var task = (Task)create.Invoke(null, new object[] { _api, null });

            await task.ConfigureAwait(false);

            var result = task.GetType().GetProperty("Result");
            return (T)result.GetValue(task);
        }
    }
}