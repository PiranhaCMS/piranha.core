/*
 * Copyright (c) 2016-2018 Håkan Edling
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
        public PostRepository(Api api, IDb db, IServiceProvider services, ICache modelCache = null) : base(api, db, services, modelCache) { }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts</returns>
        public IEnumerable<Models.DynamicPost> GetAll(Guid blogId) {
            return GetAll<Models.DynamicPost>(blogId);
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <param name="blogId">The unique id</param>
        /// <returns>The posts</returns>
        public IEnumerable<T> GetAll<T>(Guid blogId) where T : Models.PostBase {
            var posts = db.Posts
                .AsNoTracking()
                .Where(p => p.BlogId == blogId)
                .OrderByDescending(p => p.Published)
                .ThenByDescending(p => p.LastModified)
                .ThenBy(p => p.Title)
                .Select(p => p.Id);

            var models = new List<T>();

            foreach (var post in posts) {
                var model = GetById<T>(post);

                if (model != null)
                    models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        public IEnumerable<Models.DynamicPost> GetAll(string slug, Guid? siteId = null) {
            return GetAll<Models.DynamicPost>(slug, siteId);
        }

        /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="slug">The blog slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The posts</returns>
        public IEnumerable<T> GetAll<T>(string slug, Guid? siteId = null) where T : Models.PostBase {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var blogId = api.Pages.GetIdBySlug(slug, siteId);

            if (blogId.HasValue)
                return GetAll<T>(blogId.Value);
            return new List<T>();            
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
        public T GetById<T>(Guid id) where T : Models.PostBase {
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
                    post.Blog = ((Repositories.PageRepository)api.Pages).GetPageById(post.BlogId);
                }
            }

            if (post != null)
                return Load<T, Models.PostBase>(post, api.PostTypes.GetById(post.PostTypeId), Process);
            return null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        public Models.DynamicPost GetBySlug(string blog, string slug, Guid? siteId = null) {
            return GetBySlug<Models.DynamicPost>(blog, slug, siteId);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        public T GetBySlug<T>(string blog, string slug, Guid? siteId = null) where T : Models.PostBase {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var blogId = api.Pages.GetIdBySlug(blog, siteId);

            if (blogId.HasValue)
                return GetBySlug<T>(blogId.Value, slug);
            return null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public Models.DynamicPost GetBySlug(Guid blogId, string slug) {
            return GetBySlug<Models.DynamicPost>(blogId, slug);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public T GetBySlug<T>(Guid blogId, string slug) where T : Models.PostBase {
            var postId = cache != null ? cache.Get<Guid?>($"PostId_{blogId}_{slug}") : (Guid?)null;

            if (postId.HasValue) {
                // Load the post by id instead
                return GetById<T>(postId.Value);
            } else {
                // No cache found, load from database
                var post = db.Posts
                    .AsNoTracking()
                    .Include(p => p.Fields)
                    .FirstOrDefault(p => p.BlogId == blogId && p.Slug == slug);

                if (post != null) {
                    if (cache != null)
                        AddToCache(post);
                    post.Category = api.Categories.GetById(post.CategoryId);
                    post.Blog = ((Repositories.PageRepository)api.Pages).GetPageById(post.BlogId);
            
                    return Load<T, Models.PostBase>(post, api.PostTypes.GetById(post.PostTypeId), Process);
                }                    
                return null;
            }
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        public void Save<T>(T model) where T : Models.PostBase {
            var type = api.PostTypes.GetById(model.TypeId);

            if (type != null) {
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                // Ensure category
                if (model.Category.Id == Guid.Empty) {
                    Data.Category category = null;

                    if (!string.IsNullOrWhiteSpace(model.Category.Slug))
                        category = api.Categories.GetBySlug(model.BlogId, model.Category.Slug);
                    if (category == null && !string.IsNullOrWhiteSpace(model.Category.Title))
                        category = api.Categories.GetByTitle(model.BlogId, model.Category.Title);                        

                    if (category == null) {
                        category = new Data.Category() {
                            Id = Guid.NewGuid(),
                            BlogId = model.BlogId,
                            Title = model.Category.Title
                        };
                        api.Categories.Save(category);
                    }
                    model.Category.Id = category.Id;
                }

                // Ensure tags
                foreach (var t in model.Tags) {
                    if (t.Id == Guid.Empty) {
                        Data.Tag tag = null;

                        if (!string.IsNullOrWhiteSpace(t.Slug))
                            tag = api.Tags.GetBySlug(model.BlogId, t.Slug);

                        if (tag == null && !string.IsNullOrWhiteSpace(t.Title))
                            tag = api.Tags.GetByTitle(model.BlogId, t.Title);

                        if (tag == null) {
                            tag = new Data.Tag() {
                                Id = Guid.NewGuid(),
                                BlogId = model.BlogId,
                                Title = t.Title
                            };
                            api.Tags.Save(tag);
                        }
                        t.Id = tag.Id;
                    }
                }

                var post = db.Posts
                    .Include(p => p.Fields)
                    .Include(p => p.Tags)
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

                // Remove tags
                var removedTags = new List<PostTag>();
                foreach (var tag in post.Tags) {
                    if (!model.Tags.Any(t => t.Id == tag.TagId))
                        removedTags.Add(tag);
                }
                if (removedTags.Count > 0)
                    db.PostTags.RemoveRange(removedTags);

                // Add tags
                foreach (var tag in model.Tags) {
                    if (!post.Tags.Any(t => t.PostId == post.Id && t.TagId == tag.Id))
                        post.Tags.Add(new PostTag() {
                            PostId = post.Id,
                            TagId = tag.Id
                        });
                }

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

                // If this is a published post, update last modified for the
                // blog page for caching purposes.
                if (post.Published.HasValue) {
                    var page = db.Pages
                        .FirstOrDefault(p => p.Id == post.BlogId);
                    page.LastModified = DateTime.Now;
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

                // If this is a published post, update last modified for the
                // blog page for caching purposes.
                if (model.Published.HasValue) {
                    var page = db.Pages
                        .FirstOrDefault(p => p.Id == model.BlogId);
                    page.LastModified = DateTime.Now;
                }

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
        public void Delete<T>(T model) where T : Models.PostBase {
            Delete(model.Id);
        }

        /// <summary>
        /// Performs additional processing and loads related models.
        /// </summary>
        /// <param name="post">The source post</param>
        /// <param name="model">The targe model</param>
        private void Process<T>(Data.Post post, T model) where T : Models.PostBase {
            model.Category = api.Categories.GetById(post.CategoryId);

            foreach (var tag in api.Tags.GetByPostId(post.Id).OrderBy(t => t.Title)) {
                model.Tags.Add((Models.Taxonomy)tag);
            }
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
