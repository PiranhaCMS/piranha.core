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
using Piranha.Models;

namespace Piranha.Repositories
{
    /// <summary>
    /// The client category repository interface.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Gets the category with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The category</returns>
        CategoryItem GetById(Guid id);

        /// <summary>
        /// Gets the category with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The category</returns>
        CategoryItem GetBySlug(string slug);

        /// <summary>
        /// Gets the full category model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The category</returns>
        Category GetModelById(Guid id);

        /// <summary>
        /// Gets the full category model with the specified slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The category</returns>
        Category GetModelBySlug(string slug);

        /// <summary>
        /// Gets the available categories.
        /// </summary>
        /// <returns>The categories</returns>
        IList<CategoryItem> Get();

        /// <summary>
        /// Gets the available categories.
        /// </summary>
        /// <returns>The categories</returns>
        IList<Category> GetModels();

        /// <summary>
        /// Saves the category.
        /// </summary>
        /// <param name="model">The category</param>
        void Save(CategoryItem category);

        /// <summary>
        /// Saves the full category model.
        /// </summary>
        /// <param name="model">The full model</param>
        void Save(Category category);
    }
}
