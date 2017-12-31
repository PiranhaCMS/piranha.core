/*
 * Copyright (c) 2017 Håkan Edling
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
using Piranha.Manager;
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// The page edit view model.
    /// </summary>
    public class PostListModel
    {
        public class PostListItem
        {
            public Guid Id { get; set; }
            public string TypeId { get; set; }
            public string TypeName { get; set; }
            public string Title { get; set; }
            public DateTime? Published { get; set; }
        }

        public IEnumerable<PostListItem> Posts { get; set; }
        public IEnumerable<PostType> PostTypes { get; set; }

        public PostListModel() {
            Posts = new List<PostListItem>();
            PostTypes = new List<PostType>();
        }

        public static PostListModel GetByBlogId(IApi api, Guid blogId) {
            var model = new PostListModel();

            model.Posts = api.Posts.GetAll(blogId)
                .Select(p => new PostListItem() {
                    Id = p.Id,
                    TypeId = p.TypeId,
                    Title = p.Title,
                    Published = p.Published
                }).ToList();
            model.PostTypes = api.PostTypes.GetAll();

            // Sort so we show unpublished drafts first
            model.Posts = model.Posts.Where(p => !p.Published.HasValue)
                .Concat(model.Posts.Where(p => p.Published.HasValue));

            foreach (var post in model.Posts) {
                var type = api.PostTypes.GetById(post.TypeId);
                if (type != null)
                    post.TypeName = type.Title;
            }
            return model;
        }
    }
}