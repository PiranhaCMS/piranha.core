using System.Text.Json;
using Aero.Cms.Data.Data;
using Aero.Cms.Data.Services;
using Aero.Cms.Repositories;
using Marten;
using Marten.Linq;

namespace Aero.Cms.Data.Repositories;

internal class PageRepository : IPageRepository
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
    public async Task<IEnumerable<string>> GetAll(string siteId)
    {
        var pages = await _db.Pages
            .Where(p => p.SiteId == siteId)
            .OrderBy(p => p.ParentId)
            .ThenBy(p => p.SortOrder)
            .ToListAsync()
            .ConfigureAwait(false);
        return pages.Select(p => p.Id);
    }

    /// <summary>
    /// Gets the available blog pages for the current site.
    /// </summary>
    /// <param name="siteId">The site id</param>
    /// <returns>The pages</returns>
    public async Task<IEnumerable<string>> GetAllBlogs(string siteId)
    {
        var pages = await _db.Pages
            .Where(p => p.SiteId == siteId && p.ContentType == "Blog")
            .OrderBy(p => p.ParentId)
            .ThenBy(p => p.SortOrder)
            .ToListAsync()
            .ConfigureAwait(false);
        return pages.Select(p => p.Id);
    }

    /// <summary>
    /// Gets the id of all pages that have a draft for
    /// the specified site.
    /// </summary>
    /// <param name="siteId">The unique site id</param>
    /// <returns>The pages that have a draft</returns>
    public async Task<IEnumerable<string>> GetAllDrafts(string siteId)
    {
        // Query PageRevisions directly and filter in-memory since Marten can't compare two fields in SQL
        var revisions = await _db.session.Query<PageRevision>()
            .Where(r => r.SiteId == siteId)
            .ToListAsync()
            .ConfigureAwait(false);
        
        var pageIds = revisions
            .Where(r => r.Created > r.PageLastModified) // The "IsDraft" logic - filter in memory
            .Select(r => r.PageId)
            .Distinct()
            .ToList();

        return pageIds;
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
    public Task<IEnumerable<Models.Comment>> GetAllComments(string pageId, bool onlyApproved,
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
    public Task<IEnumerable<Models.Comment>> GetAllPendingComments(string pageId,
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
    public async Task<T> GetStartpage<T>(string siteId) where T : Models.PageBase
    {
        var page = await GetQuery<T>()

            .FirstOrDefaultAsync(p => p.SiteId == siteId && (p.ParentId == null || p.ParentId == "") && p.SortOrder == 0)
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
    public async Task<T> GetById<T>(string id) where T : Models.PageBase
    {
        // Use session.LoadAsync for ID-based lookups — direct storage engine fetch, no index overhead
        var page = await _db.session.LoadAsync<Page>(id).ConfigureAwait(false);

        if (page != null)
        {
            var model = await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageTypeId), ProcessAsync);

            if (model != null && !string.IsNullOrEmpty(page.OriginalPageId))
            {
                var originalPage = await _db.session.LoadAsync<Page>(page.OriginalPageId).ConfigureAwait(false);
                if (originalPage != null)
                {
                    var originalModel = await _contentService.TransformAsync<T>(originalPage, App.PageTypes.GetById(originalPage.PageTypeId), ProcessAsync);

                    if (originalModel != null)
                    {
                        if (model is Models.IDynamicContent dynamicModel && originalModel is Models.IDynamicContent dynamicOriginalModel)
                        {
                            var modelRegions = (IDictionary<string, object>)dynamicModel.Regions;
                            var originalRegions = (IDictionary<string, object>)dynamicOriginalModel.Regions;

                            // Merge regions from original page if not present in the copy
                            foreach (var region in originalRegions)
                            {
                                if (modelRegions.ContainsKey(region.Key))
                                {
                                    var modelValue = modelRegions[region.Key];
                                    if (modelValue == null || (modelValue is Aero.Cms.Extend.IField field && field.GetType().GetProperty("Value")?.GetValue(field) == null))
                                    {
                                        modelRegions[region.Key] = region.Value;
                                    }
                                }
                                else
                                {
                                    modelRegions.Add(region.Key, region.Value);
                                }
                            }
                        }
                        else
                        {
                            // Copy regions from original model for strongly typed pages
                            var pageType = App.PageTypes.GetById(page.PageTypeId);
                            if (pageType != null)
                            {
                                foreach (var regionType in pageType.Regions)
                                {
                                    var prop = model.GetType().GetProperty(regionType.Id, App.PropertyBindings);
                                    if (prop != null)
                                    {
                                        var modelValue = prop.GetValue(model);
                                        if (modelValue == null || (modelValue is Aero.Cms.Extend.IField field && field.GetType().GetProperty("Value")?.GetValue(field) == null))
                                        {
                                            var originalProp = originalModel.GetType().GetProperty(regionType.Id, App.PropertyBindings);
                                            if (originalProp != null)
                                            {
                                                prop.SetValue(model, originalProp.GetValue(originalModel));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (model.Blocks.Count == 0)
                        {
                            model.Blocks = originalModel.Blocks;
                        }
                    }
                }
            }
            return model;
        }

        return null;
    }

    /// <summary>
    /// Gets the page models with the specified id's.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="ids">The unique id's</param>
    /// <returns>The page models</returns>
    public async Task<IEnumerable<T>> GetByIds<T>(params string[] ids) where T : Models.PageBase
    {
        var ret = new List<T>();
        var pages = await GetQuery<T>()
            .Where(p => p.Id.In(ids))
            .ToListAsync()
            .ConfigureAwait(false);

        // todo - consider using session.LoadAsync for each ID if the number of IDs is small, to avoid index overhead; or use session.LoadAsync with multiple IDs if supported by RavenDB client
        //var p = await _db.session.LoadAsync<T>(ids);

        foreach (var page in pages)
        {
            ret.Add(await GetById<T>(page.Id).ConfigureAwait(false));
        }

        return ret;
    }

    /// <summary>
    /// Gets the page model with the specified slug.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="slug">The unique slug</param>
    /// <param name="siteId">The site id</param>
    /// <returns>The page model</returns>
    public async Task<T> GetBySlug<T>(string slug, string siteId) where T : Models.PageBase
    {
        var page = await GetQuery<T>()
            
            .FirstOrDefaultAsync(p => p.SiteId == siteId && p.Slug.ToLower() == slug.ToLower())
            .ConfigureAwait(false);

        if (page != null)
        {
            return await GetById<T>(page.Id).ConfigureAwait(false);
        }

        return null;
    }

    /// <summary>
    /// Gets the draft for the page model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <returns>The draft, or null if no draft exists</returns>
    public async Task<T> GetDraftById<T>(string id) where T : Models.PageBase
    {
        var page = await _db.Pages
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        DateTime? lastModified = page?.LastModified;

        if (lastModified.HasValue)
        {
            var draft = await _db.PageRevisions
                
                .FirstOrDefaultAsync(r => r.PageId == id && r.Created > lastModified)
                .ConfigureAwait(false);

            if (draft != null)
            {
                // Transform data model
                var pageData = JsonSerializer.Deserialize<Page>(draft.Data);

                return await _contentService.TransformAsync<T>(pageData, App.PageTypes.GetById(pageData.PageTypeId),
                    ProcessAsync);
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
    public async Task<IEnumerable<string>> Move<T>(T model, string? parentId, int sortOrder) where T : Models.PageBase
    {
        var affected = new List<string>();

        var source = await _db.Pages
            .Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId && p.Id != model.Id).ToListAsync()
            .ConfigureAwait(false);
        var dest = model.ParentId == parentId
            ? source
            : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == parentId).ToListAsync()
                .ConfigureAwait(false);

        // Remove the old position for the page
        affected.AddRange(MovePages(source, model.Id, model.SiteId, model.SortOrder + 1, false));
        // Add room for the new position of the page
        affected.AddRange(MovePages(dest, model.Id, model.SiteId, sortOrder, true));

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
    public async Task<Models.Comment> GetCommentById(string id)
    {
        var comments = await _db.PageComments
            .Where(c => c.Id == id)
            .ToListAsync()
            .ConfigureAwait(false);
        return comments.Select(c => new Models.PageComment
        {
            Id = c.Id,
            ContentId = c.PageId,
            UserId = c.UserId,
            Author = c.Author,
            Email = c.Email,
            Url = c.Url,
            IsApproved = c.IsApproved,
            Body = c.Body,
            Created = c.Created.DateTime
        }).FirstOrDefault();
    }

    /// <summary>
    /// Saves the given page model
    /// </summary>
    /// <param name="model">The page model</param>
    /// <returns>The other pages that were affected by the move</returns>
    public Task<IEnumerable<string>> Save<T>(T model) where T : Models.PageBase
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
    public async Task SaveComment(string pageId, Models.Comment model)
    {
        var comment = await _db.PageComments
            .FirstOrDefaultAsync(c => c.Id == model.Id)
            .ConfigureAwait(false);

        if (comment == null)
        {
            comment = new PageComment
            {
                Id = model.Id
            };
            //await _db.PageComments.AddAsync(comment);
            _db.session.Store(comment);
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
    public async Task CreateRevision(string id, int revisions)
    {
        // Use session.LoadAsync for ID-based lookup
        var page = await _db.session.LoadAsync<Page>(id).ConfigureAwait(false);

        if (page != null)
        {
            _db.session.Store(new PageRevision
            {
                Id = Snowflake.NewId(),
                PageId = id,
                // Populate denormalized fields so GetAllDrafts can filter without cross-collection navigation
                SiteId = page.SiteId,
                PageLastModified = page.LastModified,
                Data = JsonSerializer.Serialize(page),
                Created = page.LastModified
            });

            await _db.SaveChangesAsync().ConfigureAwait(false);

            // Trim revisions to the configured maximum
            if (revisions != 0)
            {
                var existing = await _db.PageRevisions
                    .Where(r => r.PageId == id)
                    .OrderByDescending(r => r.Created)
                    .Select(r => r.Id)
                    .Take(revisions)
                    .ToListAsync<string>()
                    .ConfigureAwait(false);

                if (existing.Count == revisions)
                {
                    var removed = await _db.PageRevisions
                        .Where(r => r.PageId == id && !r.Id.In(existing.ToArray()))
                        .ToListAsync()
                        .ConfigureAwait(false);

                    // Fix: session.Delete() only accepts a single entity or ID — iterate to delete each
                    foreach (var rev in removed)
                    {
                        _db.session.Delete<PageRevision>(rev.Id);
                    }
                    if (removed.Count > 0)
                    {
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
    /// <returns>The other pages that were affected by the move</returns>
    public async Task<IEnumerable<string>> Delete(string id)
    {
        // Use session.LoadAsync for ID-based lookup
        var model = await _db.session.LoadAsync<Page>(id).ConfigureAwait(false);
        var affected = new List<string>();

        if (model != null)
        {
            // Delete all copies first (pages that reference this as OriginalPageId)
            var copies = await _db.Pages
                .Where(p => p.OriginalPageId == model.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var copy in copies)
            {
                await Delete(copy.Id).ConfigureAwait(false);
            }

            // Make sure this page doesn't have child pages
            var childCount = await _db.Pages.CountAsync(p => p.ParentId == model.Id).ConfigureAwait(false);
            // Reusable blocks live in the Blocks collection and must be kept;
            // non-reusable block data is embedded in the Page document and is deleted with it.
            // Only separately-stored reusable blocks need no explicit action here.

            // Delete all revisions for this page
            var revisions = await _db.PageRevisions
                .Where(r => r.PageId == id)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var rev in revisions)
            {
                _db.session.Delete<PageRevision>(rev.Id);
            }

            // Remove the main page — embedded blocks/fields/permissions are deleted with it
            _db.session.Delete<Page>(model.Id);

            var siblings = await _db.Pages
                .Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId)
                .ToListAsync()
                .ConfigureAwait(false);

            // Move all remaining pages after this page in the site structure
            affected.AddRange(MovePages(siblings, id, model.SiteId, model.SortOrder + 1, false));

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        return affected;
    }

    /// <summary>
    /// Deletes the current draft revision for the page
    /// with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task DeleteDraft(string id)
    {
        // Use session.LoadAsync for ID-based lookup
        var page = await _db.session.LoadAsync<Page>(id).ConfigureAwait(false);

        if (page != null)
        {
            // Query all revisions for this page and filter in memory
            var allRevisions = await _db.session
                .Query<PageRevision>()
                .Where(r => r.PageId == id)
                .ToListAsync()
                .ConfigureAwait(false);
            
            var draftRevisionIds = allRevisions
                .Where(r => r.Created > page.LastModified) // IsDraft logic
                .Select(r => r.Id)
                .ToList();

            foreach (var draftId in draftRevisionIds)
            {
                _db.session.Delete<PageRevision>(draftId);
            }
            if (draftRevisionIds.Count > 0)
            {
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Deletes the comment with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task DeleteComment(string id)
    {
        var comment = await _db.PageComments
            .FirstOrDefaultAsync(c => c.Id == id)
            .ConfigureAwait(false);

        if (comment != null)
        {
            //_db.PageComments.Remove(comment);
            _db.session.Delete(comment);

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
    public async Task<IEnumerable<Models.Comment>> GetAllComments(string? pageId, bool onlyApproved,
        bool onlyPending, int page, int pageSize)
    {
        // Create base query
        System.Linq.IQueryable<PageComment> query = _db.PageComments
            ;

        // Check if only should include a comments for a certain post
        if (!string.IsNullOrEmpty(pageId))
        {
            query = query.Where(c => c.PageId == pageId);
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
        var comments = await query
            .ToListAsync()
            .ConfigureAwait(false);
        
        return comments.Select(c => new Models.PageComment
        {
            Id = c.Id,
            ContentId = c.PageId,
            UserId = c.UserId,
            Author = c.Author,
            Email = c.Email,
            Url = c.Url,
            IsApproved = c.IsApproved,
            Body = c.Body,
            Created = c.Created.DateTime
        });
    }

    /// <summary>
    /// Saves the given page model
    /// </summary>
    /// <param name="model">The page model</param>
    /// <param name="isDraft">If the model should be saved as a draft</param>
    private async Task<IEnumerable<string>> Save<T>(T model, bool isDraft) where T : Models.PageBase
    {
        // Clone the model when saving as draft to avoid modifying the original object
        if (isDraft)
        {
            var json = JsonSerializer.Serialize(model);
            model = JsonSerializer.Deserialize<T>(json);
        }

        var type = App.PageTypes.GetById(model.TypeId);
        var affected = new List<string>();
        var isNew = false;
        var lastModified = DateTime.MinValue;

        if (type != null)
        {
            IMartenQueryable<Page> pageQuery = _db.Pages;
            // Marten doesn't have NoTracking - skip this
            // if (isDraft)
            // {
            //     pageQuery = pageQuery.Customize(x => x.NoTracking());
            // }

            var page = await pageQuery
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                .ConfigureAwait(false);

            // Clone the page entity when saving as draft to avoid modifying the original in DB
            if (isDraft && page != null)
            {
                var json = JsonSerializer.Serialize(page);
                page = JsonSerializer.Deserialize<Page>(json);
            }

            if (page == null)
            {
                isNew = true;
            }
            else
            {
                lastModified = page.LastModified;
            }

            if (!string.IsNullOrEmpty(model.OriginalPageId))
            {
                // Fix: use session.LoadAsync with the correct original page ID (not a random page)
                var originalPage = await _db.session.LoadAsync<Page>(model.OriginalPageId).ConfigureAwait(false);
                if (originalPage != null && !string.IsNullOrEmpty(originalPage.OriginalPageId))
                {
                    throw new InvalidOperationException("Can not set copy of a copy");
                }

                if (originalPage != null && originalPage.PageTypeId != model.TypeId)
                {
                    throw new InvalidOperationException("Copy can not have a different content type");
                }

                // Transform the model
                if (page == null)
                {
                    page = new Page()
                    {
                        Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId().ToString(),
                        Created = DateTime.Now,
                    };

                    if (!isDraft)
                    {
                        //await _db.Pages.AddAsync(page).ConfigureAwait(false);
                        _db.session.Store(page);

                        // Make room for the new page
                        var dest = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId)
                            .ToListAsync()
                            .ConfigureAwait(false);
                        affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
                    }
                }
                else
                {
                    // Check if the page has been moved
                    if (!isDraft && (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder))
                    {
                        var source = await _db.Pages.Where(p =>
                            p.SiteId == page.SiteId && p.ParentId == page.ParentId && p.Id != model.Id).ToListAsync()
                            .ConfigureAwait(false);
                        var dest = page.ParentId == model.ParentId
                            ? source
                            : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId)
                                .ToListAsync()
                                .ConfigureAwait(false);

                        // Remove the old position for the page
                        affected.AddRange(MovePages(source, page.Id, page.SiteId, page.SortOrder + 1, false));
                        // Add room for the new position of the page
                        affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
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
                            Id = Snowflake.NewId(),
                            PageId = page.Id
                        };
                        // await _db.PageRevisions
                        //     .AddAsync(draft)
                        //     .ConfigureAwait(false);
                        _db.session.Store(draft);
                    }

                    draft.Data = JsonSerializer.Serialize(page);
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
                    Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId().ToString(),
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder,
                    PageTypeId = model.TypeId,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };
                model.Id = page.Id;

                if (!isDraft)
                {
                    //await _db.Pages.AddAsync(page).ConfigureAwait(false);
                    _db.session.Store(page);

                    // Make room for the new page
                    var dest = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
                }
            }
            else
            {
                // Check if the page has been moved
                if (!isDraft && (page.ParentId != model.ParentId || page.SortOrder != model.SortOrder))
                {
                    var source = await _db.Pages
                        .Where(p => p.SiteId == page.SiteId && p.ParentId == page.ParentId && p.Id != model.Id)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    var dest = page.ParentId == model.ParentId
                        ? source
                        : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId)
                            .ToListAsync()
                            .ConfigureAwait(false);

                    // Remove the old position for the page
                    affected.AddRange(MovePages(source, page.Id, page.SiteId, page.SortOrder + 1, false));
                    // Add room for the new position of the page
                    affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
                }

                page.LastModified = DateTime.Now;
            }

            if (isNew || page.Title != model.Title || page.NavigationTitle != model.NavigationTitle)
            {
                // If this is new page or title has been updated it means
                // the global sitemap changes. Notify the service.
                affected.Add(page.Id);
            }

            page.Title = model.Title;
            page.NavigationTitle = model.NavigationTitle;
            page.Slug = model.Slug;
            page.ParentId = model.ParentId;
            page.SortOrder = model.SortOrder;
            page.IsHidden = model.IsHidden;
            page.Route = model.Route;
            page.Published = model.Published;
            page.OriginalPageId = model.OriginalPageId;

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

            // Set the foreign key on fields — they are embedded in the Page document,
            // so no separate session.StoreAsync is needed; the page.Fields list is
            // serialized inline when the Page document is saved.
            foreach (var field in page.Fields)
            {
                if (string.IsNullOrEmpty(field.PageId))
                {
                    field.PageId = page.Id;
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
                    .Where(b => !current.Contains(b.BlockId) && b.Block.ParentId != null &&
                                removed.Select(p => p.Id).ToList().Contains(b.Block.ParentId))
                    .Select(b => b.Block);

                if (!isDraft)
                {
                    // _db.Blocks.RemoveRange(removed);
                    // _db.Blocks.RemoveRange(removedItems);
                    foreach (var item in removed)
                        _db.session.Delete(item);
                    foreach (var item in removedItems)
                        _db.session.Delete(removedItems);
                }

                // Delete the old page blocks
                page.Blocks.Clear();

                // Now map the new block
                for (var n = 0; n < blocks.Count; n++)
                {
                    var blockQuery = _db.Blocks;
                    // Marten doesn't have Customize/NoTracking
                    // if (isDraft)
                    // {
                    //     blockQuery = blockQuery.Customize(x => x.NoTracking());
                    // }

                    var id = blocks[n].Id;
                    // Marten Include has different syntax - skip for now
                    var block = await blockQuery
                        .FirstOrDefaultAsync(b => b.Id == id)
                        .ConfigureAwait(false);

                    if (block == null)
                    {
                        block = new Block
                        {
                            Id = !string.IsNullOrEmpty(blocks[n].Id) ? blocks[n].Id : Snowflake.NewId(),
                            Created = DateTime.Now
                        };
                        if (!isDraft)
                        {
                            //await _db.Blocks.AddAsync(block).ConfigureAwait(false);
                            _db.session.Store(block);
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
                        //_db.BlockFields.RemoveRange(removedFields);
                        foreach (var field in removedFields)
                            _db.session.Delete(field);
                    }

                    foreach (var newField in blocks[n].Fields)
                    {
                        var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                        if (field == null)
                        {
                            field = new BlockField
                            {
                                Id = newField.Id != string.Empty ? newField.Id : Snowflake.NewId(),
                                BlockId = block.Id,
                                FieldId = newField.FieldId
                            };
                            if (!isDraft)
                            {
                                //await _db.BlockFields.AddAsync(field).ConfigureAwait(false);
                                _db.session.Store(field);
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
                        Id = Snowflake.NewId(),
                        BlockId = block.Id,
                        Block = block,
                        PageId = page.Id,
                        SortOrder = n
                    };
                    if (!isDraft)
                    {
                        //await _db.PageBlocks.AddAsync(pageBlock).ConfigureAwait(false);
                        _db.session.Store(pageBlock);
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
                        Id = Snowflake.NewId(),
                        PageId = page.Id
                    };
                    // await _db.PageRevisions
                    //     .AddAsync(draft)
                    //     .ConfigureAwait(false);
                    _db.session.Store(draft);
                }

                draft.Data = JsonSerializer.Serialize(page);
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
    private IMartenQueryable<Page> GetQuery<T>()
    {
        var loadRelated = !typeof(Models.IContentInfo).IsAssignableFrom(typeof(T));

        var query = _db.Pages
            ;

        // FirstOrDefaultAsync(p => p.Id ...
        query = (IMartenQueryable<Page>)query.OrderBy(p => p.Id);

        //if (loadRelated)
        {
            // query = query;
        }

        return query;
    }

    /// <summary>
    /// Performs additional processing and loads related models.
    /// </summary>
    /// <param name="page">The source page</param>
    /// <param name="model">The targe model</param>
    private async Task ProcessAsync<T>(Page page, T model) where T : Models.PageBase
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
            model.CommentCount = await _db.PageComments
                .CountAsync(c => c.PageId == model.Id && c.IsApproved)
                .ConfigureAwait(false);
        }

        model.CloseCommentsAfterDays = page.CloseCommentsAfterDays;

        // Blocks
        if (!(model is Models.IContentInfo))
        {
            if (page.Blocks.Count > 0)
            {
                foreach (var pageBlock in page.Blocks.OrderBy(b => b.SortOrder))
                {
                    if (!string.IsNullOrEmpty(pageBlock.Block.ParentId))
                    {
                        var parent = page.Blocks.FirstOrDefault(b => b.BlockId == pageBlock.Block.ParentId);
                        if (parent != null)
                        {
                            pageBlock.Block.ParentId = parent.Block.Id;
                        }
                    }
                }

                model.Blocks =
                    _contentService.TransformBlocks(page.Blocks.OrderBy(b => b.SortOrder).Select(b => b.Block));
            }
        }
    }

    /// <summary>
    /// Moves the pages around. This is done when a page is deleted or moved in the structure.
    /// </summary>
    /// <param name="pages">The pages</param>
    /// <param name="pageId">The id of the page that is moved</param>
    /// <param name="siteId">The site id</param>
    /// <param name="sortOrder">The sort order</param>
    /// <param name="increase">If sort order should be increase or decreased</param>
    private IEnumerable<string> MovePages(IReadOnlyList<Page> pages, string pageId, string siteId, int sortOrder, bool increase)
    {
        var affected = pages.Where(p => p.SortOrder >= sortOrder).ToList();

        foreach (var page in affected)
        {
            page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
        }

        return affected.Select(p => p.Id).ToList();
    }
}
