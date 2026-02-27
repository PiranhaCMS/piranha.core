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
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
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

/// <summary>
/// The main db context.
/// </summary>
public interface IDb : IDisposable
{
    public IAsyncDocumentSession session { get; }

/// <summary>
    /// Gets/sets the alias set.
    /// </summary>
    IRavenQueryable<Alias> Aliases { get => session.Query<Alias>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the block set.
    /// </summary>
    IRavenQueryable<Block> Blocks { get => session.Query<Block>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the block field set.
    /// </summary>
    IRavenQueryable<BlockField> BlockFields { get => session.Query<BlockField>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the category set.
    /// </summary>
    IRavenQueryable<Category> Categories { get => session.Query<Category>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content set.
    /// </summary>
    IRavenQueryable<Content> Content { get => session.Query<Content>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content block set.
    /// </summary>
    IRavenQueryable<ContentBlock> ContentBlocks { get => session.Query<ContentBlock>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content block field set.
    /// </summary>
    IRavenQueryable<ContentBlockField> ContentBlockFields { get  => session.Query<ContentBlockField>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content block field translation set.
    /// </summary>
    IRavenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations { get => session.Query<ContentBlockFieldTranslation>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content field set.
    /// </summary>
    IRavenQueryable<ContentField> ContentFields { get  => session.Query<ContentField>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content field translation set.
    /// </summary>
    IRavenQueryable<ContentFieldTranslation> ContentFieldTranslations { get  => session.Query<ContentFieldTranslation>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content taxonomy set.
    /// </summary>
    IRavenQueryable<ContentTaxonomy> ContentTaxonomies { get  => session.Query<ContentTaxonomy>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content translation set.
    /// </summary>
    IRavenQueryable<ContentTranslation> ContentTranslations { get  => session.Query<ContentTranslation>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content group set.
    /// </summary>
    IRavenQueryable<ContentGroup> ContentGroups { get  => session.Query<ContentGroup>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the content type set.
    /// </summary>
    IRavenQueryable<AeroContentType> ContentTypes { get => session.Query<AeroContentType>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the language set.
    /// </summary>
    IRavenQueryable<Language> Languages { get => session.Query<Language>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the media set.
    /// </summary>
    IRavenQueryable<Media> Media { get => session.Query<Media>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the media folder set.
    /// </summary>
    IRavenQueryable<MediaFolder> MediaFolders { get => session.Query<MediaFolder>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the media version set.
    /// </summary>
    IRavenQueryable<MediaVersion> MediaVersions { get => session.Query<MediaVersion>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page set.
    /// </summary>
    IRavenQueryable<Page> Pages { get => session.Query<Page>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page block set.
    /// </summary>
    IRavenQueryable<PageBlock> PageBlocks { get => session.Query<PageBlock>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page comments.
    /// </summary>
    IRavenQueryable<PageComment> PageComments { get => session.Query<PageComment>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page field set.
    /// </summary>
    IRavenQueryable<PageField> PageFields { get => session.Query<PageField>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page permission set.
    /// </summary>
    IRavenQueryable<PagePermission> PagePermissions { get => session.Query<PagePermission>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page revision set.
    /// </summary>
    IRavenQueryable<PageRevision> PageRevisions { get => session.Query<PageRevision>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the page type set.
    /// </summary>
    IRavenQueryable<PageType> PageTypes { get => session.Query<PageType>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the param set.
    /// </summary>
    IRavenQueryable<Param> Params { get => session.Query<Param>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post set.
    /// </summary>
    IRavenQueryable<Post> Posts { get => session.Query<Post>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post block set.
    /// </summary>
    IRavenQueryable<PostBlock> PostBlocks { get => session.Query<PostBlock>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post comments.
    /// </summary>
    IRavenQueryable<PostComment> PostComments { get => session.Query<PostComment>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post field set.
    /// </summary>
    IRavenQueryable<PostField> PostFields { get => session.Query<PostField>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post permission set.
    /// </summary>
    IRavenQueryable<PostPermission> PostPermissions { get => session.Query<PostPermission>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post revision set.
    /// </summary>
    IRavenQueryable<PostRevision> PostRevisions { get => session.Query<PostRevision>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post tag set.
    /// </summary>
    IRavenQueryable<PostTag> PostTags { get => session.Query<PostTag>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the post type set.
    /// </summary>
    IRavenQueryable<PostType> PostTypes { get => session.Query<PostType>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the site set.
    /// </summary>
    IRavenQueryable<Site> Sites { get => session.Query<Site>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the site field set.
    /// </summary>
    IRavenQueryable<SiteField> SiteFields { get => session.Query<SiteField>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the site type set.
    /// </summary>
    IRavenQueryable<SiteType> SiteTypes { get => session.Query<SiteType>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the tag set.
    /// </summary>
    IRavenQueryable<Tag> Tags { get => session.Query<Tag>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets/sets the taxonomy set.
    /// </summary>
    IRavenQueryable<Taxonomy> Taxonomies { get => session.Query<Taxonomy>().Customize(x=>x.WaitForNonStaleResults()); }

    /// <summary>
    /// Gets the entity set for the specified type.
    /// </summary>
    //IRavenQueryable<T> Set<T>() where T : class;

    /// <summary>
    /// Saves the changes made to the context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
