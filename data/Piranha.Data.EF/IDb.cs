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
using Piranha.Data;
using Piranha.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Alias = Piranha.Data.Alias;
using ContentGroup = Piranha.Data.ContentGroup;
using Language = Piranha.Data.Language;
using Media = Piranha.Data.Media;
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

/// <summary>
/// The main db context.
/// </summary>
public interface IDb : IDisposable
{
    public IAsyncDocumentSession session { get; }

/// <summary>
    /// Gets/sets the alias set.
    /// </summary>
    IRavenQueryable<Alias> Aliases { get => session.Query<Alias>(); }

    /// <summary>
    /// Gets/sets the block set.
    /// </summary>
    IRavenQueryable<Block> Blocks { get => session.Query<Block>(); }

    /// <summary>
    /// Gets/sets the block field set.
    /// </summary>
    IRavenQueryable<BlockField> BlockFields { get => session.Query<BlockField>(); }

    /// <summary>
    /// Gets/sets the category set.
    /// </summary>
    IRavenQueryable<Category> Categories { get => session.Query<Category>(); }

    /// <summary>
    /// Gets/sets the content set.
    /// </summary>
    IRavenQueryable<Content> Content { get => session.Query<Content>(); }

    /// <summary>
    /// Gets/sets the content block set.
    /// </summary>
    IRavenQueryable<ContentBlock> ContentBlocks { get => session.Query<ContentBlock>(); }

    /// <summary>
    /// Gets/sets the content block field set.
    /// </summary>
    IRavenQueryable<ContentBlockField> ContentBlockFields { get  => session.Query<ContentBlockField>(); }

    /// <summary>
    /// Gets/sets the content block field translation set.
    /// </summary>
    IRavenQueryable<ContentBlockFieldTranslation> ContentBlockFieldTranslations { get => session.Query<ContentBlockFieldTranslation>(); }

    /// <summary>
    /// Gets/sets the content field set.
    /// </summary>
    IRavenQueryable<ContentField> ContentFields { get  => session.Query<ContentField>(); }

    /// <summary>
    /// Gets/sets the content field translation set.
    /// </summary>
    IRavenQueryable<ContentFieldTranslation> ContentFieldTranslations { get  => session.Query<ContentFieldTranslation>(); }

    /// <summary>
    /// Gets/sets the content taxonomy set.
    /// </summary>
    IRavenQueryable<ContentTaxonomy> ContentTaxonomies { get  => session.Query<ContentTaxonomy>(); }

    /// <summary>
    /// Gets/sets the content translation set.
    /// </summary>
    IRavenQueryable<ContentTranslation> ContentTranslations { get  => session.Query<ContentTranslation>(); }

    /// <summary>
    /// Gets/sets the content group set.
    /// </summary>
    IRavenQueryable<ContentGroup> ContentGroups { get  => session.Query<ContentGroup>(); }

    /// <summary>
    /// Gets/sets the content type set.
    /// </summary>
    IRavenQueryable<AeroContentType> ContentTypes { get => session.Query<AeroContentType>(); }

    /// <summary>
    /// Gets/sets the language set.
    /// </summary>
    IRavenQueryable<Language> Languages { get => session.Query<Language>(); }

    /// <summary>
    /// Gets/sets the media set.
    /// </summary>
    IRavenQueryable<Media> Media { get => session.Query<Media>(); }

    /// <summary>
    /// Gets/sets the media folder set.
    /// </summary>
    IRavenQueryable<MediaFolder> MediaFolders { get => session.Query<MediaFolder>(); }

    /// <summary>
    /// Gets/sets the media version set.
    /// </summary>
    IRavenQueryable<MediaVersion> MediaVersions { get => session.Query<MediaVersion>(); }

    /// <summary>
    /// Gets/sets the page set.
    /// </summary>
    IRavenQueryable<Page> Pages { get => session.Query<Page>(); }

    /// <summary>
    /// Gets/sets the page block set.
    /// </summary>
    IRavenQueryable<PageBlock> PageBlocks { get => session.Query<PageBlock>(); }

    /// <summary>
    /// Gets/sets the page comments.
    /// </summary>
    IRavenQueryable<PageComment> PageComments { get => session.Query<PageComment>(); }

    /// <summary>
    /// Gets/sets the page field set.
    /// </summary>
    IRavenQueryable<PageField> PageFields { get => session.Query<PageField>(); }

    /// <summary>
    /// Gets/sets the page permission set.
    /// </summary>
    IRavenQueryable<PagePermission> PagePermissions { get => session.Query<PagePermission>(); }

    /// <summary>
    /// Gets/sets the page revision set.
    /// </summary>
    IRavenQueryable<PageRevision> PageRevisions { get => session.Query<PageRevision>(); }

    /// <summary>
    /// Gets/sets the page type set.
    /// </summary>
    IRavenQueryable<PageType> PageTypes { get => session.Query<PageType>(); }

    /// <summary>
    /// Gets/sets the param set.
    /// </summary>
    IRavenQueryable<Param> Params { get => session.Query<Param>(); }

    /// <summary>
    /// Gets/sets the post set.
    /// </summary>
    IRavenQueryable<Post> Posts { get => session.Query<Post>(); }

    /// <summary>
    /// Gets/sets the post block set.
    /// </summary>
    IRavenQueryable<PostBlock> PostBlocks { get => session.Query<PostBlock>(); }

    /// <summary>
    /// Gets/sets the post comments.
    /// </summary>
    IRavenQueryable<PostComment> PostComments { get => session.Query<PostComment>(); }

    /// <summary>
    /// Gets/sets the post field set.
    /// </summary>
    IRavenQueryable<PostField> PostFields { get => session.Query<PostField>(); }

    /// <summary>
    /// Gets/sets the post permission set.
    /// </summary>
    IRavenQueryable<PostPermission> PostPermissions { get => session.Query<PostPermission>(); }

    /// <summary>
    /// Gets/sets the post revision set.
    /// </summary>
    IRavenQueryable<PostRevision> PostRevisions { get => session.Query<PostRevision>(); }

    /// <summary>
    /// Gets/sets the post tag set.
    /// </summary>
    IRavenQueryable<PostTag> PostTags { get => session.Query<PostTag>(); }

    /// <summary>
    /// Gets/sets the post type set.
    /// </summary>
    IRavenQueryable<PostType> PostTypes { get => session.Query<PostType>(); }

    /// <summary>
    /// Gets/sets the site set.
    /// </summary>
    IRavenQueryable<Site> Sites { get => session.Query<Site>(); }

    /// <summary>
    /// Gets/sets the site field set.
    /// </summary>
    IRavenQueryable<SiteField> SiteFields { get => session.Query<SiteField>(); }

    /// <summary>
    /// Gets/sets the site type set.
    /// </summary>
    IRavenQueryable<SiteType> SiteTypes { get => session.Query<SiteType>(); }

    /// <summary>
    /// Gets/sets the tag set.
    /// </summary>
    IRavenQueryable<Tag> Tags { get => session.Query<Tag>(); }

    /// <summary>
    /// Gets/sets the taxonomy set.
    /// </summary>
    IRavenQueryable<Taxonomy> Taxonomies { get => session.Query<Taxonomy>(); }

    /// <summary>
    /// Gets the entity set for the specified type.
    /// </summary>
    IRavenQueryable<T> Set<T>() where T : class;

    /// <summary>
    /// Saves the changes made to the context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
