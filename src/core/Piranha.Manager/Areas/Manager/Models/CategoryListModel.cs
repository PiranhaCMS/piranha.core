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
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    public class CategoryListModel
    {
        public IList<CategoryItem> Categories { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CategoryListModel() {
            Categories = new List<CategoryItem>();
        }

        /// <summary>
        /// Gets the current category list model.
        /// </summary>
        /// <param name="api">The current api.</param>
        /// <returns>The model</returns>
        public static CategoryListModel Get(IApi api) {
            var model = new CategoryListModel();

            model.Categories = api.Categories.Get();

            return model;
        }
    }
}
