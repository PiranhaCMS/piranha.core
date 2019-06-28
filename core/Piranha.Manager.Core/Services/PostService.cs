/*
 * Copyright (c) 2019 Håkan Edling
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
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public class PostService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        public PostService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        public async Task<PostListModel> GetList(Guid archiveId)
        {
            var model = new PostListModel
            {
                PostTypes = App.PostTypes.Select(t => new PostListModel.PostTypeItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    AddUrl = "manager/post/add/"
                }).ToList()
            };

            // Get posts
            model.Posts = (await _api.Posts.GetAllAsync<PostInfo>(archiveId))
                .Select(p => new PostListModel.PostItem
                {
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    TypeName = model.PostTypes.First(t => t.Id == p.TypeId).Title,
                    Category = p.Category.Title,
                    Published = p.Published.HasValue ? p.Published.Value.ToString("yyyy-MM-dd HH:mm") : null,
                    Status = GetState(p, false),
                    isScheduled = p.Published.HasValue && p.Published.Value > DateTime.Now,
                    EditUrl = "manager/post/edit/"
                }).ToList();

            // Get categories
            model.Categories = (await _api.Posts.GetAllCategoriesAsync(archiveId))
                .Select(c => new PostListModel.CategoryItem
                {
                    Id = c.Id.ToString(),
                    Title = c.Title
                }).ToList();

            return model;
        }

        private string GetState(PostInfo post, bool isDraft)
        {
            if (post.Created != DateTime.MinValue)
            {
                if (post.Published.HasValue)
                {
                    if (isDraft)
                    {
                        return ContentState.Draft;
                    }
                    return ContentState.Published;
                }
                else
                {
                    return ContentState.Unpublished;
                }
            }
            return ContentState.New;
        }
    }
}