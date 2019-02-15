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
    public class TagRepository : ITagRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public TagRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all available models for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<Tag>> GetAll(Guid blogId)
        {
            return await _db.Tags
                .AsNoTracking()
                .Where(t => t.BlogId == blogId)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the models for the post with the given id.
        /// </summary>
        /// <param name="postId">The post id</param>
        /// <returns>The model</returns>
        public async Task<IEnumerable<Tag>> GetByPostId(Guid postId)
        {
            return await _db.PostTags
                .AsNoTracking()
                .Where(t => t.PostId == postId)
                .Select(t => t.Tag)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        public Task<Tag> GetById(Guid id)
        {
            return _db.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Gets the model with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public Task<Tag> GetBySlug(Guid blogId, string slug)
        {
            return _db.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.BlogId == blogId && t.Slug == slug);
        }

        /// <summary>
        /// Gets the model with the given title
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="title">The unique title</param>
        /// <returns>The model</returns>
        public Task<Tag> GetByTitle(Guid blogId, string title)
        {
            return _db.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.BlogId == blogId && t.Title == title);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task Save(Tag model)
        {
            var tag = await _db.Tags
                .FirstOrDefaultAsync(t => t.Id == model.Id);

            if (tag == null)
            {
                tag = new Data.Tag
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                await _db.Tags.AddAsync(tag);
            }
            tag.BlogId = model.BlogId;
            tag.Title = model.Title;
            tag.Slug = model.Slug;
            tag.LastModified = model.LastModified;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var tag = await _db.Tags
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag != null)
            {
                _db.Tags.Remove(tag);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes all unused categories for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        public async Task DeleteUnused(Guid blogId)
        {
            var used = await _db.PostTags
                .Where(t => t.Post.BlogId == blogId)
                .Select(t => t.TagId)
                .Distinct()
                .ToArrayAsync();

            var unused = await _db.Tags
                .Where(t => t.BlogId == blogId && !used.Contains(t.Id))
                .ToListAsync();

            if (unused.Count > 0)
            {
                _db.Tags.RemoveRange(unused);
                await _db.SaveChangesAsync();
            }
        }
    }
}
