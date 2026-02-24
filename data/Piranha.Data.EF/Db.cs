/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Piranha.Data;
using Microsoft.EntityFrameworkCore;
using Piranha.Models;
using Raven.Embedded;
using Alias = Piranha.Data.Alias;
using ContentGroup = Piranha.Data.ContentGroup;
using Language = Piranha.Data.Language;
using MediaFolder = Piranha.Data.MediaFolder;
using MediaVersion = Piranha.Data.MediaVersion;
using PageComment = Piranha.Data.PageComment;
using PageType = Piranha.Data.PageType;
using Param = Piranha.Data.Param;
using PostComment = Piranha.Data.PostComment;
using PostType = Piranha.Data.PostType;
using Site = Piranha.Data.Site;
using SiteType = Piranha.Data.SiteType;
using Taxonomy = Piranha.Data.Taxonomy;

namespace Piranha;

/// <inheritdoc />
public abstract class Db<T> : DbContext, IDb where T : Db<T>
{
    private static readonly object _lock = new object();
    private static IDocumentStore _store;
    private IAsyncDocumentSession _session;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current raven db session</param>
    protected Db(IAsyncDocumentSession db)
    {
        this._session = db;

        if (this._session != null)
        {
            //RegisterConventions(this._session.Advanced.DocumentStore);
        }

        if (!IsInitialized)
        {
            lock (Mutex)
            {
                if (!IsInitialized)
                {
                    // Seed
                    Seed();

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
                                Console.WriteLine("[Db.cs] Session is null and RAVENDB_URL is empty. Falling back to EmbeddedServer.");
                                try {
                                    EmbeddedServer.Instance.StartServer(new ServerOptions
                                    {
                                        Licensing = new ServerOptions.LicensingOptions { ThrowOnInvalidOrMissingLicense = false }
                                    });
                                } catch (InvalidOperationException) { }
                                
                                _store = EmbeddedServer.Instance.GetDocumentStore(new DatabaseOptions(dbName + "-embedded"));
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

    public new IRavenQueryable<T1> Set<T1>() where T1 : class
    {
        return session.Query<T1>();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        if (session != null)
        {
            await session.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }

    /// <summary>
    /// Seeds the default data.
    /// </summary>
    private void Seed()
    {
    }

    public new void Dispose()
    {
        _session?.Dispose();
        base.Dispose();
    }

    public IRavenQueryable<Alias> Aliases => session.Query<Alias>();
    public IRavenQueryable<Block> Blocks => session.Query<Block>();
    public IRavenQueryable<BlockField> BlockFields => session.Query<BlockField>();
    public IRavenQueryable<Category> Categories => session.Query<Category>();
    public IRavenQueryable<Content> Content => session.Query<Content>();
    public IRavenQueryable<ContentBlock> ContentBlocks => session.Query<ContentBlock>();
    public IRavenQueryable<ContentBlockField> ContentBlockFields => session.Query<ContentBlockField>();
    public IRavenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations => session.Query<ContentBlockFieldTranslation>();
    public IRavenQueryable<ContentField> ContentFields => session.Query<ContentField>();
    public IRavenQueryable<ContentFieldTranslation> ContentFieldTranslations => session.Query<ContentFieldTranslation>();
    public IRavenQueryable<ContentTaxonomy> ContentTaxonomies => session.Query<ContentTaxonomy>();
    public IRavenQueryable<ContentTranslation> ContentTranslations => session.Query<ContentTranslation>();
    public IRavenQueryable<ContentGroup> ContentGroups => session.Query<ContentGroup>();
    public IRavenQueryable<AeroContentType> ContentTypes => session.Query<AeroContentType>();
    public IRavenQueryable<Language> Languages => session.Query<Language>();
    public IRavenQueryable<Data.Media> Media => session.Query<Data.Media>();
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
}
