/*
 * Copyright (c) 2016 Billy Wolfington
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Piranha.EF.Data;

namespace Piranha.EF
{
    /// <summary>
    /// Interface to allow for dependency injection for unit testing
    /// in the Piranha.EF.Repositories objects.
    /// </summary>
    /// <remarks>
    /// This interface does list some methods from <see cref="DbContext" />,
    /// since the implementing class derives from it
    /// </remarks>
    public interface IDb
    {
        #region Properties
        /// <summary>
        /// Gets/sets the block repository.
        /// </summary>
        DbSet<Block> Blocks { get; set; }
        
        /// <summary>
        /// Gets/sets the block field repository.
        /// </summary>
        DbSet<BlockField> BlockFields { get; set; }

        /// <summary>
        /// Gets/sets the block type repository.
        /// </summary>
        DbSet<BlockType> BlockTypes { get; set; }

        /// <summary>
        /// Gets/sets the category set.
        /// </summary>
        DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets/sets the page set.
        /// </summary>
        DbSet<Page> Pages { get; set; }

        /// <summary>
        /// Gets/sets the page field set.
        /// </summary>
        DbSet<PageField> PageFields { get; set; }

        /// <summary>
        /// Gets/sets the page type set.
        /// </summary>
        DbSet<PageType> PageTypes { get; set; }

        /// <summary>
        /// Gets/sets the post set.
        /// </summary>
        DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Property needed from <see cref="DbContext" />
        /// </summary>
        DatabaseFacade Database { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Method needed from <see cref="DbContext" />
        /// </summary>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Method needed from <see cref="DbContext" />
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Method needed from <see cref="DbContext" />
        /// </summary>
        int SaveChanges(bool acceptAllChangesOnSuccess);

        /// <summary>
        /// Method needed from <see cref="DbContext" />
        /// </summary>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Method needed from <see cref="DbContext" />
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        #endregion
    }
}