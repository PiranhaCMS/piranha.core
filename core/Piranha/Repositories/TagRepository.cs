/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Piranha.Repositories
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="cache">The optional model cache</param>
        public TagRepository(IDb db, ICache cache = null) 
            : base(db, cache) { }

        /// <summary>
        /// Gets the models for the post with the given id.
        /// </summary>
        /// <param name="postId">The post id</param>
        /// <returns>The model</returns>
        public IEnumerable<Tag> GetByPostId(Guid postId) {
            var tags = db.PostTags
                .AsNoTracking()
                .Where(t => t.PostId == postId)
                .Select(t => t.PostId);

            var models = new List<Tag>();

            foreach (var tag in tags) {
                var model = GetById(tag);

                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the model with the given slug.
        /// </summary>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public Tag GetBySlug(string slug) {
            var id = cache != null ? cache.Get<Guid?>($"Tag_{slug}") : null;
            Tag model = null;

            if (id.HasValue) {
                model = GetById(id.Value);
            } else {
                model = db.Tags
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Slug == slug);

                if (cache != null && model != null)
                    AddToCache(model);
            }
            return model;
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Add(Tag model) {
            PrepareInsert(model);

            // Check required
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title is required for Tag");

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = Utils.GenerateSlug(model.Title);
            else model.Slug = Utils.GenerateSlug(model.Slug);

            db.Tags.Add(model);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Update(Tag model) {
            PrepareUpdate(model);

            // Check required
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title is required for Tag");

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = Utils.GenerateSlug(model.Title);
            else model.Slug = Utils.GenerateSlug(model.Slug);

            var tag = db.Tags.FirstOrDefault(t => t.Id == model.Id);
            if (tag != null) {
                App.Mapper.Map<Tag, Tag>(model, tag);
            }
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Tag model) {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"Tag_{model.Slug}", model.Id);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Tag model) {
            cache.Remove(model.Id.ToString());
            cache.Remove($"Tag_{model.Slug}");
        }        
        #endregion
    }
}
