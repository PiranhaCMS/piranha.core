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
        /// Gets the site startpage.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param param name="siteId">The site id</param>
        /// <returns>The page model</returns>
        public async Task<T> GetStartpage<T>(Guid siteId) where T : Models.PageBase
        {
            var page = await GetQuery<T>(out var fullQuery)
                .FirstOrDefaultAsync(p => p.SiteId == siteId && p.ParentId == null && p.SortOrder == 0)
                .ConfigureAwait(false);

            if (page != null)
            {
                return _contentService.Transform<T>(page, App.PageTypes.GetById(page.PageTypeId), Process);
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
            var page = await GetQuery<T>(out var fullQuery)
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (page != null)
            {
                return _contentService.Transform<T>(page, App.PageTypes.GetById(page.PageTypeId), Process);
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
            var page = await GetQuery<T>(out var fullQuery)
                .FirstOrDefaultAsync(p => p.SiteId == siteId && p.Slug == slug)
                .ConfigureAwait(false);

            if (page != null)
            {
                return _contentService.Transform<T>(page, App.PageTypes.GetById(page.PageTypeId), Process);
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

            // Remove the old position for the page
            affected.AddRange(await MovePages(model.Id, model.SiteId, model.ParentId, model.SortOrder + 1, false).ConfigureAwait(false));
            // Add room for the new position of the page
            affected.AddRange(await MovePages(model.Id, model.SiteId, parentId, sortOrder, true).ConfigureAwait(false));

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
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        public async Task<IEnumerable<Guid>> Save<T>(T model) where T : Models.PageBase
        {
            var type = App.PageTypes.GetById(model.TypeId);
            var affected = new List<Guid>();
            var isNew = false;

            if (type != null)
            {
                // Set content type
                model.ContentType = type.ContentTypeId;

                var page = await _db.Pages
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields)
                    .FirstOrDefaultAsync(p => p.Id == model.Id)
                    .ConfigureAwait(false);

                if (page == null)
                {
                    isNew = true;
                }

                if (model.OriginalPageId.HasValue)
                {
                    var originalPageIsCopy = (await _db.Pages.FirstOrDefaultAsync(p => p.Id == model.OriginalPageId).ConfigureAwait(false))?.OriginalPageId.HasValue ?? false;
                    if (originalPageIsCopy)
                    {
                        throw new InvalidOperationException("Can not set copy of a copy");
                    }

                    var originalPageType = (await _db.Pages.FirstOrDefaultAsync(p => p.Id == model.OriginalPageId).ConfigureAwait(false))?.PageTypeId;
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
                        };

                        _db.Pages.Add(page);

                        // Make room for the new page
                        affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
                    }
                    else
                    {
                        // Check if the page has been moved
                        if (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder)
                        {
                            // Remove the old position for the page
                            affected.AddRange(await MovePages(page.Id, page.SiteId, page.ParentId, page.SortOrder + 1, false).ConfigureAwait(false));
                            // Add room for the new position of the page
                            affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true).ConfigureAwait(false));
                        }
                    }

                    if (isNew || page.Title != model.Title || page.NavigationTitle != model.NavigationTitle)
                    {
                        // If this is new page or title has been updated it means
                        // the global sitemap changes. Notify the service.
                        affected.Add(page.Id);
                    }

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

                    await _db.SaveChangesAsync().ConfigureAwait(false);

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
                    _db.Pages.Add(page);
                    model.Id = page.Id;

                    // Make room for the new page
                    affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true).ConfigureAwait(false));
                }
                else
                {
                    // Check if the page has been moved
                    if (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder)
                    {
                        // Remove the old position for the page
                        affected.AddRange(await MovePages(page.Id, page.SiteId, page.ParentId, page.SortOrder + 1, false).ConfigureAwait(false));
                        // Add room for the new position of the page
                        affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true).ConfigureAwait(false));
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

                // Transform blocks
                var blockModels = model.Blocks;

                if (blockModels != null)
                {
                    var blocks = _contentService.TransformBlocks(blockModels);
                    var current = blocks.Select(b => b.Id).ToArray();

                    // Delete removed blocks
                    var removed = page.Blocks
                        .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable)
                        .Select(b => b.Block);
                    _db.Blocks.RemoveRange(removed);

                    // Delete the old page blocks
                    page.Blocks.Clear();

                    // Now map the new block
                    for (var n = 0; n < blocks.Count; n++)
                    {
                        var block = await _db.Blocks
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
                        page.Blocks.Add(new PageBlock
                        {
                            Id = Guid.NewGuid(),
                            ParentId = blocks[n].ParentId,
                            BlockId = block.Id,
                            Block = block,
                            PageId = page.Id,
                            SortOrder = n
                        });
                    }
                }
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            return affected;
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

                // Move all remaining pages after this page in the site structure.
                affected.AddRange(await MovePages(id, model.SiteId, model.ParentId, model.SortOrder + 1, false).ConfigureAwait(false));

                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            return affected;
        }

        /// <summary>
        /// Gets the base query for loading pages.
        /// </summary>
        /// <param name="fullModel">If this is a full load or not</param>
        /// <typeparam name="T">The requested model type</typeparam>
        /// <returns>The queryable</returns>
        private IQueryable<Page> GetQuery<T>(out bool fullModel)
        {
            var loadRelated = !typeof(Models.IContentInfo).IsAssignableFrom(typeof(T));

            var query = _db.Pages
                .AsNoTracking();

            if (loadRelated)
            {
                query = query
                    .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
                    .Include(p => p.Fields);
                fullModel = true;
            }
            else
            {
                fullModel = false;
            }
            return query;
        }

        /// <summary>
        /// Performs additional processing and loads related models.
        /// </summary>
        /// <param name="page">The source page</param>
        /// <param name="model">The targe model</param>
        private void Process<T>(Data.Page page, T model) where T : Models.PageBase
        {
            if (!(model is Models.IContentInfo))
            {
                if (page.Blocks.Count > 0)
                {
                    //model.Blocks = _contentService.TransformBlocks<PageBlock>(page.Blocks.OrderBy(b => b.SortOrder));

                    foreach (var pageBlock in page.Blocks.OrderBy(b => b.SortOrder))
                    {
                        if (pageBlock.ParentId.HasValue)
                        {
                            var parent = page.Blocks.FirstOrDefault(b => b.BlockId == pageBlock.ParentId.Value);
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
        /// <param name="pageId">The id of the page that is moved</param>
        /// <param name="siteId">The site id</param>
        /// <param name="parentId">The parent id</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="increase">If sort order should be increase or decreased</param>
        private async Task<IEnumerable<Guid>> MovePages(Guid pageId, Guid siteId, Guid? parentId, int sortOrder, bool increase)
        {
            var pages = await _db.Pages
                .Where(p => p.SiteId == siteId && p.ParentId == parentId && p.SortOrder >= sortOrder && p.Id != pageId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var page in pages)
            {
                page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
            }
            return pages.Select(p => p.Id).ToList();
        }
    }
}