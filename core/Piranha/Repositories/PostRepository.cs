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
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IDb db;
        private readonly IApi api;
        private readonly IContentService<Post, PostField, Models.PostBase> contentService;
        private readonly ICache cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="db">The current db connection</param>
        /// <param name="factory">The current content service factory</param>
        /// <param name="cache">The optional model cache</param>
        public PostRepository(IApi api, IDb db, IContentServiceFactory factory, ICache cache = null) { 
            this.db = db;
            this.api = api;
            this.contentService = factory.CreatePostService();
            this.cache = cache;
        }

        /// <summary>
        /// Creates and initializes a new post of the specified type.
        /// </summary>
        /// <returns>The created post</returns>
        public T Create<T>(string typeId = null) where T : Models.PostBase {
            if (string.IsNullOrWhiteSpace(typeId))
                typeId = typeof(T).Name;

            return contentService.Create<T>(api.PostTypes.GetById(typeId));
        }        

       /// <summary>
        /// Gets the available posts for the specified blog.
        /// </summary>
        /// <returns>The posts</returns>
        public IEnumerable<Models.DynamicPost> GetAll() {
            return GetAll<Models.DynamicPost>();
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <returns>The posts</returns>
        public IEnumerable<T> GetAll<T>() where T : Models.PostBase {
            var posts = db.Posts
                .AsNoTracking()
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
                post = GetQuery<T>(out var fullQuery)
                    .FirstOrDefault(p => p.Id == id);

                if (post != null) {
                    if (cache != null && fullQuery)
                        AddToCache(post);
                    post.Category = api.Categories.GetById(post.CategoryId);
                    //
                    // TODO: Ugly hardcoded reference!!!!
                    //
                    post.Blog = ((Repositories.PageRepository)api.Pages).GetPageById(post.BlogId);
                }
            }

            if (post != null)
                return contentService.Transform<T>(post, api.PostTypes.GetById(post.PostTypeId), Process);
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
                var post = GetQuery<T>(out var fullQuery)
                    .FirstOrDefault(p => p.BlogId == blogId && p.Slug == slug);

                if (post != null) {
                    if (cache != null && fullQuery)
                        AddToCache(post);
                    post.Category = api.Categories.GetById(post.CategoryId);
                    post.Blog = ((Repositories.PageRepository)api.Pages).GetPageById(post.BlogId);
            
                    return contentService.Transform<T>(post, api.PostTypes.GetById(post.PostTypeId), Process);
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

                // Ensure that we have a slug
                if (string.IsNullOrWhiteSpace(model.Slug))
                    model.Slug = Utils.GenerateSlug(model.Title, false);
                else model.Slug = Utils.GenerateSlug(model.Slug, false);                

                var post = db.Posts
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
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
                post = contentService.Transform<T>(model, type, post);

                // Transform blocks
                var blockModels = model.Blocks;

                if (blockModels != null && blockModels.Count > 0) {
                    var blocks = contentService.TransformBlocks(blockModels);
                    var current = blocks.Select(b => b.Id).ToArray();

                    // Delete removed blocks
                    var removed = post.Blocks
                        .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable)
                        .Select(b => b.Block);
                    db.Blocks.RemoveRange(removed);

                    // Delete the old page blocks
                    post.Blocks.Clear();

                    // Now map the new block
                    for (var n = 0; n < blocks.Count; n++) {
                        var block = db.Blocks
                            .Include(b => b.Fields)
                            .FirstOrDefault(b => b.Id == blocks[n].Id);
                        if (block == null) {
                            block = new Block() {
                                Id = blocks[n].Id != Guid.Empty ? blocks[n].Id : Guid.NewGuid(),
                                Created = DateTime.Now
                            };
                            db.Blocks.Add(block);
                        }
                        block.CLRType = blocks[n].CLRType;
                        block.IsReusable = blocks[n].IsReusable;
                        block.Title = blocks[n].Title;
                        block.LastModified = DateTime.Now;

                        var currentFields = blocks[n].Fields.Select(f => f.FieldId).Distinct();
                        var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));
                        db.BlockFields.RemoveRange(removedFields);

                        foreach (var newField in blocks[n].Fields) {
                            var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                            if (field == null) {
                                field = new BlockField() {
                                    Id = newField.Id != Guid.Empty ? newField.Id : Guid.NewGuid(),
                                    BlockId = block.Id,
                                    FieldId = newField.FieldId
                                };
                                db.BlockFields.Add(field);
                                block.Fields.Add(field);
                            }
                            field.SortOrder = newField.SortOrder;
                            field.CLRType = newField.CLRType;
                            field.Value = newField.Value;
                        }

                        // Create the page block
                        post.Blocks.Add(new PostBlock() {
                            Id = Guid.NewGuid(),
                            BlockId = block.Id,
                            Block = block,
                            PostId = post.Id,
                            SortOrder = n
                        });
                    }
                }                

                // Remove tags
                var removedTags = new List<PostTag>();
                foreach (var tag in post.Tags) {
                    if (!model.Tags.Any(t => t.Id == tag.TagId))
                        removedTags.Add(tag);
                }
                foreach (var removed in removedTags)
                    post.Tags.Remove(removed);

                // Add tags
                foreach (var tag in model.Tags) {
                    if (!post.Tags.Any(t => t.PostId == post.Id && t.TagId == tag.Id))
                        post.Tags.Add(new PostTag() {
                            PostId = post.Id,
                            TagId = tag.Id
                        });
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
                .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                .Include(p => p.Fields)
                .FirstOrDefault(p => p.Id == id);

            if (model != null) {
                // Remove all blocks that are not reusable
                foreach (var postBlock in model.Blocks) {
                    if (!postBlock.Block.IsReusable)
                        db.Blocks.Remove(postBlock.Block);
                }

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
        /// Gets the base query for loading posts.
        /// </summary>
        /// <param name="fullModel">If this is a full load or not</param>
        /// <typeparam name="T">The requested model type</typeparam>
        /// <returns>The queryable</returns>
        private IQueryable<Post> GetQuery<T>(out bool fullModel) {
            var loadRelated = !typeof(Models.IContentInfo).IsAssignableFrom(typeof(T));

            var query = db.Posts
                .AsNoTracking();

            if (loadRelated) {
                query = query
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields);
                fullModel = true;
            } else {
                fullModel = false;
            }
            return query;
        }        

        /// <summary>
        /// Performs additional processing and loads related models.
        /// </summary>
        /// <param name="post">The source post</param>
        /// <param name="model">The targe model</param>
        private void Process<T>(Data.Post post, T model) where T : Models.PostBase {
            if (!(model is Models.IContentInfo)) {
                if (post.Blocks.Count > 0) {
                    var blocks = post.Blocks
                        .OrderBy(b => b.SortOrder)
                        .Select(b => b.Block)
                        .ToList();
                    model.Blocks = contentService.TransformBlocks(blocks);
                }
            }
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
