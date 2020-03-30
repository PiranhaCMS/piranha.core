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
    public class PageRepository : IPageRepository
    {
        private readonly IDb _db;
        private readonly IContentService<Page, PageField, Models.PageBase> _contentService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="factory">The content service factory</param>
        public PageRepository(IDb db, IContentServiceFactory factory)
        {
            _db = db;
            _contentService = factory.CreatePageService();
        }

        /// <summary>
        /// Gets all of the available pages for the current site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The pages</returns>
        public async Task<IEnumerable<Guid>> GetAll(Guid siteId)
        {
            return await _db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == siteId)
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .Select(p => p.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The pages</returns>
        public async Task<IEnumerable<Guid>> GetAllBlogs(Guid siteId)
        {
            return await _db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == siteId && p.ContentType == "Blog")
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .Select(p => p.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the id of all pages that have a draft for
        /// the specified site.
        /// </summary>
        /// <param name="siteId">The unique site id</param>
        /// <returns>The pages that have a draft</returns>
        public async Task<IEnumerable<Guid>> GetAllDrafts(Guid siteId)
        {
            return await _db.PageRevisions
                .AsNoTracking()
                .Where(r => r.Page.SiteId == siteId && r.Created > r.Page.LastModified)
                .Select(r => r.PageId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the comments available for the page with the specified id. If no page id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="pageId">The unique post id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        public Task<IEnumerable<Models.Comment>> GetAllComments(Guid? pageId, bool onlyApproved,
            int page, int pageSize)
        {
            return GetAllComments(pageId, onlyApproved, false, page, pageSize);
        }

        /// <summary>
        /// Gets the pending comments available for the page with the specified id.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        public Task<IEnumerable<Models.Comment>> GetAllPendingComments(Guid? pageId,
            int page, int pageSize)
        {
            return GetAllComments(pageId, false, true, page, pageSize);
        }

        /// <summary>
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="siteId">The site id</param>
        /// <returns>The page model</returns>
        public async Task<T> GetStartpage<T>(Guid siteId) where T : Models.PageBase
        {
            var page = await GetQuery<T>()
                .FirstOrDefaultAsync(p => p.SiteId == siteId && p.ParentId == null && p.SortOrder == 0)
                .ConfigureAwait(false);

            if (page != null)
            {
                return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageTypeId), ProcessAsync);
            }
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The page model</returns>
        public async Task<T> GetById<T>(Guid id) where T : Models.PageBase
        {
            var page = await GetQuery<T>()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (page != null)
            {
                return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageTypeId), ProcessAsync);
            }
            return null;
        }

        /// <summary>
        /// Gets the page model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="slug">The unique slug</param>
        /// <param name="siteId">The site id</param>
        /// <returns>The page model</returns>
        public async Task<T> GetBySlug<T>(string slug, Guid siteId) where T : Models.PageBase
        {
            var page = await GetQuery<T>()
                .FirstOrDefaultAsync(p => p.SiteId == siteId && p.Slug == slug)
                .ConfigureAwait(false);

            if (page != null)
            {
                return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageTypeId), ProcessAsync);
            }
            return null;
        }

        /// <summary>
        /// Gets the draft for the page model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The draft, or null if no draft exists</returns>
        public async Task<T> GetDraftById<T>(Guid id) where T : Models.PageBase
        {
            DateTime? lastModified = await _db.Pages
                .Where(p => p.Id == id)
                .Select(p => p.LastModified)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (lastModified.HasValue)
            {
                var draft = await _db.PageRevisions
                    .FirstOrDefaultAsync(r => r.PageId == id && r.Created > lastModified)
                    .ConfigureAwait(false);

                if (draft != null)
                {
                    // Transform data model
                    var page = JsonConvert.DeserializeObject<Page>(draft.Data);

                    return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageTypeId), ProcessAsync);
                }
            }
            return null;
        }

        /// <summary>
        /// Moves the current page in the structure.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The page to move</param>
        /// <param name="parentId">The new parent id</param>
        /// <param name="sortOrder">The new sort order</param>
        /// <returns>The other pages that were affected by the move</returns>
        public async Task<IEnumerable<Guid>> Move<T>(T model, Guid? parentId, int sortOrder) where T : Models.PageBase
        {
            var affected = new List<Guid>();

            var source = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
            var dest = model.ParentId == parentId ? source : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == parentId).ToListAsync().ConfigureAwait(false);

            // Remove the old position for the page
            affected.AddRange(MovePages(source, model.Id, model.SiteId, model.ParentId, model.SortOrder + 1, false));
            // Add room for the new position of the page
            affected.AddRange(MovePages(dest, model.Id, model.SiteId, parentId, sortOrder, true));

            // Update the position of the current page
            var page = await _db.Pages
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                .ConfigureAwait(false);
            page.ParentId = parentId;
            page.SortOrder = sortOrder;

            await _db.SaveChangesAsync().ConfigureAwait(false);

            return affected;
        }

        /// <summary>
        /// Gets the comment with the given id.
        /// </summary>
        /// <param name="id">The comment id</param>
        /// <returns>The model</returns>
        public Task<Models.Comment> GetCommentById(Guid id)
        {
            return _db.PageComments
                .Where(c => c.Id == id)
                .Select(c => new Models.Comment
                {
                    Id = c.Id,
                    ContentId = c.PageId,
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
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        /// <returns>The other pages that were affected by the move</returns>
        public Task<IEnumerable<Guid>> Save<T>(T model) where T : Models.PageBase
        {
            return Save<T>(model, false);
        }

        /// <summary>
        /// Saves the given model as a draft revision.
        /// </summary>
        /// <param name="model">The page model</param>
        public async Task SaveDraft<T>(T model) where T : Models.PageBase
        {
            await Save<T>(model, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="model">The comment model</param>
        public async Task SaveComment(Guid pageId, Models.Comment model)
        {
            var comment = await _db.PageComments
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (comment == null)
            {
                comment = new PageComment
                {
                    Id = model.Id
                };
                await _db.PageComments.AddAsync(comment);
            }

            comment.UserId = model.UserId;
            comment.PageId = pageId;
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
            var page = await GetQuery<Models.PageBase>()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (page != null)
            {
                await _db.PageRevisions.AddAsync(new PageRevision
                {
                    Id = Guid.NewGuid(),
                    PageId = id,
                    Data = JsonConvert.SerializeObject(page),
                    Created = page.LastModified
                }).ConfigureAwait(false);

                await _db.SaveChangesAsync().ConfigureAwait(false);

                // Check if we have a limit set on the number of revisions
                // we want to store.
                if (revisions != 0)
                {
                    var existing = await _db.PageRevisions
                        .Where(r => r.PageId == id)
                        .OrderByDescending(r => r.Created)
                        .Select(r => r.Id)
                        .Take(revisions)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    if (existing.Count == revisions)
                    {
                        var removed = await _db.PageRevisions
                            .Where(r => r.PageId == id && !existing.Contains(r.Id))
                            .ToListAsync()
                            .ConfigureAwait(false);

                        if (removed.Count > 0)
                        {
                            _db.PageRevisions.RemoveRange(removed);
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
        public async Task<IEnumerable<Guid>> Delete(Guid id)
        {
            var model = await _db.Pages
                .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                .Include(p => p.Fields)
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);
            var affected = new List<Guid>();

            if (model != null)
            {
                // Make sure this page isn't copied
                var copyCount = await _db.Pages.CountAsync(p => p.OriginalPageId == model.Id).ConfigureAwait(false);
                if (copyCount > 0)
                {
                    throw new InvalidOperationException("Can not delete page because it has copies");
                }

                // Make sure this page doesn't have child pages
                var childCount = await _db.Pages.CountAsync(p => p.ParentId == model.Id).ConfigureAwait(false);
                if (childCount > 0)
                {
                    throw new InvalidOperationException("Can not delete page because it has children");
                }

                // Remove all blocks that are not reusable
                foreach (var pageBlock in model.Blocks)
                {
                    if (!pageBlock.Block.IsReusable)
                    {
                        _db.Blocks.Remove(pageBlock.Block);
                    }
                }

                // Remove the main page.
                _db.Pages.Remove(model);

                var siblings = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);

                // Move all remaining pages after this page in the site structure.
                affected.AddRange(MovePages(siblings, id, model.SiteId, model.ParentId, model.SortOrder + 1, false));

                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            return affected;
        }

        /// <summary>
        /// Deletes the current draft revision for the page
        /// with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task DeleteDraft(Guid id)
        {
            var page = await GetQuery<Models.PageInfo>()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (page != null)
            {
                var draft = await _db.PageRevisions
                    .Where(r => r.PageId == id && r.Created > page.LastModified)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (draft.Count > 0)
                {
                    _db.PageRevisions.RemoveRange(draft);

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
            var comment = await _db.PageComments
                .FirstOrDefaultAsync(c => c.Id == id)
                .ConfigureAwait(false);

            if (comment != null)
            {
                _db.PageComments.Remove(comment);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the comments available for the page with the specified id. If no page id
        /// is provided all comments are fetched.
        /// </summary>
        /// <param name="pageId">The unique page id</param>
        /// <param name="onlyApproved">If only approved comments should be fetched</param>
        /// <param name="onlyPending">If only pending comments should be fetched</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The available comments</returns>
        public async Task<IEnumerable<Models.Comment>> GetAllComments(Guid? pageId, bool onlyApproved,
            bool onlyPending, int page, int pageSize)
        {
            // Create base query
            IQueryable<PageComment> query = _db.PageComments
                .AsNoTracking();

            // Check if only should include a comments for a certain post
            if (pageId.HasValue)
            {
                query = query.Where(c => c.PageId == pageId.Value);
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
                    ContentId = c.PageId,
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
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        /// <param name="isDraft">If the model should be saved as a draft</param>
        private async Task<IEnumerable<Guid>> Save<T>(T model, bool isDraft) where T : Models.PageBase
        {
            var type = App.PageTypes.GetById(model.TypeId);
            var affected = new List<Guid>();
            var isNew = false;
            var lastModified = DateTime.MinValue;

            if (type != null)
            {
                IQueryable<Page> pageQuery = _db.Pages;
                if (isDraft)
                {
                    pageQuery = pageQuery.AsNoTracking();
                }

                var page = await pageQuery
                    .Include(p => p.Permissions)
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields)
                    .FirstOrDefaultAsync(p => p.Id == model.Id)
                    .ConfigureAwait(false);

                if (page == null)
                {
                    isNew = true;
                }
                else
                {
                    lastModified = page.LastModified;
                }

                if (model.OriginalPageId.HasValue)
                {
                    var originalPageIsCopy = (await _db.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.OriginalPageId).ConfigureAwait(false))?.OriginalPageId.HasValue ?? false;
                    if (originalPageIsCopy)
                    {
                        throw new InvalidOperationException("Can not set copy of a copy");
                    }

                    var originalPageType = (await _db.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.OriginalPageId).ConfigureAwait(false))?.PageTypeId;
                    if (originalPageType != model.TypeId)
                    {
                        throw new InvalidOperationException("Copy can not have a different content type");
                    }

                    // Transform the model
                    if (page == null)
                    {
                        page = new Page()
                        {
                            Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                            Created = DateTime.Now,
                        };

                        if (!isDraft)
                        {
                            await _db.Pages.AddAsync(page).ConfigureAwait(false);

                            // Make room for the new page
                            var dest = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);
                            affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
                        }
                    }
                    else
                    {
                        // Check if the page has been moved
                        if (!isDraft && (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder))
                        {
                            var source = await _db.Pages.Where(p => p.SiteId == page.SiteId && p.ParentId == page.ParentId && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
                            var dest = page.ParentId == model.ParentId ? source : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);

                            // Remove the old position for the page
                            affected.AddRange(MovePages(source, page.Id, page.SiteId, page.ParentId, page.SortOrder + 1, false));
                            // Add room for the new position of the page
                            affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
                        }
                    }

                    if (!isDraft && (isNew || page.Title != model.Title || page.NavigationTitle != model.NavigationTitle))
                    {
                        // If this is new page or title has been updated it means
                        // the global sitemap changes. Notify the service.
                        affected.Add(page.Id);
                    }

                    page.ContentType = type.IsArchive ? "Blog" : "Page";
                    page.PageTypeId = model.TypeId;
                    page.OriginalPageId = model.OriginalPageId;
                    page.SiteId = model.SiteId;
                    page.Title = model.Title;
                    page.NavigationTitle = model.NavigationTitle;
                    page.Slug = model.Slug;
                    page.ParentId = model.ParentId;
                    page.SortOrder = model.SortOrder;
                    page.IsHidden = model.IsHidden;
                    page.Route = model.Route;
                    page.Published = model.Published;
                    page.LastModified = DateTime.Now;

                    page.Permissions.Clear();
                    foreach (var permission in model.Permissions)
                    {
                        page.Permissions.Add(new PagePermission
                        {
                            PageId = page.Id,
                            Permission = permission
                        });
                    }

                    if (!isDraft)
                    {
                        await _db.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        var draft = await _db.PageRevisions
                            .FirstOrDefaultAsync(r => r.PageId == page.Id && r.Created > lastModified)
                            .ConfigureAwait(false);

                        if (draft == null)
                        {
                            draft = new PageRevision
                            {
                                Id = Guid.NewGuid(),
                                PageId = page.Id
                            };
                            await _db.PageRevisions
                                .AddAsync(draft)
                                .ConfigureAwait(false);
                        }

                        draft.Data = JsonConvert.SerializeObject(page);
                        draft.Created = page.LastModified;

                        await _db.SaveChangesAsync().ConfigureAwait(false);
                    }
                    return affected;
                }

                // Transform the model
                if (page == null)
                {
                    page = new Page
                    {
                        Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                        ParentId = model.ParentId,
                        SortOrder = model.SortOrder,
                        PageTypeId = model.TypeId,
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    model.Id = page.Id;

                    if (!isDraft)
                    {
                        await _db.Pages.AddAsync(page).ConfigureAwait(false);

                        // Make room for the new page
                        var dest = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);
                        affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
                    }
                }
                else
                {
                    // Check if the page has been moved
                    if (!isDraft && (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder))
                    {
                        var source = await _db.Pages.Where(p => p.SiteId == page.SiteId && p.ParentId == page.ParentId && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
                        var dest = page.ParentId == model.ParentId ? source : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);

                        // Remove the old position for the page
                        affected.AddRange(MovePages(source, page.Id, page.SiteId, page.ParentId, page.SortOrder + 1, false));
                        // Add room for the new position of the page
                        affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
                    }
                    page.LastModified = DateTime.Now;
                }

                if (isNew || page.Title != model.Title || page.NavigationTitle != model.NavigationTitle)
                {
                    // If this is new page or title has been updated it means
                    // the global sitemap changes. Notify the service.
                    affected.Add(page.Id);
                }

                page = _contentService.Transform<T>(model, type, page);
                page.ContentType = type.IsArchive ? "Blog" : "Page";

                // Set if comments should be enabled
                page.EnableComments = model.EnableComments;
                page.CloseCommentsAfterDays = model.CloseCommentsAfterDays;

                // Update permissions
                page.Permissions.Clear();
                foreach (var permission in model.Permissions)
                {
                    page.Permissions.Add(new PagePermission
                    {
                        PageId = page.Id,
                        Permission = permission
                    });
                }

                // Make sure foreign key is set for fields
                if (!isDraft)
                {
                    foreach (var field in page.Fields)
                    {
                        if (field.PageId == Guid.Empty)
                        {
                            field.PageId = page.Id;
                            await _db.PageFields.AddAsync(field).ConfigureAwait(false);
                        }
                    }
                }

                // Transform blocks
                var blockModels = model.Blocks;

                if (blockModels != null)
                {
                    var blocks = _contentService.TransformBlocks(blockModels);
                    var current = blocks.Select(b => b.Id).ToArray();

                    // Delete removed blocks
                    var removed = page.Blocks
                        .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable && b.Block.ParentId == null)
                        .Select(b => b.Block);
                    var removedItems = page.Blocks
                        .Where(b => !current.Contains(b.BlockId) && b.Block.ParentId != null && removed.Select(p => p.Id).ToList().Contains(b.Block.ParentId.Value))
                        .Select(b => b.Block);

                    if (!isDraft)
                    {
                        _db.Blocks.RemoveRange(removed);
                        _db.Blocks.RemoveRange(removedItems);
                    }

                    // Delete the old page blocks
                    page.Blocks.Clear();

                    // Now map the new block
                    for (var n = 0; n < blocks.Count; n++)
                    {
                        IQueryable<Block> blockQuery = _db.Blocks;
                        if (isDraft)
                        {
                            blockQuery = blockQuery.AsNoTracking();
                        }

                        var block = await blockQuery
                            .Include(b => b.Fields)
                            .FirstOrDefaultAsync(b => b.Id == blocks[n].Id)
                            .ConfigureAwait(false);

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

                        // Create the page block
                        var pageBlock = new PageBlock
                        {
                            Id = Guid.NewGuid(),
                            BlockId = block.Id,
                            Block = block,
                            PageId = page.Id,
                            SortOrder = n
                        };
                        if (!isDraft)
                        {
                            await _db.PageBlocks.AddAsync(pageBlock).ConfigureAwait(false);
                        }
                        page.Blocks.Add(pageBlock);
                    }
                }
                if (!isDraft)
                {
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    var draft = await _db.PageRevisions
                        .FirstOrDefaultAsync(r => r.PageId == page.Id && r.Created > lastModified)
                        .ConfigureAwait(false);

                    if (draft == null)
                    {
                        draft = new PageRevision
                        {
                            Id = Guid.NewGuid(),
                            PageId = page.Id
                        };
                        await _db.PageRevisions
                            .AddAsync(draft)
                            .ConfigureAwait(false);
                    }

                    draft.Data = JsonConvert.SerializeObject(page);
                    draft.Created = page.LastModified;

                    await _db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            return affected;
        }

        /// <summary>
        /// Gets the base query for loading pages.
        /// </summary>
        /// <typeparam name="T">The requested model type</typeparam>
        /// <returns>The queryable</returns>
        private IQueryable<Page> GetQuery<T>()
        {
            var loadRelated = !typeof(Models.IContentInfo).IsAssignableFrom(typeof(T));

            IQueryable<Page> query = _db.Pages
                .AsNoTracking()
                .Include(p => p.Permissions);

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
        /// <param name="page">The source page</param>
        /// <param name="model">The targe model</param>
        private async Task ProcessAsync<T>(Data.Page page, T model) where T : Models.PageBase
        {
            // Permissions
            foreach (var permission in page.Permissions)
            {
                model.Permissions.Add(permission.Permission);
            }

            // Comments
            model.EnableComments = page.EnableComments;
            if (model.EnableComments)
            {
                model.CommentCount = await _db.PageComments.CountAsync(c => c.PageId == model.Id).ConfigureAwait(false);
            }
            model.CloseCommentsAfterDays = page.CloseCommentsAfterDays;

            // Blocks
            if (!(model is Models.IContentInfo))
            {
                if (page.Blocks.Count > 0)
                {
                    //model.Blocks = _contentService.TransformBlocks<PageBlock>(page.Blocks.OrderBy(b => b.SortOrder));

                    foreach (var pageBlock in page.Blocks.OrderBy(b => b.SortOrder))
                    {
                        if (pageBlock.Block.ParentId.HasValue)
                        {
                            var parent = page.Blocks.FirstOrDefault(b => b.BlockId == pageBlock.Block.ParentId.Value);
                            if (parent != null)
                            {
                                pageBlock.Block.ParentId = parent.Block.Id;
                            }
                        }
                    }
                    model.Blocks = _contentService.TransformBlocks(page.Blocks.OrderBy(b => b.SortOrder).Select(b => b.Block));
                }
            }
        }

        /// <summary>
        /// Moves the pages around. This is done when a page is deleted or moved in the structure.
        /// </summary>
        /// <param name="pages">The pages</param>
        /// <param name="pageId">The id of the page that is moved</param>
        /// <param name="siteId">The site id</param>
        /// <param name="parentId">The parent id</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="increase">If sort order should be increase or decreased</param>
        private IEnumerable<Guid> MovePages(IList<Page> pages, Guid pageId, Guid siteId, Guid? parentId, int sortOrder, bool increase)
        {
            var affected = pages.Where(p => p.SortOrder >= sortOrder).ToList();

            foreach (var page in affected)
            {
                page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
            }
            return affected.Select(p => p.Id).ToList();
        }
    }
}