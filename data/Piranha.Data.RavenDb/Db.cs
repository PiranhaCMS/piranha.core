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
    /// Gets/sets whether the db context has been initialized. This
    /// is only performed once in the application lifecycle.
    /// </summary>
    private static volatile bool IsInitialized = false;

    /// <summary>
    /// The object mutex used for initializing the context.
    /// </summary>
    private static readonly object Mutex = new object();

    /// <summary>
    /// Gets the raven db session.
    /// </summary>
    public IAsyncDocumentSession session
    {
        get
        {
            // TODO: Review this logic. We should rely on DI to manage the session/store lifecycle,
            // not fallback logic in the db context. This is here for testing and local dev stability.
            if (_session == null)
            {
                if (_store == null)
                {
                    lock (_lock)
                    {
                        if (_store == null)
                        {
                            string url = Environment.GetEnvironmentVariable("RAVENDB_URL");
                            string dbName = Environment.GetEnvironmentVariable("RAVENDB_DATABASE") ?? "piranha";

                            if (string.IsNullOrEmpty(url))
                            {
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
                        }
                    }
                }

                _session = _store.OpenAsyncSession();
            }

            return _session;
        }
    }

    // -------------------------------------------------------------------------
    // Aggregate root collections (queryable via LINQ / static indexes).
    // Sub-entity data (blocks, fields, permissions, tags, versions) is embedded
    // within the parent aggregate document and is NOT queryable as a separate
    // collection. Use session.LoadAsync<T>(id) for ID-based lookups.
    // -------------------------------------------------------------------------

    public IRavenQueryable<Alias> Aliases => session.Query<Alias>();
    public IRavenQueryable<Block> Blocks => session.Query<Block>();
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
    public IRavenQueryable<ContentType> ContentTypes => session.Query<ContentType>();
    public IRavenQueryable<Language> Languages => session.Query<Language>();
    public IRavenQueryable<Media> Media => session.Query<Media>();
    public IRavenQueryable<MediaFolder> MediaFolders => session.Query<MediaFolder>();
    public IRavenQueryable<Page> Pages => session.Query<Page>();
    public IRavenQueryable<PageComment> PageComments => session.Query<PageComment>();
    public IRavenQueryable<PageRevision> PageRevisions => session.Query<PageRevision>();
    public IRavenQueryable<PageType> PageTypes => session.Query<PageType>();
    public IRavenQueryable<Param> Params => session.Query<Param>();
    public IRavenQueryable<Post> Posts => session.Query<Post>();
    public IRavenQueryable<PostComment> PostComments => session.Query<PostComment>();
    public IRavenQueryable<PostRevision> PostRevisions => session.Query<PostRevision>();
    public IRavenQueryable<PostType> PostTypes => session.Query<PostType>();
    public IRavenQueryable<Site> Sites => session.Query<Site>();
    public IRavenQueryable<SiteType> SiteTypes => session.Query<SiteType>();
    public IRavenQueryable<Tag> Tags => session.Query<Tag>();
    public IRavenQueryable<Taxonomy> Taxonomies => session.Query<Taxonomy>();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (session != null)
        {
            await session.SaveChangesAsync(cancellationToken);
        }

        return 1;
    }

    // TODO: Abstract seeding to app setup. Temp here for testing and local dev.
    /// <summary>
    /// Seeds the default data (default language + site).
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
            var site = new Site
            {
                Id = Snowflake.NewId(),
                LanguageId = langId,
                InternalId = "Default",
                IsDefault = true,
                Title = "Default Site",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            await session.StoreAsync(site);
        }
        else
        {
            // Ensure existing sites have a language assigned
            var sites = await Sites.Where(s => s.LanguageId == null).ToListAsync();
            foreach (var site in sites)
            {
                site.LanguageId = langId;
            }
        }

        await SaveChangesAsync();
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}
