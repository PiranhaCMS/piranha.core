

using Aero.Cms.Data.Data;
using Aero.Cms.Models;
using Marten;
using Marten.Linq;
using Alias = Aero.Cms.Data.Data.Alias;

namespace Aero.Cms.Data;

using Data_ContentGroup = Data.ContentGroup;
using Data_Language = Data.Language;
using Data_Media = Data.Media;
using Data_MediaFolder = Data.MediaFolder;
using Data_PageComment = Data.PageComment;
using Data_PageType = Data.PageType;
using Data_Param = Data.Param;
using Data_PostComment = Data.PostComment;
using Data_PostType = Data.PostType;
using Data_Site = Data.Site;
using Data_SiteType = Data.SiteType;
using Data_Taxonomy = Data.Taxonomy;

public class AeroDb(IDocumentSession db, IDocumentStore store) : AeroDbBase(db, store);


/// <inheritdoc />
public abstract class AeroDbBase : IDb
{
    private static readonly object _lock = new object();
    private IDocumentStore _store;
    private IDocumentSession _session;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current raven db session</param>
    protected AeroDbBase(IDocumentSession db, IDocumentStore store)
    {
        if (store is null)
            ArgumentNullException.ThrowIfNull(store, nameof(IDocumentStore));

        // todo - figure out a cleaner way to instantiate IDocumentSession
        db ??= store.LightweightSession();

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
    public IDocumentSession session
    {
        get
        {
            return _session;
        }
    }

    // -------------------------------------------------------------------------
    // Aggregate root collections (queryable via LINQ / static indexes).
    // Sub-entity data (blocks, fields, permissions, tags, versions) is embedded
    // within the parent aggregate document and is NOT queryable as a separate
    // collection. Use session.LoadAsync<T>(id) for ID-based lookups.
    // -------------------------------------------------------------------------

    // todo - might be able to simply use IQueryable<> instead of IMartenQueryable<>
    public IMartenQueryable<Alias> Aliases => session.Query<Alias>();
    public IMartenQueryable<Block> Blocks => session.Query<Block>();
    public IMartenQueryable<Category> Categories => session.Query<Category>();
    public IMartenQueryable<Content> Content => session.Query<Content>();
    public IMartenQueryable<ContentBlock> ContentBlocks => session.Query<ContentBlock>();
    public IMartenQueryable<ContentBlockField> ContentBlockFields => session.Query<ContentBlockField>();
    public IMartenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations =>
        session.Query<ContentBlockFieldTranslation>();
    public IMartenQueryable<ContentField> ContentFields => session.Query<ContentField>();
    public IMartenQueryable<ContentFieldTranslation> ContentFieldTranslations =>
        session.Query<ContentFieldTranslation>();
    public IMartenQueryable<ContentTaxonomy> ContentTaxonomies => session.Query<ContentTaxonomy>();
    public IMartenQueryable<ContentTranslation> ContentTranslations => session.Query<ContentTranslation>();
    public IMartenQueryable<Data_ContentGroup> ContentGroups => session.Query<Data_ContentGroup>();
    public IMartenQueryable<ContentType> ContentTypes => session.Query<ContentType>();
    public IMartenQueryable<Data_Language> Languages => session.Query<Data_Language>();
    public IMartenQueryable<Data_Media> Media => session.Query<Data_Media>();
    public IMartenQueryable<Data_MediaFolder> MediaFolders => session.Query<Data_MediaFolder>();
    public IMartenQueryable<Page> Pages => session.Query<Page>();
    public IMartenQueryable<Data_PageComment> PageComments => session.Query<Data_PageComment>();
    public IMartenQueryable<PageRevision> PageRevisions => session.Query<PageRevision>();
    public IMartenQueryable<Data_PageType> PageTypes => session.Query<Data_PageType>();
    public IMartenQueryable<Data_Param> Params => session.Query<Data_Param>();
    public IMartenQueryable<Post> Posts => session.Query<Post>();
    public IMartenQueryable<Data_PostComment> PostComments => session.Query<Data_PostComment>();
    public IMartenQueryable<PostRevision> PostRevisions => session.Query<PostRevision>();
    public IMartenQueryable<Data_PostType> PostTypes => session.Query<Data_PostType>();
    public IMartenQueryable<Data_Site> Sites => session.Query<Data_Site>();
    public IMartenQueryable<Data_SiteType> SiteTypes => session.Query<Data_SiteType>();
    public IMartenQueryable<Tag> Tags => session.Query<Tag>();
    public IMartenQueryable<Data_Taxonomy> Taxonomies => session.Query<Data_Taxonomy>();

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
            var lang = new Data_Language
            {
                Id = langId,
                Title = "Default",
                Culture = "en-US",
                IsDefault = true
            };
            session.Store(lang);
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
            var site = new Data_Site
            {
                Id = Snowflake.NewId(),
                LanguageId = langId,
                InternalId = "Default",
                IsDefault = true,
                Title = "Default Site",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
            session.Store(site);
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
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool IsDisposing)
    {
        if (IsDisposing)
        {
            session?.Dispose();
        }
    }
}
