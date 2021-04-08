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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Extensions;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
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

        public async Task<PageBase> CreatePage(string typeId)
        {
            var type = App.PageTypes.GetById(typeId);
            var modelType = Type.GetType(type.CLRType);

            var create = modelType.GetMethod("CreateAsync", BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy);
            var task = (Task)create.Invoke(null, new object[] { _api, null });

            await task.ConfigureAwait(false);

            var result = task.GetType().GetProperty("Result");
            return (PageBase)result.GetValue(task);
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
                var model = _transform.Transform(page, type, isDraft);

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
                var model = _transform.Transform(post, type, isDraft);

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

        public async Task<ContentModel> GetContentByIdAsync(Guid id, Guid? languageId = null)
        {
            // Ensure language id
            if (!languageId.HasValue)
            {
                languageId = (await _api.Languages.GetDefaultAsync().ConfigureAwait(false))?.Id;
            }

            var content = await _api.Content.GetByIdAsync<GenericContent>(id, languageId).ConfigureAwait(false);

            if (content != null)
            {
                // Get the content type & group
                var type = App.ContentTypes.GetById(content.TypeId);
                var group = App.ContentGroups.GetById(type.Group);

                // Initialize the content for manager use
                await _factory.InitManagerAsync(content, type).ConfigureAwait(false);

                // Transform the content
                var model = _transform.Transform(content, type, group);

                // Transform taxonomies if applicable
                if (model.Features.UseCategory || model.Features.UseTags)
                {
                    model.Taxonomies.Categories = (await _api.Content.GetAllCategoriesAsync(model.GroupId).ConfigureAwait(false))
                        .Select(p => p.Title).ToList();
                    model.Taxonomies.Tags = (await _api.Content.GetAllTagsAsync(model.GroupId).ConfigureAwait(false))
                        .Select(p => p.Title).ToList();
                }

                // Set selected language
                model.LanguageId = languageId;

                return model;
            }
            return null;
        }
    }
}