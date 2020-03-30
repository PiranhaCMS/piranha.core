/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Piranha
{
    /// <summary>
    /// Interface for the Piranha Db Context.
    /// </summary>
    public interface IDb : IDisposable
    {
        /// <summary>
        /// Gets/sets the alias set.
        /// </summary>
        DbSet<Data.Alias> Aliases { get; set; }

        /// <summary>
        /// Gets/sets the block set.
        /// </summary>
        DbSet<Data.Block> Blocks { get; set; }

        /// <summary>
        /// Gets/sets the block field set.
        /// </summary>
        DbSet<Data.BlockField> BlockFields { get; set; }

        /// <summary>
        /// Gets/sets the category set.
        /// </summary>
        DbSet<Data.Category> Categories { get; set; }

        /// <summary>
        /// Gets/sets the media set.
        /// </summary>
        DbSet<Data.Media> Media { get; set; }

        /// <summary>
        /// Gets/sets the media folder set.
        /// </summary>
        DbSet<Data.MediaFolder> MediaFolders { get; set; }

        /// <summary>
        /// Gets/sets the media version set.
        /// </summary>
        DbSet<Data.MediaVersion> MediaVersions { get; set; }

        /// <summary>
        /// Gets/sets the page set.
        /// </summary>
        DbSet<Data.Page> Pages { get; set; }

        /// <summary>
        /// Gets/sets the page block set.
        /// </summary>
        DbSet<Data.PageBlock> PageBlocks { get; set; }

        /// <summary>
        /// Gets/sets the page comments.
        /// </summary>
        DbSet<Data.PageComment> PageComments { get; set; }

        /// <summary>
        /// Gets/sets the page field set.
        /// </summary>
        DbSet<Data.PageField> PageFields { get; set; }

        /// <summary>
        /// Gets/sets the page permission set.
        /// </summary>
        DbSet<Data.PagePermission> PagePermissions { get; set; }

        /// <summary>
        /// Gets/sets the page revision set.
        /// </summary>
        DbSet<Data.PageRevision> PageRevisions { get; set; }

        /// <summary>
        /// Gets/sets the page type set.
        /// </summary>
        DbSet<Data.PageType> PageTypes { get; set; }

        /// <summary>
        /// Gets/sets the param set.
        /// </summary>
        DbSet<Data.Param> Params { get; set; }

        /// <summary>
        /// Gets/sets the post set.
        /// </summary>
        DbSet<Data.Post> Posts { get; set; }

        /// <summary>
        /// Gets/sets the post block set.
        /// </summary>
        DbSet<Data.PostBlock> PostBlocks { get; set; }

        /// <summary>
        /// Gets/sets the post comments.
        /// </summary>
        DbSet<Data.PostComment> PostComments { get; set; }

        /// <summary>
        /// Gets/sets the post field set.
        /// </summary>
        DbSet<Data.PostField> PostFields { get; set; }

        /// <summary>
        /// Gets/sets the post permission set.
        /// </summary>
        DbSet<Data.PostPermission> PostPermissions { get; set; }

        /// <summary>
        /// Gets/sets the post revision set.
        /// </summary>
        DbSet<Data.PostRevision> PostRevisions { get; set; }

        /// <summary>
        /// Gets/sets the post tag set.
        /// </summary>
        DbSet<Data.PostTag> PostTags { get; set; }

        /// <summary>
        /// Gets/sets the post type set.
        /// </summary>
        DbSet<Data.PostType> PostTypes { get; set; }

        /// <summary>
        /// Gets/sets the site set.
        /// </summary>
        DbSet<Data.Site> Sites { get; set; }

        /// <summary>
        /// Gets/sets the site field set.
        /// </summary>
        DbSet<Data.SiteField> SiteFields { get; set; }

        /// <summary>
        /// Gets/sets the site type set.
        /// </summary>
        DbSet<Data.SiteType> SiteTypes { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        DbSet<Data.Tag> Tags { get; set; }

        /// <summary>
        /// Gets the entity set for the specified type.
        /// </summary>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}