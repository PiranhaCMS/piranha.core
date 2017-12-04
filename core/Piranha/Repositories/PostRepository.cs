/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Repositories
{
    public class PostRepository : ContentRepository<Post, PostField>, IPostRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db connection</param>
        /// <param name="modelCache">The optional model cache</param>
        public PostRepository(Api api, IDb db, ICache modelCache = null) : base(api, db, modelCache) { }

        /// <summary>
        /// Gets the available posts.
        /// </summary>
        /// <returns>The posts</returns>
        public IEnumerable<Models.DynamicPost> GetAll() {
            var posts = db.Posts
                .AsNoTracking()
                .OrderBy(p => p.Published)
                .ThenBy(p => p.Title)
                .Select(p => p.Id);

            var models = new List<Models.DynamicPost>();

            foreach (var post in posts) {
                var model = GetById(post);

                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public Models.DynamicPost GetById(Guid id) {
            return GetById<Models.DynamicPost>(id);
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public T GetById<T>(Guid id) where T : Models.Post<T> {
            var post = cache != null ? cache.Get<Post>(id.ToString()) : null;

            if (post == null) {
                post = db.Posts
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.Id == id);

                if (post != null) {
                    if (cache != null)
                        AddToCache(post);
                    post.Category = api.Categories.GetById(post.CategoryId);
                }
            }

            if (post != null)
                return Load<T, Models.PostBase>(post, api.PostTypes.GetById(post.PostTypeId));
            return null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="category">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public Models.DynamicPost GetBySlug(string category, string slug) {
            return GetBySlug<Models.DynamicPost>(category, slug);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="categorySlug">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public T GetBySlug<T>(string categorySlug, string slug) where T : Models.Post<T> {
            var category = api.Categories.GetBySlug(categorySlug);

            if (category != null) {
                var postId = cache != null ? cache.Get<Guid?>($"PostId_{category.Id}_{slug}") : (Guid?)null;

                if (postId.HasValue) {
                    // Load the post by id instead
                    return GetById<T>(postId.Value);
                } else {
                    // No cache found, load from database
                    var post = db.Posts
                        .AsNoTracking()
                        .Include(p => p.Fields)
                        .FirstOrDefault(p => p.CategoryId == category.Id && p.Slug == slug);

                    if (post != null) {
                        if (cache != null)
                            AddToCache(post);
                        post.Category = category;
                
                        return Load<T, Models.PostBase>(post, api.PostTypes.GetById(post.PostTypeId));
                    }                    
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the available post items for the given category id.
        /// </summary>
        /// <param name="id">The unique category id</param>
        /// <returns>The posts</returns>
        public IList<Models.DynamicPost> GetByCategoryId(Guid id) {
            var posts = db.Posts
                .AsNoTracking()
                .Where(p => p.CategoryId == id)
                .OrderBy(p => p.Published)
                .ThenBy(p => p.Title)
                .Select(p => p.Id);

            var models = new List<Models.DynamicPost>();

            foreach (var post in posts) {
                var model = GetById(post);

                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the available post items for the given category slug.
        /// </summary>
        /// <param name="slug">The unique category slug</param>
        /// <returns>The posts</returns>
        public IList<Models.DynamicPost> GetByCategorySlug(string slug) {
            var models = new List<Models.DynamicPost>();
            var category = api.Categories.GetBySlug(slug);

            if (category != null) {
                var posts = db.Posts
                    .AsNoTracking()
                    .Where(p => p.CategoryId == category.Id)
                    .OrderBy(p => p.Published)
                    .ThenBy(p => p.Title)
                    .Select(p => p.Id);


                foreach (var post in posts) {
                    var model = GetById(post);

                    if (model != null)
                        models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        public void Save<T>(T model) where T : Models.Post<T> {
            var type = api.PostTypes.GetById(model.TypeId);

            if (type != null) {
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                var post = db.Posts
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.Id == model.Id);

                // If not, create a new post
                if (post == null) {
                    post = new Post() {
                        Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    db.Posts.Add(post);
                    model.Id = post.Id;
                } else {
                    post.LastModified = DateTime.Now;
                }

                // Ensure that we have a slug
                if (string.IsNullOrWhiteSpace(model.Slug))
                    model.Slug = Utils.GenerateSlug(model.Title);
                else model.Slug = Utils.GenerateSlug(model.Slug);

                // Map basic fields
                App.Mapper.Map<Models.PostBase, Post>(model, post);

                // Map regions
                foreach (var regionKey in currentRegions) {
                    // Check that the region exists in the current model
                    if (HasRegion(model, regionKey)) {
                        var regionType = type.Regions.Single(r => r.Id == regionKey);

                        if (!regionType.Collection) {
                            MapRegion(model, post, GetRegion(model, regionKey), regionType, regionKey);
                        } else {
                            var items = new List<Guid>();
                            var sortOrder = 0;
                            foreach (var region in GetEnumerable(model, regionKey)) {
                                var fields = MapRegion(model, post, region, regionType, regionKey, sortOrder++);

                                if (fields.Count > 0)
                                    items.AddRange(fields);
                            }
                            // Now delete removed collection items
                            var removedFields = db.PostFields
                                .Where(f => f.PostId == model.Id && f.RegionId == regionKey && !items.Contains(f.Id))
                                .ToList();

                            if (removedFields.Count > 0)
                                db.PostFields.RemoveRange(removedFields);
                        }
                    }
                }

                db.SaveChanges();

                if (cache != null)
                    RemoveFromCache(post);
            }
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            var model = db.Posts
                .Include(p => p.Fields)
                .FirstOrDefault(p => p.Id == id);

            if (model != null) {
                db.Posts.Remove(model);
                db.SaveChanges();

                // Check if we have the post in cache, and if so remove it
                if (cache != null) {
                    var post = cache.Get<Post>(model.Id.ToString());
                    if (post != null)
                        RemoveFromCache(post);
                }   
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void Delete<T>(T model) where T : Models.Post<T> {
            Delete(model.Id);
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="post">The post</param>
        private void AddToCache(Post post) {
            cache.Set(post.Id.ToString(), post);
            cache.Set($"PostId_{post.CategoryId}_{post.Slug}", post.Id);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="post">The post</param>
        private void RemoveFromCache(Post post) {
            cache.Remove(post.Id.ToString());
            cache.Remove($"PostId_{post.CategoryId}_{post.Slug}");
        }        
    }
}
