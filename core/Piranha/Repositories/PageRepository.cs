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
        /// Creates and initializes a new page of the specified type.
        /// </summary>
        /// <returns>The created page</returns>
        public Task<T> Create<T>(string typeId = null) where T : Models.PageBase
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(typeId))
                {
                    typeId = typeof(T).Name;
                }
                return _contentService.Create<T>(App.PageTypes.GetById(typeId));
            });
        }

        /// <summary>
        /// Creates and initializes a copy of the given page.
        /// </summary>
        /// <returns>The created copy</returns>
        public Task<T> Copy<T>(T originalPage) where T : Models.PageBase
        {
            return Task.Run(() =>
            {
                var model = _contentService.Create<T>(App.PageTypes.GetById(originalPage.TypeId));
                model.OriginalPageId = originalPage.Id;
                model.Slug = null;
                return model;
            });
        }

        /// <summary>
        /// Gets all of the available pages for the current site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The pages</returns>
        public async Task<IEnumerable<T>> GetAll<T>(Guid siteId) where T : Models.PageBase
        {
            var pages = await _db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == siteId)
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .Select(p => p.Id)
                .ToListAsync();

            var models = new List<T>();

            foreach (var page in pages)
            {
                var model = await GetById<T>(page);

                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// Gets the available blog pages for the current site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The pages</returns>
        public async Task<IEnumerable<T>> GetAllBlogs<T>(Guid siteId) where T : Models.PageBase
        {
            var pages = await _db.Pages
                .AsNoTracking()
                .Where(p => p.SiteId == siteId && p.ContentType == "Blog")
                .OrderBy(p => p.ParentId)
                .ThenBy(p => p.SortOrder)
                .Select(p => p.Id)
                .ToListAsync();

            var models = new List<T>();

            foreach (var page in pages)
            {
                var model = await GetById<T>(page);

                if (model != null)
                {
                    models.Add(model);
                }
            }
            return models;
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
                .FirstOrDefaultAsync(p => p.SiteId == siteId && p.ParentId == null && p.SortOrder == 0);

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
                .FirstOrDefaultAsync(p => p.Id == id);

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
                .FirstOrDefaultAsync(p => p.SiteId == siteId && p.Slug == slug);

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
            affected.AddRange(await MovePages(model.Id, model.SiteId, model.ParentId, model.SortOrder + 1, false));
            // Add room for the new position of the page
            affected.AddRange(await MovePages(model.Id, model.SiteId, parentId, sortOrder, true));

            // Update the position of the current page
            var page = await _db.Pages
                .FirstOrDefaultAsync(p => p.Id == model.Id);
            page.ParentId = parentId;
            page.SortOrder = sortOrder;

            await _db.SaveChangesAsync();

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
                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (page == null)
                {
                    isNew = true;
                }

                if (model.OriginalPageId.HasValue)
                {
                    var originalPageIsCopy = (await _db.Pages.FirstOrDefaultAsync(p => p.Id == model.OriginalPageId))?.OriginalPageId.HasValue ?? false;
                    if (originalPageIsCopy)
                    {
                        throw new InvalidOperationException("Can not set copy of a copy");
                    }

                    var originalPageType = (await _db.Pages.FirstOrDefaultAsync(p => p.Id == model.OriginalPageId))?.PageTypeId;
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
                            affected.AddRange(await MovePages(page.Id, page.SiteId, page.ParentId, page.SortOrder + 1, false));
                            // Add room for the new position of the page
                            affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
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

                    await _db.SaveChangesAsync();

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
                    affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
                }
                else
                {
                    // Check if the page has been moved
                    if (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder)
                    {
                        // Remove the old position for the page
                        affected.AddRange(await MovePages(page.Id, page.SiteId, page.ParentId, page.SortOrder + 1, false));
                        // Add room for the new position of the page
                        affected.AddRange(await MovePages(page.Id, model.SiteId, model.ParentId, model.SortOrder, true));
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
                    var pageBlocks = _contentService.TransformBlocks<PageBlock>(blockModels);

                    var current = pageBlocks.Select(b => b.Block.Id).ToArray();

                    // Delete removed blocks
                    var removed = page.Blocks
                        .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable)
                        .Select(b => b.Block);
                    _db.Blocks.RemoveRange(removed);

                    // Delete the old page blocks
                    page.Blocks.Clear();

                    // Now map the new block
                    for (var n = 0; n < pageBlocks.Count; n++)
                    {
                        var block = await _db.Blocks
                            .Include(b => b.Fields)
                            .FirstOrDefaultAsync(b => b.Id == pageBlocks[n].Block.Id);
                        if (block == null)
                        {
                            block = new Block
                            {
                                Id = pageBlocks[n].Block.Id != Guid.Empty ? pageBlocks[n].Block.Id : Guid.NewGuid(),
                                Created = DateTime.Now
                            };
                            await _db.Blocks.AddAsync(block);
                        }
                        block.CLRType = pageBlocks[n].Block.CLRType;
                        block.IsReusable = pageBlocks[n].Block.IsReusable;
                        block.Title = pageBlocks[n].Block.Title;
                        block.LastModified = DateTime.Now;

                        var currentFields = pageBlocks[n].Block.Fields.Select(f => f.FieldId).Distinct();
                        var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));
                        _db.BlockFields.RemoveRange(removedFields);

                        foreach (var newField in pageBlocks[n].Block.Fields)
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
                                await _db.BlockFields.AddAsync(field);
                                block.Fields.Add(field);
                            }
                            field.SortOrder = newField.SortOrder;
                            field.CLRType = newField.CLRType;
                            field.Value = newField.Value;
                        }

                        // Create the page block
                        page.Blocks.Add(new PageBlock
                        {
                            Id = pageBlocks[n].Id,
                            ParentId = pageBlocks[n].ParentId,
                            BlockId = block.Id,
                            Block = block,
                            PageId = page.Id,
                            SortOrder = n
                        });
                    }
                }
                await _db.SaveChangesAsync();
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
                .FirstOrDefaultAsync(p => p.Id == id);
            var affected = new List<Guid>();

            if (model != null)
            {
                // Make sure this page isn't copied
                var copyCount = await _db.Pages.CountAsync(p => p.OriginalPageId == model.Id);
                if (copyCount > 0)
                {
                    throw new InvalidOperationException("Can not delete page because it has copies");
                }

                // Make sure this page doesn't have child pages
                var childCount = await _db.Pages.CountAsync(p => p.ParentId == model.Id);
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
                affected.AddRange(await MovePages(id, model.SiteId, model.ParentId, model.SortOrder + 1, false));

                await _db.SaveChangesAsync();
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
                    model.Blocks = _contentService.TransformBlocks<PageBlock>(page.Blocks.OrderBy(b => b.SortOrder));
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
                .ToListAsync();

            foreach (var page in pages)
            {
                page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
            }
            return pages.Select(p => p.Id).ToList();
        }
    }
}
