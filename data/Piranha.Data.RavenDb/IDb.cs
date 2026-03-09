using Piranha.Data.RavenDb.Data;
using Piranha.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
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

/// <summary>
/// The main db context.
/// </summary>
public interface IDb : IDisposable
{
    /// <summary>
    /// Gets the current RavenDB async document session.
    /// Prefer session.LoadAsync&lt;T&gt;(id) over LINQ queries for ID-based lookups.
    /// </summary>
    public IAsyncDocumentSession session { get; }

    // -------------------------------------------------------------------------
    // Root document collections — each is an aggregate root in RavenDB.
    // WaitForNonStaleResults is intentionally NOT applied globally here;
    // apply it per-query only where write-then-read-immediately is required.
    // -------------------------------------------------------------------------

    /// <summary>Gets the alias collection.</summary>
    IRavenQueryable<Alias> Aliases { get => session.Query<Alias>(); }

    /// <summary>
    /// Gets the reusable block collection.
    /// NOTE: Only reusable blocks (IsReusable = true) are stored here as top-level
    /// documents. Non-reusable blocks are embedded within their parent Page/Post document.
    /// </summary>
    IRavenQueryable<Block> Blocks { get => session.Query<Block>(); }

    /// <summary>Gets the category collection.</summary>
    IRavenQueryable<Category> Categories { get => session.Query<Category>(); }

    /// <summary>Gets the content collection.</summary>
    IRavenQueryable<Content> Content { get => session.Query<Content>(); }

    /// <summary>Gets the content block collection.</summary>
    IRavenQueryable<ContentBlock> ContentBlocks { get => session.Query<ContentBlock>(); }

    /// <summary>Gets the content block field collection.</summary>
    IRavenQueryable<ContentBlockField> ContentBlockFields { get => session.Query<ContentBlockField>(); }

    /// <summary>Gets the content block field translation collection.</summary>
    IRavenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations { get => session.Query<ContentBlockFieldTranslation>(); }

    /// <summary>Gets the content field collection.</summary>
    IRavenQueryable<ContentField> ContentFields { get => session.Query<ContentField>(); }

    /// <summary>Gets the content field translation collection.</summary>
    IRavenQueryable<ContentFieldTranslation> ContentFieldTranslations { get => session.Query<ContentFieldTranslation>(); }

    /// <summary>Gets the content taxonomy collection.</summary>
    IRavenQueryable<ContentTaxonomy> ContentTaxonomies { get => session.Query<ContentTaxonomy>(); }

    /// <summary>Gets the content translation collection.</summary>
    IRavenQueryable<ContentTranslation> ContentTranslations { get => session.Query<ContentTranslation>(); }

    /// <summary>Gets the content group collection.</summary>
    IRavenQueryable<ContentGroup> ContentGroups { get => session.Query<ContentGroup>(); }

    /// <summary>Gets the content type collection.</summary>
    IRavenQueryable<ContentType> ContentTypes { get => session.Query<ContentType>(); }

    /// <summary>Gets the language collection.</summary>
    IRavenQueryable<Language> Languages { get => session.Query<Language>(); }

    /// <summary>
    /// Gets the media collection.
    /// MediaVersion objects are embedded within each Media document — not a separate collection.
    /// </summary>
    IRavenQueryable<Media> Media { get => session.Query<Media>(); }

    /// <summary>Gets the media folder collection.</summary>
    IRavenQueryable<MediaFolder> MediaFolders { get => session.Query<MediaFolder>(); }

    /// <summary>
    /// Gets the page collection.
    /// PageBlock, PageField, and PagePermission objects are embedded within each Page document.
    /// </summary>
    IRavenQueryable<Page> Pages { get => session.Query<Page>(); }

    /// <summary>Gets the page comment collection.</summary>
    IRavenQueryable<PageComment> PageComments { get => session.Query<PageComment>(); }

    /// <summary>
    /// Gets the page revision collection.
    /// Each PageRevision contains SiteId and PageLastModified as denormalized fields.
    /// </summary>
    IRavenQueryable<PageRevision> PageRevisions { get => session.Query<PageRevision>(); }

    /// <summary>Gets the page type collection.</summary>
    IRavenQueryable<PageType> PageTypes { get => session.Query<PageType>(); }

    /// <summary>Gets the param collection.</summary>
    IRavenQueryable<Param> Params { get => session.Query<Param>(); }

    /// <summary>
    /// Gets the post collection.
    /// PostBlock, PostField, PostPermission, and PostTag objects are embedded within each Post document.
    /// </summary>
    IRavenQueryable<Post> Posts { get => session.Query<Post>(); }

    /// <summary>Gets the post comment collection.</summary>
    IRavenQueryable<PostComment> PostComments { get => session.Query<PostComment>(); }

    /// <summary>
    /// Gets the post revision collection.
    /// Each PostRevision contains BlogId and PostLastModified as denormalized fields.
    /// </summary>
    IRavenQueryable<PostRevision> PostRevisions { get => session.Query<PostRevision>(); }

    /// <summary>Gets the post type collection.</summary>
    IRavenQueryable<PostType> PostTypes { get => session.Query<PostType>(); }

    /// <summary>
    /// Gets the site collection.
    /// SiteField objects are embedded within each Site document.
    /// </summary>
    IRavenQueryable<Site> Sites { get => session.Query<Site>(); }

    /// <summary>Gets the site type collection.</summary>
    IRavenQueryable<SiteType> SiteTypes { get => session.Query<SiteType>(); }

    /// <summary>Gets the tag collection.</summary>
    IRavenQueryable<Tag> Tags { get => session.Query<Tag>(); }

    /// <summary>Gets the taxonomy collection.</summary>
    IRavenQueryable<Taxonomy> Taxonomies { get => session.Query<Taxonomy>(); }

    /// <summary>
    /// Saves the changes made to the current session.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
