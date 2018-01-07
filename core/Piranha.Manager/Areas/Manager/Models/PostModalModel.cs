/*
 * Copyright (c) 2018 Håkan Edling
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
    public class PostModalModel
    {
        public class PostModalItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public DateTime? Published { get; set; }
        }

        public class BlogItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public IEnumerable<PostModalItem> Posts { get; set; }
        public IEnumerable<BlogItem> Blogs { get; set; }

        public PostModalModel() {
            Posts = new List<PostModalItem>();
            Blogs = new List<BlogItem>();
        }

        public static PostModalModel GetByBlogId(IApi api, Guid? siteId = null, Guid? blogId = null) {
            var model = new PostModalModel();

            // Get default site if none is selected
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();

                if (site != null)
                    siteId = site.Id;
            }

            // Get the blogs available
            model.Blogs = api.Pages.GetAllBlogs(siteId.Value)
                .Select(p => new BlogItem() {
                    Id = p.Id,
                    Title = p.Title
                }).OrderBy(p => p.Title).ToList();

            if (!blogId.HasValue && model.Blogs.Count() > 0) {
                // Select the first blog
                blogId = model.Blogs.First().Id;
            }

            // Get the available posts
            model.Posts = api.Posts.GetAll(blogId.Value)
                .Select(p => new PostModalItem() {
                    Id = p.Id,
                    Title = p.Title,
                    Published = p.Published
                }).ToList();

            // Sort so we show unpublished drafts first
            model.Posts = model.Posts.Where(p => !p.Published.HasValue)
                .Concat(model.Posts.Where(p => p.Published.HasValue));

            return model;
        }
    }
}