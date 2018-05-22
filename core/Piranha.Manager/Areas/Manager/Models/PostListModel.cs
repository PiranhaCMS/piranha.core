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
            public Guid CategoryId { get; set; }
            public string Category { get; set; }
            public DateTime? Published { get; set; }
        }

        public IEnumerable<PostListItem> Posts { get; set; }
        public IEnumerable<PostType> PostTypes { get; set; }
        public IEnumerable<PostType> CurrentPostTypes { get; set; }
        public IEnumerable<Taxonomy> CurrentCategories { get; set; }

        public PostListModel() {
            Posts = new List<PostListItem>();
            PostTypes = new List<PostType>();
            CurrentPostTypes = new List<PostType>();
            CurrentCategories = new List<Taxonomy>();
        }

        public static PostListModel GetByBlogId(IApi api, Guid blogId) {
            var model = new PostListModel();

            model.Posts = api.Posts.GetAll<PostInfo>(blogId)
                .Select(p => new PostListItem() {
                    Id = p.Id,
                    TypeId = p.TypeId,
                    Title = p.Title,
                    CategoryId = p.Category.Id,
                    Category = p.Category.Title,
                    Published = p.Published
                }).ToList();
            model.PostTypes = api.PostTypes.GetAll();

            // Filter out the currently used post types
            var typesId = model.Posts.Select(p => p.TypeId).Distinct();
            model.CurrentPostTypes = model.PostTypes.Where(t => typesId.Contains(t.Id));

            // Get the currently used categories
            var categoriesId = model.Posts.Select(p => p.CategoryId).Distinct();
            model.CurrentCategories = api.Categories.GetAll(blogId)
                .Where(c => categoriesId.Contains(c.Id))
                .Select(c => new Taxonomy() {
                    Id = c.Id,
                    Title = c.Title
                });

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