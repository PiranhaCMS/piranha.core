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
using Piranha.Manager;
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    public class CategoryEditModel : Category
    {
        /// <summary>
        /// Gets the category with the given id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="id">The unique id</param>
        /// <returns>The model</returns>
        public static CategoryEditModel GetById(IApi api, Guid id) {
            var category = api.Categories.GetModelById(id);

            if (category != null)
                return Module.Mapper.Map<Category, CategoryEditModel>(category);
            throw new ArgumentOutOfRangeException($"No category found with id '{id}'");
        }

        /// <summary>
        /// Saves the current category on the given api.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>If the operation was successful</returns>
        public bool Save(IApi api) {
            var category = Module.Mapper.Map<CategoryEditModel, Category>(this);
            api.Categories.Save(category);

            return true;
        }
    }
}