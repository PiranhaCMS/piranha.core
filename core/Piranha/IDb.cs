/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using System;

namespace Piranha
{
    /// <summary>
    /// Interface for the Piranha Db Context.
    /// </summary>
    public interface IDb : IDisposable
    {
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
        /// Gets/sets the page set.
        /// </summary>
        /// <returns></returns>
        DbSet<Data.Page> Pages { get; set; }

        /// <summary>
        /// Gets/sets the page field set.
        /// </summary>
        /// <returns></returns>
        DbSet<Data.PageField> PageFields { get; set; }

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
        /// Gets/sets the post field set.
        /// </summary>        
        DbSet<Data.PostField> PostFields { get; set; }

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
    }
}