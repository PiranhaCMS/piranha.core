/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

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
        DbSet<Alias> Aliases { get; set; }

        /// <summary>
        /// Gets/sets the category set.
        /// </summary>
        DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets/sets the media set.
        /// </summary>
        DbSet<Media> Media { get; set; }

        /// <summary>
        /// Gets/sets the media folder set.
        /// </summary>
        DbSet<MediaFolder> MediaFolders { get; set; }

        /// <summary>
        /// Gets/sets the media version set.
        /// </summary>
        DbSet<MediaVersion> MediaVersions { get; set; }

        /// <summary>
        /// Gets/sets the page set.
        /// </summary>
        /// <returns></returns>
        DbSet<Page> Pages { get; set; }

        /// <summary>
        /// Gets/sets the page field set.
        /// </summary>
        /// <returns></returns>
        DbSet<PageField> PageFields { get; set; }

        /// <summary>
        /// Gets/sets the page type set.
        /// </summary>
        DbSet<PageType> PageTypes { get; set; }

        /// <summary>
        /// Gets/sets the param set.
        /// </summary>
        DbSet<Param> Params { get; set; }

        /// <summary>
        /// Gets/sets the post set.
        /// </summary>
        DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Gets/sets the post field set.
        /// </summary>        
        DbSet<PostField> PostFields { get; set; }

        /// <summary>
        /// Gets/sets the post tag set.
        /// </summary>
        DbSet<PostTag> PostTags { get; set; }

        /// <summary>
        /// Gets/sets the post type set.
        /// </summary>
        DbSet<PostType> PostTypes { get; set; }

        /// <summary>
        /// Gets/sets the site set.
        /// </summary>
        DbSet<Site> Sites { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Gets the entity set for the specified type.
        /// </summary>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        int SaveChanges();
    }
}