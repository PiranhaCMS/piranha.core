/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Manager;

namespace Piranha.Areas.Manager.Models
{
    public class PostEditModel : Piranha.Models.Post
    {
        public IList<Piranha.Models.CategoryItem> AllCategories { get; set; }

        public PostEditModel() {
            AllCategories = new List<Piranha.Models.CategoryItem>();
        }

        public static PostEditModel GetById(IApi api, Guid id) {
            var post = api.Posts.GetById(id);

            if (post != null) {
                var model = Module.Mapper.Map<Piranha.Models.Post, PostEditModel>(post);
                model.AllCategories = api.Categories.Get();

                return model;
            }
            throw new KeyNotFoundException($"No post found with the id '{id}'");
        }
    }
}
