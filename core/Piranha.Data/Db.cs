/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Piranha.Data;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Piranha
{
	/// <summary>
	/// The main db context for low-level data access.
	/// </summary>
    public sealed class Db : DbContext
    {
		#region Properties
		/// <summary>
		/// Gets/sets the author set.
		/// </summary>
		public DbSet<Author> Authors { get; set; }

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
		/// Gets/sets the page type field set.
		/// </summary>
		public DbSet<PageTypeField> PageTypeFields { get; set; }

		/// <summary>
		/// Gets/sets the param set.
		/// </summary>
		public DbSet<Param> Params { get; set; }

		/// <summary>
		/// Gets/sets the post set.
		/// </summary>
		public DbSet<Post> Posts { get; set; }

		/// <summary>
		/// Gets/sets the post field set.
		/// </summary>
		public DbSet<PostField> PostFields { get; set; }

		/// <summary>
		/// Gets/sets the post type set.
		/// </summary>
		public DbSet<PostType> PostTypes { get; set; }

		/// <summary>
		/// Gets/sets the post type field set.
		/// </summary>
		public DbSet<PostTypeField> PostTypeFields { get; set; }

		/// <summary>
		/// Gets/sets the tag set.
		/// </summary>
		public DbSet<Tag> Tags { get; set; }
		#endregion

		/// <summary>
		/// Configurs the db context.
		/// </summary>
		/// <param name="optionsBuilder">The current configuration options</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=piranha.dnx;Trusted_Connection=True;");
		}

		/// <summary>
		/// Creates and configures the data model.
		/// </summary>
		/// <param name="mb">The model builder</param>
		protected override void OnModelCreating(ModelBuilder mb) {
			mb.Entity<Author>().ToTable("Piranha_Authors");
			mb.Entity<Author>().Property(a => a.UserId).HasMaxLength(128);
			mb.Entity<Author>().Property(a => a.Name).HasMaxLength(128).IsRequired();
			mb.Entity<Author>().Property(a => a.Description).HasMaxLength(512);

			mb.Entity<Category>().ToTable("Piranha_Categories");
			mb.Entity<Category>().Property(c => c.Title).HasMaxLength(64).IsRequired();
			mb.Entity<Category>().Property(c => c.Slug).HasMaxLength(64).IsRequired();
			mb.Entity<Category>().Property(c => c.Description).HasMaxLength(512);
			mb.Entity<Category>().Property(c => c.ArchiveTitle).HasMaxLength(128);
			mb.Entity<Category>().Property(c => c.ArchiveRoute).HasMaxLength(128);
			mb.Entity<Category>().HasIndex(c => c.Slug).IsUnique();

			mb.Entity<Media>().ToTable("Piranha_Media");
			mb.Entity<Media>().Property(m => m.Filename).HasMaxLength(128).IsRequired();
			mb.Entity<Media>().Property(m => m.ContentType).HasMaxLength(255).IsRequired();
			mb.Entity<Media>().Property(m => m.PublicUrl).HasMaxLength(255).IsRequired();

			mb.Entity<MediaFolder>().ToTable("Piranha_MediaFolders");
			mb.Entity<MediaFolder>().Property(m => m.Name).HasMaxLength(64).IsRequired();

			mb.Entity<Page>().ToTable("Piranha_Pages");
			mb.Entity<Page>().Property(p => p.Title).HasMaxLength(128).IsRequired();
			mb.Entity<Page>().Property(p => p.NavigationTitle).HasMaxLength(128);
			mb.Entity<Page>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
			mb.Entity<Page>().Property(p => p.MetaTitle).HasMaxLength(64);
			mb.Entity<Page>().Property(p => p.MetaKeywords).HasMaxLength(128);
			mb.Entity<Page>().Property(p => p.MetaDescription).HasMaxLength(256);
			mb.Entity<Page>().Property(p => p.Route).HasMaxLength(128);
			mb.Entity<Page>().HasOne(p => p.Author).WithMany().OnDelete(DeleteBehavior.SetNull);
			mb.Entity<Page>().HasOne(p => p.Type).WithMany().OnDelete(DeleteBehavior.Restrict);
			mb.Entity<Page>().HasIndex(p => p.Slug).IsUnique();

			mb.Entity<PageField>().ToTable("Piranha_PageFields");
			mb.Entity<PageField>().HasIndex(p => new { p.ParentId, p.TypeId }).IsUnique();

			mb.Entity<PageType>().ToTable("Piranha_PageTypes");
			mb.Entity<PageType>().Property(p => p.Name).HasMaxLength(64).IsRequired();
			mb.Entity<PageType>().Property(p => p.InternalId).HasMaxLength(64).IsRequired();
			mb.Entity<PageType>().Property(p => p.Description).HasMaxLength(256);
			mb.Entity<PageType>().Property(p => p.Route).HasMaxLength(128);
			mb.Entity<PageType>().HasIndex(p => p.InternalId).IsUnique();

			mb.Entity<PageTypeField>().ToTable("Piranha_PageTypeFields");
			mb.Entity<PageTypeField>().Property(p => p.Name).HasMaxLength(64).IsRequired();
			mb.Entity<PageTypeField>().Property(p => p.InternalId).HasMaxLength(64).IsRequired();
			mb.Entity<PageTypeField>().Property(p => p.CLRType).HasMaxLength(512).IsRequired();
			mb.Entity<PageTypeField>().HasIndex(p => new { p.TypeId, p.InternalId }).IsUnique();

			mb.Entity<Param>().ToTable("Piranha_Params");
			mb.Entity<Param>().Property(p => p.Name).HasMaxLength(64).IsRequired();
			mb.Entity<Param>().Property(p => p.InternalId).HasMaxLength(64).IsRequired();
			mb.Entity<Param>().HasIndex(p => p.InternalId).IsUnique();

			mb.Entity<Post>().ToTable("Piranha_Posts");
			mb.Entity<Post>().Property(p => p.Title).HasMaxLength(128).IsRequired();
			mb.Entity<Post>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
			mb.Entity<Post>().Property(p => p.MetaTitle).HasMaxLength(64);
			mb.Entity<Post>().Property(p => p.MetaKeywords).HasMaxLength(128);
			mb.Entity<Post>().Property(p => p.MetaDescription).HasMaxLength(256);
			mb.Entity<Post>().Property(p => p.Route).HasMaxLength(128);
			mb.Entity<Post>().Property(p => p.Excerpt).HasMaxLength(512);
			mb.Entity<Post>().HasOne(p => p.Type).WithMany().OnDelete(DeleteBehavior.Restrict);
			mb.Entity<Post>().HasOne(p => p.Author).WithMany().OnDelete(DeleteBehavior.SetNull);
			mb.Entity<Post>().HasOne(p => p.Category).WithMany().OnDelete(DeleteBehavior.Restrict);
			mb.Entity<Post>().HasIndex(p => new { p.CategoryId, p.Slug });

			mb.Entity<PostField>().ToTable("Piranha_PostFields");
			mb.Entity<PostField>().HasIndex(p => new { p.ParentId, p.TypeId }).IsUnique();

			mb.Entity<PostTypeField>().ToTable("Piranha_PostTypeFields");
			mb.Entity<PostTypeField>().Property(p => p.Name).HasMaxLength(64).IsRequired();
			mb.Entity<PostTypeField>().Property(p => p.InternalId).HasMaxLength(64).IsRequired();
			mb.Entity<PostTypeField>().Property(p => p.CLRType).HasMaxLength(512).IsRequired();
			mb.Entity<PostTypeField>().HasIndex(p => new { p.TypeId, p.InternalId }).IsUnique();

			mb.Entity<Tag>().ToTable("Piranha_Tags");
			mb.Entity<Tag>().Property(t => t.Title).HasMaxLength(64).IsRequired();
			mb.Entity<Tag>().Property(t => t.Slug).HasMaxLength(64).IsRequired();
			mb.Entity<Tag>().HasIndex(t => t.Slug).IsUnique();

			base.OnModelCreating(mb);
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
		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
			OnSave();
			return base.SaveChangesAsync(cancellationToken);
		}

		/// <summary>
		/// Saves the changes made to the context.
		/// </summary>
		/// <returns>The number of saved rows</returns>
		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) {
			OnSave();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		#region Private methods
		/// <summary>
		/// Pre process the
		/// </summary>
		private void OnSave() {
			foreach (var entry in ChangeTracker.Entries()) {
				var now = DateTime.Now;

				if (entry.State == EntityState.Added) {
					// Should we auto generate a unique id
					if (entry.Entity is Data.IModel && ((Data.IModel)entry.Entity).Id == Guid.Empty) 
						((Data.IModel)entry.Entity).Id = Guid.NewGuid();
					
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
					// Shoud we set modified date
					if (entry.Entity is IModified)
						((IModified)entry.Entity).LastModified = now;

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
