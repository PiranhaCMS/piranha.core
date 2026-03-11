using Aero.Cms.Data.Data;
using Aero.Cms.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Alias = Aero.Cms.Data.Data.Alias;
using AliasEntity = Aero.Cms.Data.Data.Alias;
using ContentGroup = Aero.Cms.Data.Data.ContentGroup;
using Language = Aero.Cms.Data.Data.Language;
using Media = Aero.Cms.Data.Data.Media;
using MediaFolder = Aero.Cms.Data.Data.MediaFolder;
using PageComment = Aero.Cms.Data.Data.PageComment;
using PageType = Aero.Cms.Data.Data.PageType;
using Param = Aero.Cms.Data.Data.Param;
using PostComment = Aero.Cms.Data.Data.PostComment;
using PostType = Aero.Cms.Data.Data.PostType;
using Site = Aero.Cms.Data.Data.Site;
using SiteType = Aero.Cms.Data.Data.SiteType;
using Taxonomy = Aero.Cms.Data.Data.Taxonomy;

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
    IRavenQueryable<Data_ContentGroup> ContentGroups { get => session.Query<Data_ContentGroup>(); }

    /// <summary>Gets the content type collection.</summary>
    IRavenQueryable<ContentType> ContentTypes { get => session.Query<ContentType>(); }

    /// <summary>Gets the language collection.</summary>
    IRavenQueryable<Data_Language> Languages { get => session.Query<Data_Language>(); }

    /// <summary>
    /// Gets the media collection.
    /// MediaVersion objects are embedded within each Media document — not a separate collection.
    /// </summary>
    IRavenQueryable<Data_Media> Media { get => session.Query<Data_Media>(); }

    /// <summary>Gets the media folder collection.</summary>
    IRavenQueryable<Data_MediaFolder> MediaFolders { get => session.Query<Data_MediaFolder>(); }

    /// <summary>
    /// Gets the page collection.
    /// PageBlock, PageField, and PagePermission objects are embedded within each Page document.
    /// </summary>
    IRavenQueryable<Page> Pages { get => session.Query<Page>(); }

    /// <summary>Gets the page comment collection.</summary>
    IRavenQueryable<Data_PageComment> PageComments { get => session.Query<Data_PageComment>(); }

    /// <summary>
    /// Gets the page revision collection.
    /// Each PageRevision contains SiteId and PageLastModified as denormalized fields.
    /// </summary>
    IRavenQueryable<PageRevision> PageRevisions { get => session.Query<PageRevision>(); }

    /// <summary>Gets the page type collection.</summary>
    IRavenQueryable<Data_PageType> PageTypes { get => session.Query<Data_PageType>(); }

    /// <summary>Gets the param collection.</summary>
    IRavenQueryable<Data_Param> Params { get => session.Query<Data_Param>(); }

    /// <summary>
    /// Gets the post collection.
    /// PostBlock, PostField, PostPermission, and PostTag objects are embedded within each Post document.
    /// </summary>
    IRavenQueryable<Post> Posts { get => session.Query<Post>(); }

    /// <summary>Gets the post comment collection.</summary>
    IRavenQueryable<Data_PostComment> PostComments { get => session.Query<Data_PostComment>(); }

    /// <summary>
    /// Gets the post revision collection.
    /// Each PostRevision contains BlogId and PostLastModified as denormalized fields.
    /// </summary>
    IRavenQueryable<PostRevision> PostRevisions { get => session.Query<PostRevision>(); }

    /// <summary>Gets the post type collection.</summary>
    IRavenQueryable<Data_PostType> PostTypes { get => session.Query<Data_PostType>(); }

    /// <summary>
    /// Gets the site collection.
    /// SiteField objects are embedded within each Site document.
    /// </summary>
    IRavenQueryable<Data_Site> Sites { get => session.Query<Data_Site>(); }

    /// <summary>Gets the site type collection.</summary>
    IRavenQueryable<Data_SiteType> SiteTypes { get => session.Query<Data_SiteType>(); }

    /// <summary>Gets the tag collection.</summary>
    IRavenQueryable<Tag> Tags { get => session.Query<Tag>(); }

    /// <summary>Gets the taxonomy collection.</summary>
    IRavenQueryable<Data_Taxonomy> Taxonomies { get => session.Query<Data_Taxonomy>(); }

    /// <summary>
    /// Saves the changes made to the current session.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
