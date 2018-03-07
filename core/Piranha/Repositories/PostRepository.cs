/*
 * Copyright (c) 2016-2017 Håkan Edling
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
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Models;

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
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts</returns>
        public IEnumerable<DynamicPost> GetAll(Guid blogId)
        {
            var posts = Db.Posts
                .AsNoTracking()
                .Where(p => p.BlogId == blogId)
                .OrderByDescending(p => p.Published)
                .ThenByDescending(p => p.LastModified)
                .ThenBy(p => p.Title)
                .Select(p => p.Id);

            var models = new List<DynamicPost>();

            foreach (var post in posts)
            {
                var model = GetById(post);

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
        public IEnumerable<DynamicPost> GetAll(string slug, Guid? siteId = null)
        {
            if (!siteId.HasValue)
            {
                var site = Api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var blogId = Api.Pages.GetIdBySlug(slug, siteId);

            return blogId.HasValue ? GetAll(blogId.Value) : new List<DynamicPost>();
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public DynamicPost GetById(Guid id)
        {
            return GetById<DynamicPost>(id);
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public T GetById<T>(Guid id) where T : Post<T>
        {
            var post = Cache != null ? Cache.Get<Post>(id.ToString()) : null;

            if (post != null)
            {
                return Load<T, PostBase>(post, Api.PostTypes.GetById(post.PostTypeId), Process);
            }

            post = Db.Posts
                .AsNoTracking()
                .Include(p => p.Fields)
                .FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return null;
            }

            if (Cache != null)
            {
                AddToCache(post);
            }

            post.Category = Api.Categories.GetById(post.CategoryId);
            post.Blog = ((PageRepository)Api.Pages).GetPageById(post.BlogId);

            return Load<T, PostBase>(post, Api.PostTypes.GetById(post.PostTypeId), Process);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        public DynamicPost GetBySlug(string blog, string slug, Guid? siteId = null)
        {
            return GetBySlug<DynamicPost>(blog, slug, siteId);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The post model</returns>
        public T GetBySlug<T>(string blog, string slug, Guid? siteId = null) where T : Post<T>
        {
            if (!siteId.HasValue)
            {
                var site = Api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            }

            var blogId = Api.Pages.GetIdBySlug(blog, siteId);

            return blogId.HasValue ? GetBySlug<T>(blogId.Value, slug) : null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public DynamicPost GetBySlug(Guid blogId, string slug)
        {
            return GetBySlug<DynamicPost>(blogId, slug);
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blog">The unique blog slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public T GetBySlug<T>(Guid blogId, string slug) where T : Post<T>
        {
            var postId = Cache != null ? Cache.Get<Guid?>($"PostId_{blogId}_{slug}") : null;

            if (postId.HasValue)
            {
                // Load the post by id instead
                return GetById<T>(postId.Value);
            }
            // No cache found, load from database
            var post = Db.Posts
                .AsNoTracking()
                .Include(p => p.Fields)
                .FirstOrDefault(p => p.BlogId == blogId && p.Slug == slug);

            if (post == null)
            {
                return null;
            }

            if (Cache != null)
                AddToCache(post);
            post.Category = Api.Categories.GetById(post.CategoryId);
            post.Blog = ((PageRepository)Api.Pages).GetPageById(post.BlogId);

            return Load<T, PostBase>(post, Api.PostTypes.GetById(post.PostTypeId), Process);
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        public void Save<T>(T model) where T : Post<T>
        {
            var type = Api.PostTypes.GetById(model.TypeId);

            if (type == null)
            {
                return;
            }

            var currentRegions = type.Regions.Select(r => r.Id).ToArray();

            // Ensure category
            if (model.Category.Id == Guid.Empty)
            {
                Category category = null;

                if (!string.IsNullOrWhiteSpace(model.Category.Slug))
                    category = Api.Categories.GetBySlug(model.BlogId, model.Category.Slug);
                if (category == null && !string.IsNullOrWhiteSpace(model.Category.Title))
                    category = Api.Categories.GetByTitle(model.BlogId, model.Category.Title);

                if (category == null)
                {
                    category = new Category
                    {
                        Id = Guid.NewGuid(),
                        BlogId = model.BlogId,
                        Title = model.Category.Title
                    };
                    Api.Categories.Save(category);
                }
                model.Category.Id = category.Id;
            }

            // Ensure tags
            foreach (var t in model.Tags)
            {
                if (t.Id != Guid.Empty)
                {
                    continue;
                }
                Tag tag = null;

                if (!string.IsNullOrWhiteSpace(t.Slug))
                    tag = Api.Tags.GetBySlug(model.BlogId, t.Slug);

                if (tag == null && !string.IsNullOrWhiteSpace(t.Title))
                    tag = Api.Tags.GetByTitle(model.BlogId, t.Title);

                if (tag == null)
                {
                    tag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        BlogId = model.BlogId,
                        Title = t.Title
                    };
                    Api.Tags.Save(tag);
                }
                t.Id = tag.Id;
            }

            var post = Db.Posts
                .Include(p => p.Fields)
                .Include(p => p.Tags)
                .FirstOrDefault(p => p.Id == model.Id);

            // If not, create a new post
            if (post == null)
            {
                post = new Post
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };
                Db.Posts.Add(post);
                model.Id = post.Id;
            }
            else
            {
                post.LastModified = DateTime.Now;
            }

            // Ensure that we have a slug
            model.Slug = Utils.GenerateSlug(string.IsNullOrWhiteSpace(model.Slug) ? model.Title : model.Slug);

            // Map basic fields
            App.Mapper.Map<PostBase, Post>(model, post);

            // Remove tags
            var removedTags = new List<PostTag>();
            foreach (var tag in post.Tags)
            {
                if (model.Tags.All(t => t.Id != tag.TagId))
                {
                    removedTags.Add(tag);
                }
            }
            if (removedTags.Count > 0)
            {
                Db.PostTags.RemoveRange(removedTags);
            }

            // Add tags
            foreach (var tag in model.Tags)
            {
                if (!post.Tags.Any(t => t.PostId == post.Id && t.TagId == tag.Id))
                    post.Tags.Add(new PostTag
                    {
                        PostId = post.Id,
                        TagId = tag.Id
                    });
            }

            // Map regions
            foreach (var regionKey in currentRegions)
            {
                // Check that the region exists in the current model
                if (!HasRegion(model, regionKey))
                {
                    continue;
                }

                var regionType = type.Regions.Single(r => r.Id == regionKey);

                if (!regionType.Collection)
                {
                    MapRegion(model, post, GetRegion(model, regionKey), regionType, regionKey);
                }
                else
                {
                    var items = new List<Guid>();
                    var sortOrder = 0;
                    foreach (var region in GetEnumerable(model, regionKey))
                    {
                        var fields = MapRegion(model, post, region, regionType, regionKey, sortOrder++);

                        if (fields.Count > 0)
                            items.AddRange(fields);
                    }
                    // Now delete removed collection items
                    var removedFields = Db.PostFields
                        .Where(f => f.PostId == model.Id && f.RegionId == regionKey && !items.Contains(f.Id))
                        .ToList();

                    if (removedFields.Count > 0)
                        Db.PostFields.RemoveRange(removedFields);
                }
            }

            // If this is a published post, update last modified for the
            // blog page for caching purposes.
            if (post.Published.HasValue)
            {
                var page = Db.Pages
                    .FirstOrDefault(p => p.Id == post.BlogId);
                page.LastModified = DateTime.Now;
            }

            Db.SaveChanges();

            if (Cache != null)
            {
                RemoveFromCache(post);
            }
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id)
        {
            var model = Db.Posts
                .Include(p => p.Fields)
                .FirstOrDefault(p => p.Id == id);

            if (model == null)
            {
                return;
            }

            Db.Posts.Remove(model);

            // If this is a published post, update last modified for the
            // blog page for caching purposes.
            if (model.Published.HasValue)
            {
                var page = Db.Pages
                    .FirstOrDefault(p => p.Id == model.BlogId);
                page.LastModified = DateTime.Now;
            }

            Db.SaveChanges();

            // Check if we have the post in cache, and if so remove it

            var post = Cache?.Get<Post>(model.Id.ToString());
            if (post != null)
            {
                RemoveFromCache(post);
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void Delete<T>(T model) where T : Post<T>
        {
            Delete(model.Id);
        }

        /// <summary>
        /// Performs additional processing and loads related models.
        /// </summary>
        /// <param name="post">The source post</param>
        /// <param name="model">The targe model</param>
        private void Process<T>(Post post, T model) where T : PostBase
        {
            model.Category = Api.Categories.GetById(post.CategoryId);

            foreach (var tag in Api.Tags.GetByPostId(post.Id).OrderBy(t => t.Title))
            {
                model.Tags.Add(tag);
            }
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="post">The post</param>
        private void AddToCache(Post post)
        {
            Cache.Set(post.Id.ToString(), post);
            Cache.Set($"PostId_{post.CategoryId}_{post.Slug}", post.Id);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="post">The post</param>
        private void RemoveFromCache(Post post)
        {
            Cache.Remove(post.Id.ToString());
            Cache.Remove($"PostId_{post.CategoryId}_{post.Slug}");
        }
    }
}
