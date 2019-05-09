/*
 * Copyright (c) 2016-2019 Håkan Edling
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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Services;

namespace Piranha.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IDb _db;
        private readonly IContentService<Post, PostField, Models.PostBase> _contentService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="factory">The current content service factory</param>
        public PostRepository(IDb db, IContentServiceFactory factory)
        {
            _db = db;
            _contentService = factory.CreatePostService();
        }

        /// <summary>
        /// Gets the available posts for the specified archive.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <returns>The posts</returns>
        public async Task<IEnumerable<Guid>> GetAll(Guid blogId)
        {
            return await _db.Posts
                .AsNoTracking()
                .Where(p => p.BlogId == blogId)
                .OrderByDescending(p => p.Published)
                .ThenByDescending(p => p.LastModified)
                .ThenBy(p => p.Title)
                .Select(p => p.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the available post items for the given site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The posts</returns>
        public async Task<IEnumerable<Guid>> GetAllBySiteId(Guid siteId)
        {
            return await _db.Posts
                .AsNoTracking()
                .Where(p => p.Blog.SiteId == siteId)
                .OrderByDescending(p => p.Published)
                .ThenByDescending(p => p.LastModified)
                .ThenBy(p => p.Title)
                .Select(p => p.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all available categories for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available categories</returns>
        public async Task<IEnumerable<Models.Taxonomy>> GetAllCategories(Guid blogId)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.BlogId == blogId)
                .OrderBy(c => c.Title)
                .Select(c => new Models.Taxonomy
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all available tags for the specified blog.
        /// </summary>
        /// <param name="id">The blog id</param>
        /// <returns>The available tags</returns>
        public async Task<IEnumerable<Models.Taxonomy>> GetAllTags(Guid blogId)
        {
            return await _db.Tags
                .AsNoTracking()
                .Where(c => c.BlogId == blogId)
                .OrderBy(c => c.Title)
                .Select(c => new Models.Taxonomy
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public async Task<T> GetById<T>(Guid id) where T : Models.PostBase
        {
            var post = await GetQuery<T>()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (post != null)
            {
                return _contentService.Transform<T>(post, App.PostTypes.GetById(post.PostTypeId), Process);
            }
            return null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public async Task<T> GetBySlug<T>(Guid blogId, string slug) where T : Models.PostBase
        {
            // No cache found, load from database
            var post = await GetQuery<T>()
                .FirstOrDefaultAsync(p => p.BlogId == blogId && p.Slug == slug)
                .ConfigureAwait(false);

            if (post != null)
            {
                return _contentService.Transform<T>(post, App.PostTypes.GetById(post.PostTypeId), Process);
            }
            return null;
        }

        /// <summary>
        /// Gets the category with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The category</returns>
        public Task<Models.Taxonomy> GetCategoryBySlug(Guid blogId, string slug)
        {
            return _db.Categories
                .Where(c => c.BlogId == blogId && c.Slug == slug)
                .Select(c => new Models.Taxonomy
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug
                }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the tag with the given slug.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The tag</returns>
        public Task<Models.Taxonomy> GetTagBySlug(Guid blogId, string slug)
        {
            return _db.Tags
                .Where(c => c.BlogId == blogId && c.Slug == slug)
                .Select(c => new Models.Taxonomy
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug
                }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        public async Task Save<T>(T model) where T : Models.PostBase
        {
            var type = App.PostTypes.GetById(model.TypeId);

            if (type != null)
            {
                // Ensure category
                var category = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Id == model.Category.Id)
                    .ConfigureAwait(false);

                if (category == null)
                {
                    if (!string.IsNullOrWhiteSpace(model.Category.Slug))
                    {
                        category = await _db.Categories
                            .FirstOrDefaultAsync(c => c.BlogId == model.BlogId && c.Slug == model.Category.Slug)
                            .ConfigureAwait(false);
                    }
                    if (category == null && !string.IsNullOrWhiteSpace(model.Category.Title))
                    {
                        category = await _db.Categories
                            .FirstOrDefaultAsync(c => c.BlogId == model.BlogId && c.Title == model.Category.Title)
                            .ConfigureAwait(false);
                    }

                    if (category == null)
                    {
                        category = new Category
                        {
                            Id = model.Category.Id != Guid.Empty ? model.Category.Id : Guid.NewGuid(),
                            BlogId = model.BlogId,
                            Title = model.Category.Title,
                            Slug = Utils.GenerateSlug(model.Category.Title),
                            Created = DateTime.Now,
                            LastModified = DateTime.Now
                        };
                        await _db.Categories.AddAsync(category).ConfigureAwait(false);
                    }
                    model.Category.Id = category.Id;
                }

                // Ensure tags
                foreach (var t in model.Tags)
                {
                    var tag = await _db.Tags
                        .FirstOrDefaultAsync(tg => tg.Id == t.Id)
                        .ConfigureAwait(false);

                    if (tag == null)
                    {
                        if (!string.IsNullOrWhiteSpace(t.Slug))
                        {
                            tag = await _db.Tags
                                .FirstOrDefaultAsync(tg => tg.BlogId == model.BlogId && tg.Slug == t.Slug)
                                .ConfigureAwait(false);
                        }
                        if (tag == null && !string.IsNullOrWhiteSpace(t.Title))
                        {
                            tag = await _db.Tags
                                .FirstOrDefaultAsync(tg => tg.BlogId == model.BlogId && tg.Title == t.Title)
                                .ConfigureAwait(false);
                        }

                        if (tag == null)
                        {
                            tag = new Tag
                            {
                                Id = t.Id != Guid.Empty ? t.Id : Guid.NewGuid(),
                                BlogId = model.BlogId,
                                Title = t.Title,
                                Slug = Utils.GenerateSlug(t.Title),
                                Created = DateTime.Now,
                                LastModified = DateTime.Now
                            };
                            await _db.Tags.AddAsync(tag).ConfigureAwait(false);
                        }
                        t.Id = tag.Id;
                    }
                }

                // Ensure that we have a slug
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    model.Slug = Utils.GenerateSlug(model.Title, false);
                }
                else
                {
                    model.Slug = Utils.GenerateSlug(model.Slug, false);
                }

                var post = await _db.Posts
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields)
                    .Include(p => p.Tags)
                    .FirstOrDefaultAsync(p => p.Id == model.Id)
                    .ConfigureAwait(false);

                // If not, create a new post
                if (post == null)
                {
                    post = new Post
                    {
                        Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    await _db.Posts.AddAsync(post).ConfigureAwait(false);
                    model.Id = post.Id;
                }
                else
                {
                    post.LastModified = DateTime.Now;
                }
                post = _contentService.Transform<T>(model, type, post);

                // Transform blocks
                var blockModels = model.Blocks;

                if (blockModels != null)
                {
                    var blocks = _contentService.TransformBlocks(blockModels);
                    var current = blocks.Select(b => b.Id).ToArray();

                    // Delete removed blocks
                    var removed = post.Blocks
                        .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable)
                        .Select(b => b.Block);
                    _db.Blocks.RemoveRange(removed);

                    // Delete the old page blocks
                    post.Blocks.Clear();

                    // Now map the new block
                    for (var n = 0; n < blocks.Count; n++)
                    {
                        var block = _db.Blocks
                            .Include(b => b.Fields)
                            .FirstOrDefault(b => b.Id == blocks[n].Id);
                        if (block == null)
                        {
                            block = new Block
                            {
                                Id = blocks[n].Id != Guid.Empty ? blocks[n].Id : Guid.NewGuid(),
                                Created = DateTime.Now
                            };
                            await _db.Blocks.AddAsync(block).ConfigureAwait(false);
                        }
                        block.CLRType = blocks[n].CLRType;
                        block.IsReusable = blocks[n].IsReusable;
                        block.Title = blocks[n].Title;
                        block.LastModified = DateTime.Now;

                        var currentFields = blocks[n].Fields.Select(f => f.FieldId).Distinct();
                        var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));
                        _db.BlockFields.RemoveRange(removedFields);

                        foreach (var newField in blocks[n].Fields)
                        {
                            var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                            if (field == null)
                            {
                                field = new BlockField
                                {
                                    Id = newField.Id != Guid.Empty ? newField.Id : Guid.NewGuid(),
                                    BlockId = block.Id,
                                    FieldId = newField.FieldId
                                };
                                await _db.BlockFields.AddAsync(field).ConfigureAwait(false);
                                block.Fields.Add(field);
                            }
                            field.SortOrder = newField.SortOrder;
                            field.CLRType = newField.CLRType;
                            field.Value = newField.Value;
                        }

                        // Create the page block
                        post.Blocks.Add(new PostBlock
                        {
                            Id = Guid.NewGuid(),
                            BlockId = block.Id,
                            ParentId = blocks[n].ParentId,
                            Block = block,
                            PostId = post.Id,
                            SortOrder = n
                        });
                    }
                }

                // Remove tags
                var removedTags = new List<PostTag>();
                foreach (var tag in post.Tags)
                {
                    if (!model.Tags.Any(t => t.Id == tag.TagId))
                    {
                        removedTags.Add(tag);
                    }
                }
                foreach (var removed in removedTags)
                {
                    post.Tags.Remove(removed);
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

                await _db.SaveChangesAsync().ConfigureAwait(false);
                await DeleteUnusedCategories(model.BlogId).ConfigureAwait(false);
                await DeleteUnusedTags(model.BlogId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var model = await _db.Posts
                .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                .Include(p => p.Fields)
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (model != null)
            {
                // Remove all blocks that are not reusable
                foreach (var postBlock in model.Blocks)
                {
                    if (!postBlock.Block.IsReusable)
                    {
                        _db.Blocks.Remove(postBlock.Block);
                    }
                }

                _db.Posts.Remove(model);

                //
                // TODO
                //
                // If this is a published post, update last modified for the
                // blog page for caching purposes.
                if (model.Published.HasValue)
                {
                    var page = await _db.Pages
                        .FirstOrDefaultAsync(p => p.Id == model.BlogId)
                        .ConfigureAwait(false);
                    page.LastModified = DateTime.Now;
                }

                await _db.SaveChangesAsync().ConfigureAwait(false);
                await DeleteUnusedCategories(model.BlogId).ConfigureAwait(false);
                await DeleteUnusedTags(model.BlogId).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes all unused categories for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        private async Task DeleteUnusedCategories(Guid blogId)
        {
            var used = await _db.Posts
                .Where(p => p.BlogId == blogId)
                .Select(p => p.CategoryId)
                .Distinct()
                .ToArrayAsync()
                .ConfigureAwait(false);

            var unused = await _db.Categories
                .Where(c => c.BlogId == blogId && !used.Contains(c.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            if (unused.Count > 0)
            {
                _db.Categories.RemoveRange(unused);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes all unused tags for the specified blog.
        /// </summary>
        /// <param name="blogId">The blog id</param>
        private async Task DeleteUnusedTags(Guid blogId)
        {
            var used = await _db.PostTags
                .Where(t => t.Post.BlogId == blogId)
                .Select(t => t.TagId)
                .Distinct()
                .ToArrayAsync()
                .ConfigureAwait(false);

            var unused = await _db.Tags
                .Where(t => t.BlogId == blogId && !used.Contains(t.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            if (unused.Count > 0)
            {
                _db.Tags.RemoveRange(unused);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the base query for loading posts.
        /// </summary>
        /// <typeparam name="T">The requested model type</typeparam>
        /// <returns>The queryable</returns>
        private IQueryable<Post> GetQuery<T>()
        {
            var loadRelated = !typeof(Models.IContentInfo).IsAssignableFrom(typeof(T));

            IQueryable<Post> query = _db.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Tags).ThenInclude(t => t.Tag);

            if (loadRelated)
            {
                query = query
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields);
            }
            return query;
        }

        /// <summary>
        /// Performs additional processing and loads related models.
        /// </summary>
        /// <param name="post">The source post</param>
        /// <param name="model">The targe model</param>
        private void Process<T>(Data.Post post, T model) where T : Models.PostBase
        {
            if (!(model is Models.IContentInfo))
            {
                if (post.Blocks.Count > 0)
                {
                    foreach (var postBlock in post.Blocks.OrderBy(b => b.SortOrder))
                    {
                        if (postBlock.ParentId.HasValue)
                        {
                            var parent = post.Blocks.FirstOrDefault(b => b.BlockId == postBlock.ParentId.Value);
                            if (parent != null)
                            {
                                postBlock.Block.ParentId = parent.Block.Id;
                            }
                        }
                    }
                    model.Blocks = _contentService.TransformBlocks(post.Blocks.OrderBy(b => b.SortOrder).Select(b => b.Block));

                }
            }
        }
    }
}
