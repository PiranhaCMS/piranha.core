

using Aero.Cms.RavenDb.Data;
using Aero.Cms.Models;
using Aero.Cms.RavenDb.Data;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Embedded;
using AliasEntity = Aero.Cms.RavenDb.Data.Alias;
using ContentGroup = Aero.Cms.RavenDb.Data.ContentGroup;
using Language = Aero.Cms.RavenDb.Data.Language;
using Media = Aero.Cms.RavenDb.Data.Media;
using MediaFolder = Aero.Cms.RavenDb.Data.MediaFolder;
using PageComment = Aero.Cms.RavenDb.Data.PageComment;
using PageType = Aero.Cms.RavenDb.Data.PageType;
using Param = Aero.Cms.RavenDb.Data.Param;
using PostComment = Aero.Cms.RavenDb.Data.PostComment;
using PostType = Aero.Cms.RavenDb.Data.PostType;
using Site = Aero.Cms.RavenDb.Data.Site;
using SiteType = Aero.Cms.RavenDb.Data.SiteType;
using Taxonomy = Aero.Cms.RavenDb.Data.Taxonomy;

namespace Aero.Cms.RavenDb;

public class DbRaven : DbRavenBase
{
    public DbRaven(IAsyncDocumentSession db, IDocumentStore store) : base(db, store)
    {
        // todo - drop the max requests limit for RavenDB sessions. This is a safety mechanism to prevent runaway sessions, but it can cause issues if users are not aware of it and have long-running sessions or perform many operations in a single session. We should review this and consider removing it or increasing the limit, especially for local dev and testing scenarios.
        db.Advanced.MaxNumberOfRequestsPerSession = 1000;
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
        Console.WriteLine($"[DEBUG] DbRavenBase constructor: db null={db == null}, store null={store == null}");
        if (store != null)
        {
            Console.WriteLine($"[DEBUG] DbRavenBase constructor: Database={store.Database}");
        }
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
    private bool IsInitialized = false;

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
                            string dbName = Environment.GetEnvironmentVariable("RAVENDB_DATABASE") ?? "Aero";

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

    public IRavenQueryable<AliasEntity> Aliases => session.Query<AliasEntity>();
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
