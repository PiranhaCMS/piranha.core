/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Piranha;

/// <inheritdoc />
public abstract class Db<T> : IDb where T : Db<T>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="options">Configuration options</param>
    protected Db(IAsyncDocumentSession db) : base()
    {
        this.session = db;
        if (!IsInitialized)
        {
            lock (Mutex)
            {
                if (!IsInitialized)
                {
                    // Migrate database
                    //Database.Migrate();
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
    /// Gets/sets the raven db session
    /// </summary>
    public IAsyncDocumentSession session { get; }

    /// <summary>
    /// Gets/sets the alias set.
    /// </summary>
    public IRavenQueryable<Data.Alias> Aliases { get;  }

    /// <summary>
    /// Gets/sets the block set.
    /// </summary>
    public IRavenQueryable<Data.Block> Blocks { get;  }

    /// <summary>
    /// Gets/sets the block field set.
    /// </summary>
    public IRavenQueryable<Data.BlockField> BlockFields { get;  }

    /// <summary>
    /// Gets/sets the category set.
    /// </summary>
    public IRavenQueryable<Data.Category> Categories { get;  }

    /// <summary>
    /// Gets/sets the content set.
    /// </summary>
    public IRavenQueryable<Data.Content> Content { get;  }

    /// <summary>
    /// Gets/sets the content block set.
    /// </summary>
    public IRavenQueryable<Data.ContentBlock> ContentBlocks { get;  }

    /// <summary>
    /// Gets/sets the content block field set.
    /// </summary>
    public IRavenQueryable<Data.ContentBlockField> ContentBlockFields { get;  }

    /// <summary>
    /// Gets/sets the content block field translation set.
    /// </summary>
    public IRavenQueryable<Data.ContentBlockFieldTranslation> ContentBlockFieldTranslations { get;  }

    /// <summary>
    /// Gets/sets the content field set.
    /// </summary>
    public IRavenQueryable<Data.ContentField> ContentFields { get;  }

    /// <summary>
    /// Gets/sets the content field translation set.
    /// </summary>
    public IRavenQueryable<Data.ContentFieldTranslation> ContentFieldTranslations { get;  }

    /// <summary>
    /// Gets/sets the content taxonomy set.
    /// </summary>
    public IRavenQueryable<Data.ContentTaxonomy> ContentTaxonomies { get;  }

    /// <summary>
    /// Gets/sets the content translation set.
    /// </summary>
    public IRavenQueryable<Data.ContentTranslation> ContentTranslations { get;  }

    /// <summary>
    /// Gets/sets the content group set.
    /// </summary>
    public IRavenQueryable<Data.ContentGroup> ContentGroups { get;  }

    /// <summary>
    /// Gets/sets the content type set.
    /// </summary>
    public IRavenQueryable<Data.ContentType> ContentTypes { get;  }

    /// <summary>
    /// Gets/sets the language set.
    /// </summary>
    public IRavenQueryable<Data.Language> Languages { get;  }

    /// <summary>
    /// Gets/sets the media set.
    /// </summary>
    public IRavenQueryable<Data.Media> Media { get;  }

    /// <summary>
    /// Gets/sets the media folder set.
    /// </summary>
    public IRavenQueryable<Data.MediaFolder> MediaFolders { get;  }

    /// <summary>
    /// Gets/sets the media version set.
    /// </summary>
    public IRavenQueryable<Data.MediaVersion> MediaVersions { get;  }

    /// <summary>
    /// Gets/sets the page set.
    /// </summary>
    public IRavenQueryable<Data.Page> Pages { get;  }

    /// <summary>
    /// Gets/sets the page block set.
    /// </summary>
    public IRavenQueryable<Data.PageBlock> PageBlocks { get;  }

    /// <summary>
    /// Gets/sets the page comments.
    /// </summary>
    public IRavenQueryable<Data.PageComment> PageComments { get;  }

    /// <summary>
    /// Gets/sets the page field set.
    /// </summary>
    public IRavenQueryable<Data.PageField> PageFields { get;  }

    /// <summary>
    /// Gets/sets the page permission set.
    /// </summary>
    public IRavenQueryable<Data.PagePermission> PagePermissions { get;  }

    /// <summary>
    /// Gets/sets the page revision set.
    /// </summary>
    public IRavenQueryable<Data.PageRevision> PageRevisions { get;  }

    /// <summary>
    /// Gets/sets the page type set.
    /// </summary>
    public IRavenQueryable<Data.PageType> PageTypes { get;  }

    /// <summary>
    /// Gets/sets the param set.
    /// </summary>
    public IRavenQueryable<Data.Param> Params { get;  }

    /// <summary>
    /// Gets/sets the post set.
    /// </summary>
    public IRavenQueryable<Data.Post> Posts { get;  }

    /// <summary>
    /// Gets/sets the post block set.
    /// </summary>
    public IRavenQueryable<Data.PostBlock> PostBlocks { get;  }

    /// <summary>
    /// Gets/sets the post comments.
    /// </summary>
    public IRavenQueryable<Data.PostComment> PostComments { get;  }

    /// <summary>
    /// Gets/sets the post field set.
    /// </summary>
    public IRavenQueryable<Data.PostField> PostFields { get;  }

    /// <summary>
    /// Gets/sets the post permission set.
    /// </summary>
    public IRavenQueryable<Data.PostPermission> PostPermissions { get;  }

    /// <summary>
    /// Gets/sets the post revision set.
    /// </summary>
    public IRavenQueryable<Data.PostRevision> PostRevisions { get;  }

    /// <summary>
    /// Gets/sets the post tag set.
    /// </summary>
    public IRavenQueryable<Data.PostTag> PostTags { get;  }

    /// <summary>
    /// Gets/sets the post type set.
    /// </summary>
    public IRavenQueryable<Data.PostType> PostTypes { get;  }

    /// <summary>
    /// Gets/sets the site set.
    /// </summary>
    public IRavenQueryable<Data.Site> Sites { get;  }

    /// <summary>
    /// Gets/sets the site field set.
    /// </summary>
    public IRavenQueryable<Data.SiteField> SiteFields { get;  }

    /// <summary>
    /// Gets/sets the site type set.
    /// </summary>
    public IRavenQueryable<Data.SiteType> SiteTypes { get;  }

    /// <summary>
    /// Gets/sets the tag set.
    /// </summary>
    public IRavenQueryable<Data.Tag> Tags { get;  }

    /// <summary>
    /// Gets/sets the taxonomy set.
    /// </summary>
    public IRavenQueryable<Data.Taxonomy> Taxonomies { get;  }

    public IRavenQueryable<T1> Set<T1>() where T1 : class
    {
        throw new NotImplementedException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        // Track what the session considers "new" documents
        var newDocuments = session.Advanced.WhatChanged()
            .Where(change => change.Value.Any(c => c.Change == DocumentsChanges.ChangeType.DocumentAdded))
            .ToList();

        await session.SaveChangesAsync(cancellationToken);
        return newDocuments.Count;
    }

    /// <summary>
    /// Seeds the default data.
    /// </summary>
    private void Seed()
    {
        session.SaveChangesAsync().GetAwaiter().GetResult();

        //
        // Default language
        //
        var langId = Guid.NewGuid();

        if (Languages.Count() == 0)
        {
            session.StoreAsync(new Data.Language
            {
                Id = langId,
                Title = "Default",
                Culture = "en-US",
                IsDefault = true
            }).GetAwaiter().GetResult();
        }
        else
        {
            langId = Languages.FirstOrDefault(l => l.IsDefault).Id;
        }

        //
        // Default site
        //
        if (Sites.Count() == 0)
        {
            session.StoreAsync(new Data.Site
            {
                Id = Guid.NewGuid(),
                LanguageId = langId,
                InternalId = "Default",
                IsDefault = true,
                Title = "Default Site",
                Created = DateTime.Now,
                LastModified = DateTime.Now
            }).GetAwaiter().GetResult();
        }
        else
        {
            // When upgrading, make sure we assign the default language id
            // to already created sites.
            var sites = Sites.Where(s => s.LanguageId == Guid.Empty);
            foreach (var site in sites)
            {
                site.LanguageId = langId;
            }
        }

        //
        // Make sure we don't have NULL values in Piranha_MediaVersions.FileExtension
        //
        var versions = MediaVersions
            .Where(m => m.FileExtension == null)
            .ToList();
        foreach (var version in versions)
            version.FileExtension = ".jpg";

        var pageBlocks = PageBlocks
            .Where(b => b.ParentId.HasValue)
            .ToList();
        var pageBlocksId = pageBlocks.Select(b => b.BlockId).ToList();
        var blocks = Blocks
            .Where(b => pageBlocksId.Contains(b.Id))
            .ToList();
        foreach (var block in blocks)
        {
            var pageBlock = pageBlocks.Single(b => b.BlockId == block.Id);
            block.ParentId = pageBlock.ParentId;
            pageBlock.ParentId = null;
        }
        var postBlocks = PostBlocks
            .Where(b => b.ParentId.HasValue)
            .ToList();
        var postBlocksId = postBlocks.Select(b => b.BlockId).ToList();
        blocks = Blocks
            .Where(b => postBlocksId.Contains(b.Id))
            .ToList();
        foreach (var block in blocks)
        {
            var postBlock = postBlocks.Single(b => b.BlockId == block.Id);
            block.ParentId = postBlock.ParentId;
            postBlock.ParentId = null;
        }

        session.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        session?.Dispose();
    }
}
