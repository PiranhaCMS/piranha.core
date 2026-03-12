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

/// <summary>
/// The main db context.
/// </summary>
public interface IDb : IDisposable
{
    /// <summary>
    /// Gets the current RavenDB async document session.
    /// Prefer session.LoadAsync&lt;T&gt;(id) over LINQ queries for ID-based lookups.
    /// </summary>
    public IDocumentSession session { get; }

    // -------------------------------------------------------------------------
    // Root document collections — each is an aggregate root in RavenDB.
    // WaitForNonStaleResults is intentionally NOT applied globally here;
    // apply it per-query only where write-then-read-immediately is required.
    // -------------------------------------------------------------------------

    /// <summary>Gets the alias collection.</summary>
    IMartenQueryable<Alias> Aliases { get => session.Query<Alias>(); }

    /// <summary>
    /// Gets the reusable block collection.
    /// NOTE: Only reusable blocks (IsReusable = true) are stored here as top-level
    /// documents. Non-reusable blocks are embedded within their parent Page/Post document.
    /// </summary>
    IMartenQueryable<Block> Blocks { get => session.Query<Block>(); }

    /// <summary>Gets the category collection.</summary>
    IMartenQueryable<Category> Categories { get => session.Query<Category>(); }

    /// <summary>Gets the content collection.</summary>
    IMartenQueryable<Content> Content { get => session.Query<Content>(); }

    /// <summary>Gets the content block collection.</summary>
    IMartenQueryable<ContentBlock> ContentBlocks { get => session.Query<ContentBlock>(); }

    /// <summary>Gets the content block field collection.</summary>
    IMartenQueryable<ContentBlockField> ContentBlockFields { get => session.Query<ContentBlockField>(); }

    /// <summary>Gets the content block field translation collection.</summary>
    IMartenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations { get => session.Query<ContentBlockFieldTranslation>(); }

    /// <summary>Gets the content field collection.</summary>
    IMartenQueryable<ContentField> ContentFields { get => session.Query<ContentField>(); }

    /// <summary>Gets the content field translation collection.</summary>
    IMartenQueryable<ContentFieldTranslation> ContentFieldTranslations { get => session.Query<ContentFieldTranslation>(); }

    /// <summary>Gets the content taxonomy collection.</summary>
    IMartenQueryable<ContentTaxonomy> ContentTaxonomies { get => session.Query<ContentTaxonomy>(); }

    /// <summary>Gets the content translation collection.</summary>
    IMartenQueryable<ContentTranslation> ContentTranslations { get => session.Query<ContentTranslation>(); }

    /// <summary>Gets the content group collection.</summary>
    IMartenQueryable<Data_ContentGroup> ContentGroups { get => session.Query<Data_ContentGroup>(); }

    /// <summary>Gets the content type collection.</summary>
    IMartenQueryable<ContentType> ContentTypes { get => session.Query<ContentType>(); }

    /// <summary>Gets the language collection.</summary>
    IMartenQueryable<Data_Language> Languages { get => session.Query<Data_Language>(); }

    /// <summary>
    /// Gets the media collection.
    /// MediaVersion objects are embedded within each Media document — not a separate collection.
    /// </summary>
    IMartenQueryable<Data_Media> Media { get => session.Query<Data_Media>(); }

    /// <summary>Gets the media folder collection.</summary>
    IMartenQueryable<Data_MediaFolder> MediaFolders { get => session.Query<Data_MediaFolder>(); }

    /// <summary>
    /// Gets the page collection.
    /// PageBlock, PageField, and PagePermission objects are embedded within each Page document.
    /// </summary>
    IMartenQueryable<Page> Pages { get => session.Query<Page>(); }

    /// <summary>Gets the page comment collection.</summary>
    IMartenQueryable<Data_PageComment> PageComments { get => session.Query<Data_PageComment>(); }

    /// <summary>
    /// Gets the page revision collection.
    /// Each PageRevision contains SiteId and PageLastModified as denormalized fields.
    /// </summary>
    IMartenQueryable<PageRevision> PageRevisions { get => session.Query<PageRevision>(); }

    /// <summary>Gets the page type collection.</summary>
    IMartenQueryable<Data_PageType> PageTypes { get => session.Query<Data_PageType>(); }

    /// <summary>Gets the param collection.</summary>
    IMartenQueryable<Data_Param> Params { get => session.Query<Data_Param>(); }

    /// <summary>
    /// Gets the post collection.
    /// PostBlock, PostField, PostPermission, and PostTag objects are embedded within each Post document.
    /// </summary>
    IMartenQueryable<Post> Posts { get => session.Query<Post>(); }

    /// <summary>Gets the post comment collection.</summary>
    IMartenQueryable<Data_PostComment> PostComments { get => session.Query<Data_PostComment>(); }

    /// <summary>
    /// Gets the post revision collection.
    /// Each PostRevision contains BlogId and PostLastModified as denormalized fields.
    /// </summary>
    IMartenQueryable<PostRevision> PostRevisions { get => session.Query<PostRevision>(); }

    /// <summary>Gets the post type collection.</summary>
    IMartenQueryable<Data_PostType> PostTypes { get => session.Query<Data_PostType>(); }

    /// <summary>
    /// Gets the site collection.
    /// SiteField objects are embedded within each Site document.
    /// </summary>
    IMartenQueryable<Data_Site> Sites { get => session.Query<Data_Site>(); }

    /// <summary>Gets the site type collection.</summary>
    IMartenQueryable<Data_SiteType> SiteTypes { get => session.Query<Data_SiteType>(); }

    /// <summary>Gets the tag collection.</summary>
    IMartenQueryable<Tag> Tags { get => session.Query<Tag>(); }

    /// <summary>Gets the taxonomy collection.</summary>
    IMartenQueryable<Data_Taxonomy> Taxonomies { get => session.Query<Data_Taxonomy>(); }

    /// <summary>
    /// Saves the changes made to the current session.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
