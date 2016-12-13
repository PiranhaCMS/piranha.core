/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.EF.Data;

namespace Piranha.EF
{
    public sealed class Db : DbContext, IDb
    {
        #region Properties
        /// <summary>
        /// Gets/sets the block repository.
        /// </summary>
        public DbSet<Block> Blocks { get; set; }

        /// <summary>
        /// Gets/sets the block field repository.
        /// </summary>
        public DbSet<BlockField> BlockFields { get; set; }

        /// <summary>
        /// Gets/sets the block type repository.
        /// </summary>
        public DbSet<BlockType> BlockTypes { get; set; }

        /// <summary>
        /// Gets/sets the category set.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets/sets the media set.
        /// </summary>
        public DbSet<Media> Media { get; set; }

        /// <summary>
        /// Gets/sets the media folder set.
        /// </summary>
        public DbSet<MediaFolder> MediaFolders { get; set; }

        /// <summary>
        /// Gets/sets the page set.
        /// </summary>
        public DbSet<Page> Pages { get; set; }

        /// <summary>
        /// Gets/sets the page field set.
        /// </summary>
        public DbSet<PageField> PageFields { get; set; }

        /// <summary>
        /// Gets/sets the page type set.
        /// </summary>
        public DbSet<PageType> PageTypes { get; set; }

        /// <summary>
        /// Gets/sets the post set.
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public Db(DbContextOptions<Db> options) : base(options) {
            // Ensure that the database is created & in sync
            Database.Migrate();
        }

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        /// <returns>The number of saved rows</returns>
        public override int SaveChanges() {
            OnSave();
            return base.SaveChanges();
        }

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        /// <returns>The number of saved rows</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess) {
            OnSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        /// <returns>The number of saved rows</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) {
            OnSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Saves the changes made to the context.
        /// </summary>
        /// <returns>The number of saved rows</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            OnSave();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Creates the data model.
        /// </summary>
        /// <param name="mb">The current model builder</param>
        protected override void OnModelCreating(ModelBuilder mb) {
            mb.Entity<Block>().ToTable("Piranha_Blocks");
            mb.Entity<Block>().Property(b => b.TypeId).IsRequired().HasMaxLength(32);
            mb.Entity<Block>().Property(b => b.Title).IsRequired().HasMaxLength(128);
            mb.Entity<Block>().Property(b => b.View).HasMaxLength(255);

            mb.Entity<BlockField>().ToTable("Piranha_BlockFields");
            mb.Entity<BlockField>().Property(f => f.RegionId).IsRequired().HasMaxLength(32);
            mb.Entity<BlockField>().Property(f => f.FieldId).IsRequired().HasMaxLength(32);
            mb.Entity<BlockField>().Property(f => f.CLRType).IsRequired().HasMaxLength(255);
            mb.Entity<BlockField>().HasIndex(f => new { f.BlockId, f.RegionId, f.FieldId, f.SortOrder }).IsUnique();

            mb.Entity<BlockType>().ToTable("Piranha_BlockTypes");
            mb.Entity<BlockType>().Property(t => t.Id).IsRequired().HasMaxLength(32);

            mb.Entity<Category>().ToTable("Piranha_Categories");
            mb.Entity<Category>().Property(c => c.Title).IsRequired().HasMaxLength(64);
            mb.Entity<Category>().Property(c => c.Slug).IsRequired().HasMaxLength(64);
            mb.Entity<Category>().Property(c => c.Description).HasMaxLength(512);
            mb.Entity<Category>().HasIndex(c => c.Slug).IsUnique();

            mb.Entity<Media>().ToTable("Piranha_Media");
            mb.Entity<Media>().Property(m => m.FileName).HasMaxLength(128);
            mb.Entity<Media>().Property(m => m.PublicUrl).IsRequired().HasMaxLength(255);
            mb.Entity<Media>().Property(m => m.ContentType).HasMaxLength(255);

            mb.Entity<MediaFolder>().ToTable("Piranha_MediaFolders");
            mb.Entity<MediaFolder>().Property(m => m.Title).IsRequired().HasMaxLength(128);

            mb.Entity<Page>().ToTable("Piranha_Pages");
            mb.Entity<Page>().Property(p => p.TypeId).IsRequired().HasMaxLength(32);
            mb.Entity<Page>().Property(p => p.Title).IsRequired().HasMaxLength(128);
            mb.Entity<Page>().Property(p => p.Slug).IsRequired().HasMaxLength(128);
            mb.Entity<Page>().Property(p => p.NavigationTitle).HasMaxLength(128);
            mb.Entity<Page>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Page>().Property(p => p.MetaDescription).HasMaxLength(255);
            mb.Entity<Page>().Property(p => p.Route).HasMaxLength(255);
            mb.Entity<Page>().Property(p => p.View).HasMaxLength(255);            
            mb.Entity<Page>().HasIndex(p => p.Slug).IsUnique();

            mb.Entity<PageField>().ToTable("Piranha_PageFields");
            mb.Entity<PageField>().Property(f => f.RegionId).IsRequired().HasMaxLength(32);
            mb.Entity<PageField>().Property(f => f.FieldId).IsRequired().HasMaxLength(32);
            mb.Entity<PageField>().Property(f => f.CLRType).IsRequired().HasMaxLength(255);
            mb.Entity<PageField>().HasIndex(f => new { f.PageId, f.RegionId, f.FieldId, f.SortOrder }).IsUnique();

            mb.Entity<PageType>().ToTable("Piranha_PageTypes");
            mb.Entity<PageType>().Property(t => t.Id).IsRequired().HasMaxLength(32);

            mb.Entity<Post>().ToTable("Piranha_Posts");
            mb.Entity<Post>().Property(p => p.Title).IsRequired().HasMaxLength(128);
            mb.Entity<Post>().Property(p => p.Slug).IsRequired().HasMaxLength(128);
            mb.Entity<Post>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Post>().Property(p => p.MetaDescription).HasMaxLength(255);
            mb.Entity<Post>().Property(p => p.Excerpt).HasMaxLength(512);
            mb.Entity<Post>().Property(p => p.Route).HasMaxLength(255);
            mb.Entity<Post>().HasIndex(p => new { p.CategoryId, p.Slug }).IsUnique();

            mb.Entity<Tag>().ToTable("Piranha_Tags");
            mb.Entity<Tag>().Property(t => t.Title).IsRequired().HasMaxLength(64);
            mb.Entity<Tag>().Property(t => t.Slug).IsRequired().HasMaxLength(64);
            mb.Entity<Tag>().HasIndex(t => t.Slug).IsUnique();

            base.OnModelCreating(mb);
        }

        #region Private methods
        /// <summary>
        /// Processes the changed entities before saving them to the database.
        /// </summary>
        private void OnSave() {
            foreach (var entry in ChangeTracker.Entries()) {
                var now = DateTime.Now;

                if (entry.State != EntityState.Deleted) {
                    // Shoud we set modified date
                    if (entry.Entity is IModified)
                        ((IModified)entry.Entity).LastModified = now;
                }

                if (entry.State == EntityState.Added) {
                    // Should we auto generate a unique id
                    if (entry.Entity is IModel && ((IModel)entry.Entity).Id == Guid.Empty)
                        ((IModel)entry.Entity).Id = Guid.NewGuid();

                    // Should we set created date
                    if (entry.Entity is ICreated)
                        ((ICreated)entry.Entity).Created = now;

                    // Should we auto generate slug
                    if (entry.Entity is ISlug && String.IsNullOrWhiteSpace(((ISlug)entry.Entity).Slug))
                        ((ISlug)entry.Entity).Slug = Utils.GenerateSlug(((ISlug)entry.Entity).Title);

                    // Should we notify changes
                    if (entry.Entity is INotify)
                        ((INotify)entry.Entity).OnSave(this);
                } else if (entry.State == EntityState.Modified) {
                    // Should we notify changes
                    if (entry.Entity is INotify)
                        ((INotify)entry.Entity).OnSave(this);
                } else if (entry.State == EntityState.Deleted) {
                    // Should we notify changes
                    if (entry.Entity is INotify)
                        ((INotify)entry.Entity).OnDelete(this);
                }
            }
        }
        #endregion
    }
}
