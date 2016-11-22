/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using System.Linq;
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    public class PostListModel
    {
        #region Properties
        public CategoryItem Category { get; set; }
        public IList<CategoryItem> Categories { get; set; }
        public IList<PostItem> Posts { get; set; }
        #endregion

        public PostListModel() {
            Categories = new List<CategoryItem>();
            Posts = new List<PostItem>();
        }

        public static PostListModel Get(IApi api, string categorySlug = null) {
            var model = new PostListModel();

            model.Categories = api.Categories.Get();
            if (!string.IsNullOrEmpty(categorySlug))
                model.Posts = api.Posts.GetByCategorySlug(categorySlug);
            else model.Posts = api.Posts.Get();

            if (!string.IsNullOrEmpty(categorySlug)) {
                model.Category = model.Categories
                    .FirstOrDefault(c => c.Slug == categorySlug);
            }
            return model;
        }
    }
}
