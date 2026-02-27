/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Data.RavenDb.Data;
using Piranha.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Embedded;
using Alias = Piranha.Data.RavenDb.Data.Alias;
using ContentGroup = Piranha.Data.RavenDb.Data.ContentGroup;
using Language = Piranha.Data.RavenDb.Data.Language;
using Media = Piranha.Data.RavenDb.Data.Media;
using MediaFolder = Piranha.Data.RavenDb.Data.MediaFolder;
using MediaVersion = Piranha.Data.RavenDb.Data.MediaVersion;
using PageComment = Piranha.Data.RavenDb.Data.PageComment;
using PageType = Piranha.Data.RavenDb.Data.PageType;
using Param = Piranha.Data.RavenDb.Data.Param;
using PostComment = Piranha.Data.RavenDb.Data.PostComment;
using PostType = Piranha.Data.RavenDb.Data.PostType;
using Site = Piranha.Data.RavenDb.Data.Site;
using SiteType = Piranha.Data.RavenDb.Data.SiteType;
using Taxonomy = Piranha.Data.RavenDb.Data.Taxonomy;

namespace Piranha.Data.RavenDb;

public class DbRaven : DbRavenBase
{
    public DbRaven(IAsyncDocumentSession db, IDocumentStore store) : base(db, store)
    {
    }
}

/// <inheritdoc />
public abstract class DbRavenBase : IDb 
{
    private static readonly object _lock = new object();
    private IDocumentStore _store;
    private IAsyncDocumentSession _session;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current raven db session</param>
    protected DbRavenBase(IAsyncDocumentSession db, IDocumentStore store)
    {
        this._store = store;
        this._session = db;

        if (!IsInitialized)
        {
            lock (Mutex)
            {
                if (!IsInitialized)
                {
                    // Seed
                    SeedAsync().GetAwaiter().GetResult();

                    IsInitialized = true;
                }
            }
        }
    }


    /// <summary>
    /// Gets/sets whether the db context as been initialized. This
    /// is only performed once in the application lifecycle.
    /// </summary>
    private static volatile bool IsInitialized = false;

    /// <summary>
    /// The object mutex used for initializing the context.
    /// </summary>
    private static readonly object Mutex = new object();

    /// <summary>
    /// Gets the raven db session
    /// </summary>
    public IAsyncDocumentSession session
    {
        get
        {
            // todo - review this logic. We should ideally be relying on DI to manage the lifecycle of the session/store, and not have this fallback logic in the db context. This is currently here to ensure we have a working session/store for testing and local dev purposes, but it may not be ideal for production scenarios. We should consider refactoring this to be more explicit about how the session/store are managed, and potentially remove this fallback logic in favor of a more robust DI setup.
            if (_session == null)
            {
                if (_store == null)
                {
                    lock (_lock)
                    {
                        if (_store == null)
                        {
                            // TODO: Review this setup. DI should ideally be handling the session/store lifecycle.
                            // This fallback logic is a temporary measure for test/local environment stability.
                            string url = Environment.GetEnvironmentVariable("RAVENDB_URL");
                            string dbName = Environment.GetEnvironmentVariable("RAVENDB_DATABASE") ?? "piranha";

                            if (string.IsNullOrEmpty(url))
                            {
                                // Fallback to embedded if no URL is provided (test/local dev)
                                Console.WriteLine(
                                    "[Db.cs] Session is null and RAVENDB_URL is empty. Falling back to EmbeddedServer.");
                                try
                                {
                                    EmbeddedServer.Instance.StartServer(new ServerOptions
                                    {
                                        Licensing = new ServerOptions.LicensingOptions
                                            { ThrowOnInvalidOrMissingLicense = false }
                                    });
                                }
                                catch (InvalidOperationException)
                                {
                                }

                                _store = EmbeddedServer.Instance.GetDocumentStore(
                                    new DatabaseOptions(dbName + "-embedded"));
                            }
                            else
                            {
                                Console.WriteLine($"[Db.cs] Session is null. Connecting to RavenDB at {url}");
                                var store = new DocumentStore
                                {
                                    Urls = new[] { url },
                                    Database = dbName
                                };
                                store.Initialize();
                                _store = store;
                            }

                            //RegisterConventions(_store);
                        }
                    }
                }

                _session = _store.OpenAsyncSession();
            }

            return _session;
        }
    }

    //public new IRavenQueryable<T1> Set<T1>() where T1 : class
    //{
    //    return session.Query<T1>();
    //}

    public IRavenQueryable<Alias> Aliases => session.Query<Alias>();
    public IRavenQueryable<Block> Blocks => session.Query<Block>();
    public IRavenQueryable<BlockField> BlockFields => session.Query<BlockField>();
    public IRavenQueryable<Category> Categories => session.Query<Category>();
    public IRavenQueryable<Content> Content => session.Query<Content>();
    public IRavenQueryable<ContentBlock> ContentBlocks => session.Query<ContentBlock>();
    public IRavenQueryable<ContentBlockField> ContentBlockFields => session.Query<ContentBlockField>();
    public IRavenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations =>
        session.Query<ContentBlockFieldTranslation>();
    public IRavenQueryable<ContentField> ContentFields => session.Query<ContentField>();
    public IRavenQueryable<ContentFieldTranslation> ContentFieldTranslations =>
        session.Query<ContentFieldTranslation>();
    public IRavenQueryable<ContentTaxonomy> ContentTaxonomies => session.Query<ContentTaxonomy>();
    public IRavenQueryable<ContentTranslation> ContentTranslations => session.Query<ContentTranslation>();
    public IRavenQueryable<ContentGroup> ContentGroups => session.Query<ContentGroup>();
    public IRavenQueryable<AeroContentType> ContentTypes => session.Query<AeroContentType>();
    public IRavenQueryable<Language> Languages => session.Query<Language>();
    public IRavenQueryable<Media> Media => session.Query<Media>();
    public IRavenQueryable<MediaFolder> MediaFolders => session.Query<MediaFolder>();
    public IRavenQueryable<MediaVersion> MediaVersions => session.Query<MediaVersion>();
    public IRavenQueryable<Page> Pages => session.Query<Page>();
    public IRavenQueryable<PageBlock> PageBlocks => session.Query<PageBlock>();
    public IRavenQueryable<PageComment> PageComments => session.Query<PageComment>();
    public IRavenQueryable<PageField> PageFields => session.Query<PageField>();
    public IRavenQueryable<PagePermission> PagePermissions => session.Query<PagePermission>();
    public IRavenQueryable<PageRevision> PageRevisions => session.Query<PageRevision>();
    public IRavenQueryable<PageType> PageTypes => session.Query<PageType>();
    public IRavenQueryable<Param> Params => session.Query<Param>();
    public IRavenQueryable<Post> Posts => session.Query<Post>();
    public IRavenQueryable<PostBlock> PostBlocks => session.Query<PostBlock>();
    public IRavenQueryable<PostComment> PostComments => session.Query<PostComment>();
    public IRavenQueryable<PostField> PostFields => session.Query<PostField>();
    public IRavenQueryable<PostPermission> PostPermissions => session.Query<PostPermission>();
    public IRavenQueryable<PostRevision> PostRevisions => session.Query<PostRevision>();
    public IRavenQueryable<PostTag> PostTags => session.Query<PostTag>();
    public IRavenQueryable<PostType> PostTypes => session.Query<PostType>();
    public IRavenQueryable<Site> Sites => session.Query<Site>();
    public IRavenQueryable<SiteField> SiteFields => session.Query<SiteField>();
    public IRavenQueryable<SiteType> SiteTypes => session.Query<SiteType>();
    public IRavenQueryable<Tag> Tags => session.Query<Tag>();
    public IRavenQueryable<Taxonomy> Taxonomies => session.Query<Taxonomy>();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        if (session != null)
        {
            await session.SaveChangesAsync(cancellationToken);
        }

        return 1;
    }




    // TODO: abstract Seeding data to app setup  -  temp here to ensure we have a default language and site for testing and local dev purposes. This should ideally be handled by the application setup logic, not the db context.
    /// <summary>
    /// Seeds the default data.
    /// </summary>
    private async Task SeedAsync()
    {
        await SaveChangesAsync();

        //
        // Default language
        //
        var langId = Snowflake.NewId();

        var count = await Languages.CountAsync();
        if (count == 0)
        {
            //Languages.Add(new Data.Language
            var lang = new Language
            {
                Id = langId,
                Title = "Default",
                Culture = "en-US",
                IsDefault = true
            };
            await session.StoreAsync(lang);
        }
        else
        {
            langId = (await Languages.FirstOrDefaultAsync(l => l.IsDefault)).Id;
        }

        //
        // Default site
        //
        count = await Sites.CountAsync();
        if (count == 0)
        {
            //Sites.Add(new Data.Site
            var stie = new Site
            {
                Id = Snowflake.NewId(),
                LanguageId = langId,
                InternalId = "Default",
                IsDefault = true,
                Title = "Default Site",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            await session.StoreAsync(stie);
        }
        else
        {
            // todo - verify this works correctly being we don't use guids anymore 
            // When upgrading, make sure we assign the default language id
            // to already created sites.
            var sites = await Sites.Where(s => s.LanguageId == null).ToListAsync();
            foreach (var site in sites)
            {
                site.LanguageId = langId;
            }
        }

        //
        // Make sure we don't have NULL values in Piranha_MediaVersions.FileExtension
        //
        var versions = await MediaVersions
            .Where(m => m.FileExtension == null)
            .ToListAsync();
        foreach (var version in versions)
            version.FileExtension = ".webp"; // shouldn't this be webp since we're makin use of https://static.photos

        var pageBlocks = await PageBlocks
            .Where(b => !string.IsNullOrEmpty(b.ParentId))
            .ToListAsync();
        var pageBlocksId = pageBlocks.Select(b => b.BlockId).ToList();
        var blocks = await Blocks
            .Where(b => b.Id.In(pageBlocksId))
            .ToListAsync();
        
        foreach (var block in blocks)
        {
            var pageBlock = pageBlocks.Single(b => b.BlockId == block.Id);
            block.ParentId = pageBlock.ParentId;
            pageBlock.ParentId = null;
        }
        var postBlocks = await PostBlocks
            .Where(b => !string.IsNullOrEmpty(b.ParentId))
            .ToListAsync();
        var postBlocksId = postBlocks.Select(b => b.BlockId).ToList();
        blocks = await Blocks
            .Where(b => b.Id.In(postBlocksId))
            .ToListAsync();
        
        foreach (var block in blocks)
        {
            var postBlock = postBlocks.Single(b => b.BlockId == block.Id);
            block.ParentId = postBlock.ParentId;
            postBlock.ParentId = null;
        }

        await SaveChangesAsync();
    }

    public new void Dispose()
    {
        _session?.Dispose();
    }
}
