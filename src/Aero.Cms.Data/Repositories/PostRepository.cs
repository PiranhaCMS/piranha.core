using Aero.Cms.Repositories;


using System.Text.Json;
using Aero.Cms.Data.Data;

using Aero.Cms.Data.Services;
using Marten;
using Marten.Linq;

namespace Aero.Cms.Data.Repositories;

internal class PostRepository : IPostRepository
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
    public async Task<IEnumerable<string>> GetAll(string blogId, int? index = null, int? pageSize = null)
    {
        // Prepare base query
        var query = _db.Posts
            .Where(p => p.BlogId == blogId)
            .OrderByDescending(p => p.Published)
            .ThenByDescending(p => p.LastModified)
            .ThenBy(p => p.Title);

        // Add paging if requested
        if (index.HasValue && pageSize.HasValue)
        {
            query = (IOrderedQueryable<Post>)query
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
    public async Task<IEnumerable<string>> GetAllBySiteId(string siteId)
    {
        //return await _db.Posts
        //    .Where(p => p.Blog.SiteId == siteId)
        //    .OrderByDescending(p => p.Published)
        //    .ThenByDescending(p => p.LastModified)
        //    .ThenBy(p => p.Title)
        //    .Select(p => p.Id)
        //    .ToListAsync()
        //    .ConfigureAwait(false);

        var posts = await _db.Posts
            .Where(p => p.SiteId == siteId)
            .OrderByDescending(p => p.Published)
            .ThenByDescending(p => p.LastModified)
            .ThenBy(p => p.Title)
            .Select(p => p.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        return posts;
    }

    /// <summary>
    /// Gets all available categories for the specified blog.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <returns>The available categories</returns>
    public async Task<IEnumerable<Models.Taxonomy>> GetAllCategories(string blogId)
    {
        return await _db.Categories
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
    public async Task<IEnumerable<Models.Taxonomy>> GetAllTags(string blogId)
    {
        var tags = await _db.Tags
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

        return tags;
    }

    /// <summary>
    /// Gets the id of all posts that have a draft for
    /// the specified blog.
    /// </summary>
    /// <param name="blogId">The unique blog id</param>
    /// <returns>The posts that have a draft</returns>
    public async Task<IEnumerable<string>> GetAllDrafts(string blogId)
    {
        // Use the PostRevisions_ByBlog index which precomputes IsDraft = Created > PostLastModified.
        // RavenDB LINQ cannot compare two document fields in a Where clause at query time.
        //var postIds = await _db.session
        //    .Query<PostRevisions_ByBlog.Result, PostRevisions_ByBlog>()
        //    .Where(r => r.BlogId == blogId && r.IsDraft)
        //    .Select(r => r.PostId)
        //    .Distinct()
        //    .ToListAsync()
        //    .ConfigureAwait(false);
        var postIds = await _db.session.Query<PostRevision>()
            .Where(x => x.BlogId == blogId)
            // No need for a pre-computed index field!
            .Where(x => x.Created > x.PostLastModified)
            .Select(r => 
                //r.BlogId,
                r.PostId
                //r.Id,
                //r.Created,
                //r.PostLastModified,
                //IsDraft = r.Created > r.PostLastModified
            )
            .ToListAsync();

        return postIds;
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
    public Task<IEnumerable<Models.Comment>> GetAllComments(string postId, bool onlyApproved,
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
    public Task<IEnumerable<Models.Comment>> GetAllPendingComments(string postId,
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
    public async Task<T> GetById<T>(string id) where T : Models.PostBase
    {
        // Use session.LoadAsync for ID-based lookups — direct storage engine fetch, no index overhead
        var post = await _db.session.LoadAsync<Post>(id).ConfigureAwait(false);

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
    public async Task<T> GetBySlug<T>(string blogId, string slug) where T : Models.PostBase
    {
        // No cache found, load from database
        Console.WriteLine($"[DEBUG] GetBySlug: blogId={blogId}, slug={slug}, session={_db.session.GetHashCode()}");
        
        var query = GetQuery<T>();
        var post = await query
            
            .FirstOrDefaultAsync(p => p.BlogId == blogId && p.Slug == slug)
            .ConfigureAwait(false);
        
        Console.WriteLine($"[DEBUG] GetBySlug: post found = {post != null}");

        if (post == null)
        {
            Console.WriteLine($"[DEBUG] GetBySlug: Database: session document store, session={_db.session.GetHashCode()}");
            // Debug: list all posts in the whole database
            var allPostsGlobal = await _db.session.Query<Post>().ToListAsync();
            Console.WriteLine($"[DEBUG] GetBySlug: GLOBAL posts count: {allPostsGlobal.Count}");
            foreach (var p in allPostsGlobal)
            {
                Console.WriteLine($"[DEBUG]   GLOBAL Post: Id={p.Id}, Slug={p.Slug}, BlogId='{p.BlogId}'");
            }

            // Debug: list all posts for this blog
            var allPosts = await query.Where(p => p.BlogId == blogId).ToListAsync();
            Console.WriteLine($"[DEBUG] GetBySlug: All posts for blog {blogId}: {allPosts.Count}");
            foreach (var p in allPosts)
            {
                Console.WriteLine($"[DEBUG]   Post: Id={p.Id}, Slug={p.Slug}");
            }
        }

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
    public async Task<T> GetDraftById<T>(string id) where T : Models.PostBase
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
                var post = JsonSerializer.Deserialize<Post>(draft.Data);

                return await _contentService.TransformAsync<T>(post, App.PostTypes.GetById(post.PostTypeId),
                    ProcessAsync);
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the number of available posts in the specified archive.
    /// </summary>
    /// <param name="archiveId">The archive id</param>
    /// <returns>The number of posts</returns>
    public Task<int> GetCount(string archiveId)
    {
        return _db.Posts
            .Where(p => p.BlogId == archiveId)
            .CountAsync();
    }

    /// <summary>
    /// Gets the category with the id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model</returns>
    public Task<Models.Taxonomy> GetCategoryById(string id)
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
    /// Gets the category with the given slug.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <param name="slug">The unique slug</param>
    /// <returns>The category</returns>
    public Task<Models.Taxonomy> GetCategoryBySlug(string blogId, string slug)
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
    /// Gets the tag with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The category</returns>
    public Task<Models.Taxonomy> GetTagById(string id)
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
    /// Gets the tag with the given slug.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    /// <param name="slug">The unique slug</param>
    /// <returns>The tag</returns>
    public Task<Models.Taxonomy> GetTagBySlug(string blogId, string slug)
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
    /// Gets the comment with the given id.
    /// </summary>
    /// <param name="id">The comment id</param>
    /// <returns>The model</returns>
    public async Task<Models.Comment> GetCommentById(string id)
    {
        return await _db.PostComments
            .Where(c => c.Id == id)
            .Select(c => new Models.PostComment
            {
                Id = c.Id,
                ContentId = c.PostId,
                UserId = c.UserId,
                Author = c.Author,
                Email = c.Email,
                Url = c.Url,
                IsApproved = c.IsApproved,
                Body = c.Body,
                Created = c.Created.DateTime
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
    public async Task SaveComment(string postId, Models.Comment model)
    {
        var comment = await _db.PostComments
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (comment == null)
        {
            comment = new Data.PostComment
            {
                Id = model.Id
            };
            //await _db.PostComments.AddAsync(comment);
            _db.session.Store(comment);
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
    public async Task CreateRevision(string id, int revisions)
    {
        // Use session.LoadAsync for ID-based lookup
        var post = await _db.session.LoadAsync<Post>(id).ConfigureAwait(false);

        if (post != null)
        {
            _db.session.Store(new PostRevision
            {
                Id = Snowflake.NewId(),
                PostId = id,
                // Populate denormalized fields so GetAllDrafts can filter without cross-collection navigation
                BlogId = post.BlogId,
                PostLastModified = post.LastModified,
                Data = JsonSerializer.Serialize(post),
                Created = post.LastModified
            });

            await _db.SaveChangesAsync().ConfigureAwait(false);

            // Trim revisions to the configured maximum
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

                    // Fix: session.Delete() only accepts a single entity or ID — iterate to delete each
                    foreach (var rev in removed)
                    {
                        _db.session.Delete(rev.Id);
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
    public async Task Delete(string id)
    {
        var model = await _db.session.LoadAsync<Post>(id).ConfigureAwait(false);

        if (model != null)
        {
            // Blocks are embedded in the Post document, so deleting the Post removes them.
            // We do not need to explicitly delete non-reusable blocks here.
            
            //_db.Posts.Remove(model);
            _db.session.Delete(model);


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
    public async Task DeleteDraft(string id)
    {
        // Use session.LoadAsync for ID-based lookup
        var post = await _db.session.LoadAsync<Post>(id).ConfigureAwait(false);

        if (post != null)
        {
            // Use PostRevisions_ByBlog index to find drafts — avoids field-to-field date comparison
            var draftRevisionIds = await _db.session.Query<PostRevision>()
                .Where(x => x.BlogId == "blogs/1")
                // No need for a pre-computed index field!
                .Where(x => x.Created > x.PostLastModified)
                .Select(r => new {
                    r.BlogId,
                    r.PostId,
                    r.Id,
                    r.Created,
                    r.PostLastModified,
                    IsDraft = r.Created > r.PostLastModified
                })
                .ToListAsync();

            foreach (var draftId in draftRevisionIds)
            {
                _db.session.Delete(draftId);
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
        var comment = await _db.PostComments
            .FirstOrDefaultAsync(c => c.Id == id)
            .ConfigureAwait(false);

        if (comment != null)
        {
            //_db.PostComments.Remove(comment);
            _db.session.Delete(comment);

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
    public async Task<IEnumerable<Models.Comment>> GetAllComments(string? postId, bool onlyApproved,
        bool onlyPending, int page, int pageSize)
    {
        // Create base query
        IQueryable<PostComment> query = _db.PostComments;
            ;

        // Check if only should include a comments for a certain post
        if (!string.IsNullOrEmpty(postId))
        {
            query = query.Where(c => c.PostId == postId);
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
            .Select(c => new Models.PostComment
            {
                Id = c.Id,
                ContentId = c.PostId,
                UserId = c.UserId,
                Author = c.Author,
                Email = c.Email,
                Url = c.Url,
                IsApproved = c.IsApproved,
                Body = c.Body,
                Created = c.Created.DateTime
            }).ToListAsync();
    }

    /// <summary>
    /// Saves the given post model
    /// </summary>
    /// <param name="model">The post model</param>
    /// <param name="isDraft">If the model should be saved as a draft</param>
    private async Task Save<T>(T model, bool isDraft) where T : Models.PostBase
    {
        // Clone the model when saving as draft to avoid modifying the original object
        if (isDraft)
        {
            var json = JsonSerializer.Serialize(model);
            model = JsonSerializer.Deserialize<T>(json);
        }

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
                        Id = !string.IsNullOrEmpty(model.Category.Id) ? model.Category.Id : Snowflake.NewId(),
                        BlogId = model.BlogId,
                        Title = model.Category.Title,
                        Slug = Utils.GenerateSlug(model.Category.Title),
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    //await _db.Categories.AddAsync(category).ConfigureAwait(false);
                    _db.session.Store(category);
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
                            Id = !string.IsNullOrEmpty(t.Id) ? t.Id : Snowflake.NewId(),
                            BlogId = model.BlogId,
                            Title = t.Title,
                            Slug = Utils.GenerateSlug(t.Title),
                            Created = DateTime.Now,
                            LastModified = DateTime.Now
                        };
                        //await _db.Tags.AddAsync(tag).ConfigureAwait(false);
                        _db.session.Store(tag);
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
                // NoTracking not supported in Marten
            }

            // FirstOrDefaultAsync(p => p.Id ...) - no need for OrderBy

            var post = await postQuery
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                .ConfigureAwait(false);

            // Clone the post entity when saving as draft to avoid modifying the original in DB
            if (isDraft && post != null)
            {
                var json = JsonSerializer.Serialize(post);
                post = JsonSerializer.Deserialize<Post>(json);
            }
            // If not, create a new post
            if (post == null)
            {
                post = new Post
                {
                    Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };
                model.Id = post.Id;

                if (!isDraft)
                {
                    //await _db.Posts.AddAsync(post).ConfigureAwait(false);
                    _db.session.Store(post);
                }
            }
            else
            {
                post.LastModified = DateTime.Now;
            }

            post.Title = model.Title;
            post.Slug = model.Slug;
            post.BlogId = model.BlogId;
            post.SiteId = model.SiteId;
            post.CategoryId = model.Category.Id;
            post.MetaTitle = model.MetaTitle;
            post.MetaKeywords = model.MetaKeywords;
            post.MetaDescription = model.MetaDescription;
            post.Route = model.Route;
            post.Published = model.Published;

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

            // Set the foreign key on fields — they are embedded in the Post document,
            // so no separate session.StoreAsync is needed.
            foreach (var field in post.Fields)
            {
                if (string.IsNullOrEmpty(field.PostId))
                {
                    field.PostId = post.Id;
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

                // Delete removed non-reusable blocks from the embedded list
                // (they have no separate document — just removing from post.Blocks is sufficient)
                // For reusable blocks, also delete the Block document only if truly removing from all posts
                var removedFromEmbed = post.Blocks
                    .Where(b => !current.Contains(b.BlockId) && !b.Block.IsReusable && b.Block.ParentId == null)
                    .Select(b => b.Block);
                var removedItemsFromEmbed = post.Blocks
                    .Where(b => !current.Contains(b.BlockId) && b.Block.ParentId != null &&
                                removedFromEmbed.Select(p => p.Id).ToList().Contains(b.Block.ParentId))
                    .Select(b => b.Block);

                // No session.Delete needed for non-reusable blocks — deleting the embedding list entry
                // removes them when the Post document is saved.

                // Delete the old page blocks
                post.Blocks.Clear();

                // Now map the new block
                for (var n = 0; n < blocks.Count; n++)
                {
                    IMartenQueryable<Block> blockQuery = _db.Blocks;
                    if (isDraft)
                    {
                    // NoTracking not supported in Marten
                    }


                    var id = blocks[n].Id;
                    // Use session.LoadAsync for reusable blocks (standalone documents);
                    // non-reusable blocks are created fresh each time as embedded objects
                    Block block = null;
                    if (!string.IsNullOrEmpty(id) && blocks[n].IsReusable)
                    {
                        block = await _db.session.LoadAsync<Block>(id).ConfigureAwait(false);
                    }

                    if (block == null)
                    {
                        block = new Block
                        {
                            Id = !string.IsNullOrEmpty(blocks[n].Id) ? blocks[n].Id : Snowflake.NewId(),
                            Created = DateTime.Now
                        };
                        if (!isDraft && blocks[n].IsReusable)
                        {
                            // Only store reusable blocks as separate documents
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

                    if (!isDraft && blocks[n].IsReusable)
                    {
                        // Only individually-stored reusable block fields should be deleted
                        foreach (var removedField in removedFields)
                            _db.session.Delete(removedField.Id);
                    }

                    foreach (var newField in blocks[n].Fields)
                    {
                        var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                        if (field == null)
                        {
                            field = new BlockField
                            {
                                Id = !string.IsNullOrEmpty(newField.Id) ? newField.Id : Snowflake.NewId(),
                                BlockId = block.Id,
                                FieldId = newField.FieldId
                            };
                            // BlockFields are embedded in the Block object (not separate documents)
                            // Only store separately for reusable blocks
                            if (!isDraft && blocks[n].IsReusable)
                            {
                                _db.session.Store(field);
                            }

                            block.Fields.Add(field);
                        }

                        field.SortOrder = newField.SortOrder;
                        field.CLRType = newField.CLRType;
                        field.Value = newField.Value;
                    }

                    // PostBlock is embedded in post.Blocks — do NOT call session.StoreAsync
                    var postBlock = new PostBlock
                    {
                        Id = Snowflake.NewId(),
                        BlockId = block.Id,
                        Block = block,
                        PostId = post.Id,
                        SortOrder = n
                    };

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
                        Id = Snowflake.NewId(),
                        PostId = post.Id,
                        // Populate denormalized fields for draft detection
                        BlogId = post.BlogId,
                        PostLastModified = post.LastModified
                    };
                    _db.session.Store(draft);
                }

                draft.Data = JsonSerializer.Serialize(post);
                draft.Created = post.LastModified;

                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Deletes all unused categories for the specified blog.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    private async Task DeleteUnusedCategories(string blogId)
    {
        // 1. Collect all category IDs used by published posts
        var used = (await _db.Posts
            
            .Where(p => p.BlogId == blogId)
            .Select(p => p.CategoryId)
            .Distinct()
            .ToListAsync()
            .ConfigureAwait(false)).ToList();

        // 2. Query the index for revisions newer than their posts
        // 2. Query for revisions newer than their posts directly
        var revisions = await _db.session.Query<PostRevision>()
            .Where(r => r.BlogId == blogId)
            .ToListAsync()
            .ConfigureAwait(false);
        
        // Filter in-memory for drafts (Created > PostLastModified)
        var draftIds = revisions
            .Where(r => r.Created > r.PostLastModified)
            .Select(r => r.Id)
            .ToList();

        // 3. Use the revisions we already have (filter for drafts)
        var drafts = revisions.Where(r => r.Created > r.PostLastModified).ToList();

        // 4. Extract category IDs from draft data
        foreach (var draft in drafts)
        {
            var post = JsonSerializer.Deserialize<Post>(draft.Data);
            used.Add(post.CategoryId);
        }

        used = used.Distinct().ToList();

        // 5. Find categories not used by published posts or drafts
        var unused = await _db.Categories
            
            .Where(c => c.BlogId == blogId && !c.Id.In(used)) // !used.Contains(c.Id))
            .ToListAsync()
            .ConfigureAwait(false);

        // 6. Delete unused categories
        if (unused.Count > 0)
        {
            foreach (var item in unused)
                _db.session.Delete(item);

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
    //private async Task DeleteUnusedCategories(string blogId)
    //{
    //    var used = await _db.Posts
    //        .Where(p => p.BlogId == blogId)
    //        .Select(p => p.CategoryId)
    //        .Distinct()
    //        .ToListAsync()
    //        .ConfigureAwait(false);
    //
    //    var drafts = await _db.PostRevisions
    //        .Where(r => r.Post.BlogId == blogId && r.Created > r.Post.LastModified)
    //        .ToListAsync()
    //        .ConfigureAwait(false);
    //
    //    foreach (var draft in drafts)
    //    {
    //        var post = JsonSerializer.Deserialize<Post>(draft.Data);
    //        used.Add(post.CategoryId);
    //    }
    //
    //    used = used.Distinct().ToList();
    //
    //    var unused = await _db.Categories
    //        .Where(c => c.BlogId == blogId && !used.Contains(c.Id))
    //        .ToListAsync()
    //        .ConfigureAwait(false);

    //    if (unused.Count > 0)
    //    {
    //        //_db.Categories.RemoveRange(unused);
    //        foreach (var item in unused)
    //            _db.session.Delete(item);
    //        await _db.SaveChangesAsync().ConfigureAwait(false);
    //    }
    //}

    /// <summary>
    /// Deletes all unused tags for the specified blog.
    /// </summary>
    /// <param name="blogId">The blog id</param>
    private async Task DeleteUnusedTags(string blogId)
    {
        // Tags are embedded within Post documents (post.Tags) — no separate PostTags collection.
        // Collect all tag IDs currently referenced by any post in this blog.
        var allPosts = await _db.Posts
            
            .Where(p => p.BlogId == blogId)
            .ToListAsync()
            .ConfigureAwait(false);

        var used = allPosts
            .SelectMany(p => p.Tags.Select(t => t.TagId))
            .Distinct()
            .ToList();

        // Also collect tag IDs referenced by draft revisions
        var allRevisions = await _db.session.Query<PostRevision>()
            .Where(r => r.BlogId == blogId)
            .ToListAsync()
            .ConfigureAwait(false);
        
        // Filter in-memory for drafts
        var draftIds = allRevisions
            .Where(r => r.Created > r.PostLastModified)
            .Select(r => r.Id)
            .ToList();

        // Filter for drafts and extract tag IDs directly from revisions we already have
        var draftRevisions = allRevisions.Where(r => r.Created > r.PostLastModified).ToList();
        
        foreach (var draft in draftRevisions)
        {
            var draftPost = JsonSerializer.Deserialize<Post>(draft.Data);
            if (draftPost?.Tags != null)
            {
                foreach (var tag in draftPost.Tags)
                {
                    used.Add(tag.TagId);
                }
            }
        }
        
        if (draftRevisions.Count > 0)
        {
            used = used.Distinct().ToList();
        }

        var unused = await _db.Tags
            
            .Where(t => t.BlogId == blogId && !t.Id.In(used))
            .ToListAsync()
            .ConfigureAwait(false);

        if (unused.Count > 0)
        {
            foreach (var tag in unused)
                _db.session.Delete(tag);

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the base query for loading posts.
    /// </summary>
    /// <typeparam name="T">The requested model type</typeparam>
    /// <returns>The queryable</returns>
    private IMartenQueryable<Post> GetQuery<T>()
    {
        var loadRelated = !typeof(Models.IContentInfo).IsAssignableFrom(typeof(T));

        IMartenQueryable<Post> query = _db.Posts
            ;

        if (loadRelated)
        {
            // query = query;
        }

        query = (IMartenQueryable<Post>)query.OrderBy(p => p.Created);

        return query;
    }

    /// <summary>
    /// Performs additional processing and loads related models.
    /// </summary>
    /// <param name="post">The source post</param>
    /// <param name="model">The targe model</param>
    private async Task ProcessAsync<T>(Post post, T model) where T : Models.PostBase
    {
        // Category
        if (model.Category == null && !string.IsNullOrEmpty(post.CategoryId))
        {
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == post.CategoryId).ConfigureAwait(false);
            if (category != null)
            {
                model.Category = new Models.Taxonomy
                {
                    Id = category.Id,
                    Title = category.Title,
                    Slug = category.Slug,
                    Type = Models.TaxonomyType.Category
                };
            }
        }

        // Tags
        if (model.Tags.Any(t => string.IsNullOrEmpty(t.Title)))
        {
            var tagIds = post.Tags.Select(t => t.TagId).ToList();
            if (tagIds.Count > 0)
            {
                var tags = await _db.Tags.Where(t => t.Id.In(tagIds)).ToListAsync().ConfigureAwait(false);
                model.Tags.Clear();
                foreach (var tag in tags)
                {
                    model.Tags.Add(new Models.Taxonomy
                    {
                        Id = tag.Id,
                        Title = tag.Title,
                        Slug = tag.Slug,
                        Type = Models.TaxonomyType.Tag
                    });
                }
            }
        }

        // Permissions
        foreach (var permission in post.Permissions)
        {
            model.Permissions.Add(permission.Permission);
        }

        // Comments
        model.EnableComments = post.EnableComments;
        if (model.EnableComments)
        {
            model.CommentCount = await _db.PostComments
                
                .CountAsync(c => c.PostId == model.Id && c.IsApproved)
                .ConfigureAwait(false);
        }

        model.CloseCommentsAfterDays = post.CloseCommentsAfterDays;

        // Blocks
        if (!(model is Models.IContentInfo))
        {
            if (post.Blocks.Count > 0)
            {
                foreach (var postBlock in post.Blocks.OrderBy(b => b.SortOrder))
                {
                    if (!string.IsNullOrEmpty(postBlock.Block.ParentId))
                    {
                        var parent = post.Blocks.FirstOrDefault(b => b.BlockId == postBlock.Block.ParentId);
                        if (parent != null)
                        {
                            postBlock.Block.ParentId = parent.Block.Id;
                        }
                    }
                }

                model.Blocks =
                    _contentService.TransformBlocks(post.Blocks.OrderBy(b => b.SortOrder).Select(b => b.Block));
            }
        }
    }
}

