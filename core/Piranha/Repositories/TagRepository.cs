/*
 * Copyright (c) 2017-2018 Håkan Edling
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
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

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
        /// Gets all available models for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available models</returns>
        public IEnumerable<Tag> GetAll(Guid blogId)
        {
            var models = new List<Tag>();
            var tags = db.Tags
                .AsNoTracking()
                .Where(t => t.BlogId == blogId)
                .Select(t => t.Id);

            foreach (var t in tags)
            {
                var model = GetById(t);
                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the models for the post with the given id.
        /// </summary>
        /// <param name="postId">The post id</param>
        /// <returns>The model</returns>
        public IEnumerable<Tag> GetByPostId(Guid postId)
        {
            var tags = db.PostTags
                .AsNoTracking()
                .Where(t => t.PostId == postId)
                .Select(t => t.TagId);

            var models = new List<Tag>();

            foreach (var tag in tags)
            {
                var model = GetById(tag);

                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the model with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The model</returns>
        public Tag GetBySlug(Guid blogId, string slug)
        {
            var id = cache != null ? cache.Get<Guid?>($"Tag_{blogId}_{slug}") : null;
            Tag model = null;

            if (id.HasValue)
            {
                model = GetById(id.Value);
            }
            else
            {
                id = db.Tags
                    .AsNoTracking()
                    .Where(t => t.BlogId == blogId && t.Slug == slug)
                    .Select(t => t.Id)
                    .FirstOrDefault();

                if (id != Guid.Empty)
                    model = GetById(id.Value);
            }
            return model;
        }

        /// <summary>
        /// Gets the model with the given title
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="title">The unique title</param>
        /// <returns>The model</returns>
        public Tag GetByTitle(Guid blogId, string title)
        {
            var id = db.Tags
                .AsNoTracking()
                .Where(t => t.BlogId == blogId && t.Title == title)
                .Select(t => t.Id)
                .FirstOrDefault();

            if (id != Guid.Empty)
                return GetById(id);
            return null;
        }

        /// <summary>
        /// Deletes all unused tags for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        public void DeleteUnused(Guid blogId)
        {
            var used = db.PostTags
                .Where(t => t.Post.BlogId == blogId)
                .Select(t => t.TagId)
                .Distinct()
                .ToArray();

            var unused = db.Tags
                .Where(t => t.BlogId == blogId && !used.Contains(t.Id))
                .ToList();

            if (unused.Count > 0)
            {
                db.Tags.RemoveRange(unused);
                db.SaveChanges();
            }
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Add(Tag model)
        {
            PrepareInsert(model);

            // Check required
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title is required for Tag");

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = Utils.GenerateSlug(model.Title, false);
            else model.Slug = Utils.GenerateSlug(model.Slug, false);

            db.Tags.Add(model);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void Update(Tag model)
        {
            PrepareUpdate(model);

            // Check required
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title is required for Tag");

            // Ensure slug
            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = Utils.GenerateSlug(model.Title);
            else model.Slug = Utils.GenerateSlug(model.Slug);

            var tag = db.Tags.FirstOrDefault(t => t.Id == model.Id);
            if (tag != null)
            {
                App.Mapper.Map<Tag, Tag>(model, tag);
            }
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Tag model)
        {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"Tag_{model.BlogId}_{model.Slug}", model.Id);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Tag model)
        {
            cache.Remove(model.Id.ToString());
            cache.Remove($"Tag_{model.BlogId}_{model.Slug}");
        }
        #endregion
    }
}
