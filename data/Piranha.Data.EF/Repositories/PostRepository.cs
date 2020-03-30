/*
 * Copyright (c) .NET Foundation and Contributors
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
using Newtonsoft.Json;
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
        /// <param name="index">The optional page to fetch</param>
        /// <param name="pageSize">The optional page size</param>
        /// <returns>The posts</returns>
        public async Task<IEnumerable<Guid>> GetAll(Guid blogId, int? index = null, int? pageSize = null)
        {
            // Prepare base query
            IQueryable<Data.Post> query = _db.Posts
                .AsNoTracking()
                .Where(p => p.BlogId == blogId)
                .OrderByDescending(p => p.Published)
                .ThenByDescending(p => p.LastModified)
                .ThenBy(p => p.Title);

            // Add paging if requested
            if (index.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip(index.Value * pageSize.Value)
                    .Take(pageSize.Value);
            }

            // Execute query
            return await query
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
        /// <param name="blogId">The blog id</param>
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
        /// <param name="blogId">The blog id</param>
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
        /// Gets the id of all posts that have a draft for
        /// the specified blog.
        /// </summary>
        /// <param name="blogId">The unique blog id</param>
        /// <returns>The posts that have a draft</returns>
        public async Task<IEnumerable<Guid>> GetAllDrafts(Guid blogId)
        {
            return await _db.PostRevisions
                .AsNoTracking()
                .Where(r => r.Post.BlogId == blogId && r.Created > r.Post.LastModified)
                .Select(r => r.PostId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the comments available for the post with the specified id. If no post id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        public Task<IEnumerable<Models.Comment>> GetAllComments(Guid? postId, bool onlyApproved,
            int page, int pageSize)
        {
            return GetAllComments(postId, onlyApproved, false, page, pageSize);
        }

        /// <summary>
        /// Gets the pending comments available for the post with the specified id.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        public Task<IEnumerable<Models.Comment>> GetAllPendingComments(Guid? postId,
            int page, int pageSize)
        {
            return GetAllComments(postId, false, true, page, pageSize);
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
                return await _contentService.TransformAsync<T>(post, App.PostTypes.GetById(post.PostTypeId), ProcessAsync);
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
                return await _contentService.TransformAsync<T>(post, App.PostTypes.GetById(post.PostTypeId), ProcessAsync);
            }
            return null;
        }

        /// <summary>
        /// Gets the draft for the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        public async Task<T> GetDraftById<T>(Guid id) where T : Models.PostBase
        {
            DateTime? lastModified = await _db.Posts
                .Where(p => p.Id == id)
                .Select(p => p.LastModified)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (lastModified.HasValue)
            {
                var draft = await _db.PostRevisions
                    .FirstOrDefaultAsync(r => r.PostId == id && r.Created > lastModified)
                    .ConfigureAwait(false);

                if (draft != null)
                {
                    // Transform data model
                    var post = JsonConvert.DeserializeObject<Post>(draft.Data);

                    return await _contentService.TransformAsync<T>(post, App.PostTypes.GetById(post.PostTypeId), ProcessAsync);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the number of available posts in the specified archive.
        /// </summary>
        /// <param name="archiveId">The archive id</param>
        /// <returns>The number of posts</returns>
        public Task<int> GetCount(Guid archiveId)
        {
            return _db.Posts
                .Where(p => p.BlogId == archiveId)
                .CountAsync();
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
        /// Gets the category with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The category</returns>
        public Task<Models.Taxonomy> GetCategoryById(Guid id)
        {
            return _db.Categories
                .Where(c => c.Id == id)
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
                .Where(t => t.BlogId == blogId && t.Slug == slug)
                .Select(t => new Models.Taxonomy
                {
                    Id = t.Id,
                    Title = t.Title,
                    Slug = t.Slug
                }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the tag with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The category</returns>
        public Task<Models.Taxonomy> GetTagById(Guid id)
        {
            return _db.Tags
                .Where(t => t.Id == id)
                .Select(t => new Models.Taxonomy
                {
                    Id = t.Id,
                    Title = t.Title,
                    Slug = t.Slug
                }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the comment with the given id.
        /// </summary>
        /// <param name="id">The comment id</param>
        /// <returns>The model</returns>
        public Task<Models.Comment> GetCommentById(Guid id)
        {
            return _db.PostComments
                .Where(c => c.Id == id)
                .Select(c => new Models.Comment
                {
                    Id = c.Id,
                    ContentId = c.PostId,
                    UserId = c.UserId,
                    Author = c.Author,
                    Email = c.Email,
                    Url = c.Url,
                    IsApproved = c.IsApproved,
                    Body = c.Body,
                    Created = c.Created
                }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        public Task Save<T>(T model) where T : Models.PostBase
        {
            return Save<T>(model, false);
        }

        /// <summary>
        /// Saves the given model as a draft revision.
        /// </summary>
        /// <param name="model">The post model</param>
        public Task SaveDraft<T>(T model) where T : Models.PostBase
        {
            return Save<T>(model, true);
        }

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="model">The comment model</param>
        public async Task SaveComment(Guid postId, Models.Comment model)
        {
            var comment = await _db.PostComments
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (comment == null)
            {
                comment = new PostComment
                {
                    Id = model.Id
                };
                await _db.PostComments.AddAsync(comment);
            }

            comment.UserId = model.UserId;
            comment.PostId = postId;
            comment.Author = model.Author;
            comment.Email = model.Email;
            comment.Url = model.Url;
            comment.IsApproved = model.IsApproved;
            comment.Body = model.Body;
            comment.Created = model.Created;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a revision from the current version
        /// of the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="revisions">The maximum number of revisions that should be stored</param>
        public async Task CreateRevision(Guid id, int revisions)
        {
            var post = await GetQuery<Models.PostBase>()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (post != null)
            {
                await _db.PostRevisions.AddAsync(new PostRevision
                {
                    Id = Guid.NewGuid(),
                    PostId = id,
                    Data = JsonConvert.SerializeObject(post),
                    Created = post.LastModified
                }).ConfigureAwait(false);

                await _db.SaveChangesAsync().ConfigureAwait(false);

                // Check if we have a limit set on the number of revisions
                // we want to store.
                if (revisions != 0)
                {
                    var existing = await _db.PostRevisions
                        .Where(r => r.PostId == id)
                        .OrderByDescending(r => r.Created)
                        .Select(r => r.Id)
                        .Take(revisions)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    if (existing.Count == revisions)
                    {
                        var removed = await _db.PostRevisions
                            .Where(r => r.PostId == id && !existing.Contains(r.Id))
                            .ToListAsync()
                            .ConfigureAwait(false);

                        if (removed.Count > 0)
                        {
                            _db.PostRevisions.RemoveRange(removed);
                            await _db.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                }
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
        /// Deletes the current draft revision for the page
        /// with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteDraft(Guid id)
        {
            var post = await GetQuery<Models.PostInfo>()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (post != null)
            {
                var draft = await _db.PostRevisions
                    .Where(r => r.PostId == id && r.Created > post.LastModified)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (draft.Count > 0)
                {
                    _db.PostRevisions.RemoveRange(draft);

                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Deletes the comment with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteComment(Guid id)
        {
            var comment = await _db.PostComments
                .FirstOrDefaultAsync(c => c.Id == id)
                .ConfigureAwait(false);

            if (comment != null)
            {
                _db.PostComments.Remove(comment);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the comments available for the post with the specified id. If no post id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="postId">The unique post id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="onlyPending">If only pending comments should be fetched</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        public async Task<IEnumerable<Models.Comment>> GetAllComments(Guid? postId, bool onlyApproved,
            bool onlyPending, int page, int pageSize)
        {
            // Create base query
            IQueryable<PostComment> query = _db.PostComments
                .AsNoTracking();

            // Check if only should include a comments for a certain post
            if (postId.HasValue)
            {
                query = query.Where(c => c.PostId == postId.Value);
            }

            // Check if we should only include approved
            if (onlyPending)
            {
                query = query.Where(c => !c.IsApproved);
            }
            else if (onlyApproved)
            {
                query = query.Where(c => c.IsApproved);
            }

            // Order the comments by date
            query = query.OrderByDescending(c => c.Created);

            // Check if this is a paged query
            if (pageSize > 0)
            {
                query = query
                    .Skip(page * pageSize)
                    .Take(pageSize);
            }

            // Get the comments
            return await query
                .Select(c => new Models.Comment
                {
                    Id = c.Id,
                    ContentId = c.PostId,
                    UserId = c.UserId,
                    Author = c.Author,
                    Email = c.Email,
                    Url = c.Url,
                    IsApproved = c.IsApproved,
                    Body = c.Body,
                    Created = c.Created
                }).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        /// <param name="isDraft">If the model should be saved as a draft</param>
        private async Task Save<T>(T model, bool isDraft) where T : Models.PostBase
        {
            var type = App.PostTypes.GetById(model.TypeId);
            var lastModified = DateTime.MinValue;

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
                    model.Category.Title = category.Title;
                    model.Category.Slug = category.Slug;
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
                    t.Title = tag.Title;
                    t.Slug = tag.Slug;
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

                IQueryable<Post> postQuery = _db.Posts;
                if (isDraft)
                {
                    postQuery = postQuery.AsNoTracking();
                }

                var post = await postQuery
                    .Include(p => p.Permissions)
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields)
                    .Include(p => p.Tags).ThenInclude(t => t.Tag)
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
                    model.Id = post.Id;

                    if (!isDraft)
                    {
                        await _db.Posts.AddAsync(post).ConfigureAwait(false);
                    }
                }
                else
                {
                    post.LastModified = DateTime.Now;
                }
                post = _contentService.Transform<T>(model, type, post);

                // Set if comments should be enabled
                post.EnableComments = model.EnableComments;
                post.CloseCommentsAfterDays = model.CloseCommentsAfterDays;

                // Update permissions
                post.Permissions.Clear();
                foreach (var permission in model.Permissions)
                {
                    post.Permissions.Add(new PostPermission
                    {
                        PostId = post.Id,
                        Permission = permission
                    });
                }

                // Make sure foreign key is set for fields
                if (!isDraft)
                {
                    foreach (var field in post.Fields)
                    {
                        if (field.PostId == Guid.Empty)
                        {
                            field.PostId = post.Id;
                            await _db.PostFields.AddAsync(field).ConfigureAwait(false);
                        }
                    }
                }

                if (isDraft)
                {
                    post.Category = new Category
                    {
                        Id = model.Category.Id,
                        BlogId = model.BlogId,
                        Title = model.Category.Title,
                        Slug = model.Category.Slug
                    };
                }

                // Transform blocks
                var blockModels = model.Blocks;

                if (blockModels != null)
                {
                    var blocks = _contentService.TransformBlocks(blockModels);
                    var current = blocks.Select(b => b.Id).ToArray();

                    // Delete removed blocks
                    var removed = post.Blocks
                        .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable && b.Block.ParentId == null)
                        .Select(b => b.Block);
                    var removedItems = post.Blocks
                        .Where(b => !current.Contains(b.BlockId) && b.Block.ParentId != null && removed.Select(p => p.Id).ToList().Contains(b.Block.ParentId.Value))
                        .Select(b => b.Block);

                    if (!isDraft)
                    {
                        _db.Blocks.RemoveRange(removed);
                        _db.Blocks.RemoveRange(removedItems);
                    }

                    // Delete the old page blocks
                    post.Blocks.Clear();

                    // Now map the new block
                    for (var n = 0; n < blocks.Count; n++)
                    {
                        IQueryable<Block> blockQuery = _db.Blocks;
                        if (isDraft)
                        {
                            blockQuery = blockQuery.AsNoTracking();
                        }

                        var block = blockQuery
                            .Include(b => b.Fields)
                            .FirstOrDefault(b => b.Id == blocks[n].Id);

                        if (block == null)
                        {
                            block = new Block
                            {
                                Id = blocks[n].Id != Guid.Empty ? blocks[n].Id : Guid.NewGuid(),
                                Created = DateTime.Now
                            };
                            if (!isDraft)
                            {
                                await _db.Blocks.AddAsync(block).ConfigureAwait(false);
                            }
                        }
                        block.ParentId = blocks[n].ParentId;
                        block.CLRType = blocks[n].CLRType;
                        block.IsReusable = blocks[n].IsReusable;
                        block.Title = blocks[n].Title;
                        block.LastModified = DateTime.Now;

                        var currentFields = blocks[n].Fields.Select(f => f.FieldId).Distinct();
                        var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));

                        if (!isDraft)
                        {
                            _db.BlockFields.RemoveRange(removedFields);
                        }

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
                                if (!isDraft)
                                {
                                    await _db.BlockFields.AddAsync(field).ConfigureAwait(false);
                                }
                                block.Fields.Add(field);
                            }
                            field.SortOrder = newField.SortOrder;
                            field.CLRType = newField.CLRType;
                            field.Value = newField.Value;
                        }

                        // Create the post block
                        var postBlock = new PostBlock
                        {
                            Id = Guid.NewGuid(),
                            BlockId = block.Id,
                            Block = block,
                            PostId = post.Id,
                            SortOrder = n
                        };
                        if (!isDraft)
                        {
                            await _db.PostBlocks.AddAsync(postBlock).ConfigureAwait(false);
                        }
                        post.Blocks.Add(postBlock);
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
                    {
                        var postTag = new PostTag
                        {
                            PostId = post.Id,
                            TagId = tag.Id
                        };

                        if (isDraft)
                        {
                            postTag.Tag = new Tag
                            {
                                Id = tag.Id,
                                BlogId = post.BlogId,
                                Title = tag.Title,
                                Slug = tag.Slug
                            };
                        }
                        post.Tags.Add(postTag);
                    }
                }

                if (!isDraft)
                {
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                    await DeleteUnusedCategories(model.BlogId).ConfigureAwait(false);
                    await DeleteUnusedTags(model.BlogId).ConfigureAwait(false);
                }
                else
                {
                    var draft = await _db.PostRevisions
                        .FirstOrDefaultAsync(r => r.PostId == post.Id && r.Created > lastModified)
                        .ConfigureAwait(false);

                    if (draft == null)
                    {
                        draft = new PostRevision
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id
                        };
                        await _db.PostRevisions
                            .AddAsync(draft)
                            .ConfigureAwait(false);
                    }

                    draft.Data = JsonConvert.SerializeObject(post);
                    draft.Created = post.LastModified;

                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }
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
                .ToListAsync()
                .ConfigureAwait(false);

            var drafts = await _db.PostRevisions
                .Where(r => r.Post.BlogId == blogId && r.Created > r.Post.LastModified)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var draft in drafts)
            {
                var post = JsonConvert.DeserializeObject<Post>(draft.Data);
                used.Add(post.CategoryId);
            }
            used = used.Distinct().ToList();

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
                .ToListAsync()
                .ConfigureAwait(false);

            var drafts = await _db.PostRevisions
                .Where(r => r.Post.BlogId == blogId && r.Created > r.Post.LastModified)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var draft in drafts)
            {
                var post = JsonConvert.DeserializeObject<Post>(draft.Data);

                foreach (var tag in post.Tags)
                {
                    used.Add(tag.TagId);
                }
            }
            used = used.Distinct().ToList();

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
                .Include(p => p.Permissions)
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
        private async Task ProcessAsync<T>(Data.Post post, T model) where T : Models.PostBase
        {
            // Permissions
            foreach (var permission in post.Permissions)
            {
                model.Permissions.Add(permission.Permission);
            }

            // Comments
            model.EnableComments = post.EnableComments;
            if (model.EnableComments)
            {
                model.CommentCount = await _db.PostComments.CountAsync(c => c.PostId == model.Id).ConfigureAwait(false);
            }
            model.CloseCommentsAfterDays = post.CloseCommentsAfterDays;

            // Blocks
            if (!(model is Models.IContentInfo))
            {
                if (post.Blocks.Count > 0)
                {
                    foreach (var postBlock in post.Blocks.OrderBy(b => b.SortOrder))
                    {
                        if (postBlock.Block.ParentId.HasValue)
                        {
                            var parent = post.Blocks.FirstOrDefault(b => b.BlockId == postBlock.Block.ParentId.Value);
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
