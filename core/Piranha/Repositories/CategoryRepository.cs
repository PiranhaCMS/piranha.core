/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

namespace Piranha.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public CategoryRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all available models for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<Category>> GetAll(Guid blogId)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.BlogId == blogId)
                .OrderBy(c => c.Title)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        public Task<Category> GetById(Guid id)
        {
            return _db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Gets the model with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public Task<Category> GetBySlug(Guid blogId, string slug)
        {
            return _db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.BlogId == blogId && c.Slug == slug);
        }

        /// <summary>
        /// Gets the model with the given title
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="title">The unique title</param>
        /// <returns>The model</returns>
        public Task<Category> GetByTitle(Guid blogId, string title)
        {
            return _db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.BlogId == blogId && c.Title == title);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task Save(Category model)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (category == null)
            {
                category = new Data.Category
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                await _db.Categories.AddAsync(category);
            }
            category.BlogId = model.BlogId;
            category.Title = model.Title;
            category.Slug = model.Slug;
            category.LastModified = model.LastModified;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes all unused categories for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        public async Task DeleteUnused(Guid blogId)
        {
            var used = await _db.Posts
                .Where(p => p.BlogId == blogId)
                .Select(p => p.CategoryId)
                .Distinct()
                .ToArrayAsync();

            var unused = await _db.Categories
                .Where(c => c.BlogId == blogId && !used.Contains(c.Id))
                .ToListAsync();

            if (unused.Count > 0)
            {
                _db.Categories.RemoveRange(unused);
                await _db.SaveChangesAsync();
            }
        }
    }
}
