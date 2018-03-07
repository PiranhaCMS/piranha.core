/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Piranha
{
    public sealed class Db : DbContext, IDb
    {
        /// <summary>
        /// Gets/sets whether the db context as been initialized. This
        /// is only performed once in the application lifecycle.
        /// </summary>
        static bool isInitialized = false;

        /// <summary>
        /// The object mutext used for initializing the context.
        /// </summary>
        static object mutex = new object();

        /// <summary>
        /// Gets/sets the alias set.
        /// </summary>
        public DbSet<Data.Alias> Aliases { get; set; }

        /// <summary>
        /// Gets/sets the category set.
        /// </summary>
        public DbSet<Data.Category> Categories { get; set; }

        /// <summary>
        /// Gets/sets the media set.
        /// </summary>
        public DbSet<Data.Media> Media { get; set; }

        /// <summary>
        /// Gets/sets the media folder set.
        /// </summary>
        public DbSet<Data.MediaFolder> MediaFolders { get; set; }

        /// <summary>
        /// Gets/sets the media version set.
        /// </summary>
        public DbSet<Data.MediaVersion> MediaVersions { get; set; }

        /// <summary>
        /// Gets/sets the page set.
        /// </summary>
        public DbSet<Data.Page> Pages { get; set; }

        /// <summary>
        /// Gets/sets the page field set.
        /// </summary>
        public DbSet<Data.PageField> PageFields { get; set; }

        /// <summary>
        /// Gets/sets the page type set.
        /// </summary>
        public DbSet<Data.PageType> PageTypes { get; set; }

        /// <summary>
        /// Gets/sets the param set.
        /// </summary>
        public DbSet<Data.Param> Params { get; set; }

        /// <summary>
        /// Gets/sets the post set.
        /// </summary>
        public DbSet<Data.Post> Posts { get; set; }

        /// <summary>
        /// Gets/sets the post field set.
        /// </summary>        
        public DbSet<Data.PostField> PostFields { get; set; }

        /// <summary>
        /// Gets/sets the post tag set.
        /// </summary>
        public DbSet<Data.PostTag> PostTags { get; set; }

        /// <summary>
        /// Gets/sets the post type set.
        /// </summary>
        public DbSet<Data.PostType> PostTypes { get; set; }

        /// <summary>
        /// Gets/sets the site set.
        /// </summary>
        public DbSet<Data.Site> Sites { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        public DbSet<Data.Tag> Tags { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public Db(DbContextOptions<Db> options) : base(options) {
            if (!isInitialized) {
                lock (mutex) {
                    if (!isInitialized) {
                        // Migrate database
                        Database.Migrate();
                        // Seed
                        Seed();

                        isInitialized = true;
                    }
                }
            }
        }        

        /// <summary>
        /// Creates and configures the data model.
        /// </summary>
        /// <param name="mb">The current model builder</param>
        protected override void OnModelCreating(ModelBuilder mb) {
            mb.Entity<Data.Alias>().ToTable("Piranha_Aliases");
            mb.Entity<Data.Alias>().Property(a => a.AliasUrl).IsRequired().HasMaxLength(256);
            mb.Entity<Data.Alias>().Property(a => a.RedirectUrl).IsRequired().HasMaxLength(256);
            mb.Entity<Data.Alias>().HasIndex(a => new { a.SiteId, a.AliasUrl }).IsUnique();

            mb.Entity<Data.Category>().ToTable("Piranha_Categories");
            mb.Entity<Data.Category>().Property(c => c.Title).IsRequired().HasMaxLength(64);
            mb.Entity<Data.Category>().Property(c => c.Slug).IsRequired().HasMaxLength(64);
            mb.Entity<Data.Category>().HasIndex(c => new { c.BlogId, c.Slug }).IsUnique();

            mb.Entity<Data.Media>().ToTable("Piranha_Media");
            mb.Entity<Data.Media>().Property(m => m.Filename).HasMaxLength(128).IsRequired();
            mb.Entity<Data.Media>().Property(m => m.ContentType).HasMaxLength(256).IsRequired();

            mb.Entity<Data.MediaFolder>().ToTable("Piranha_MediaFolders");
            mb.Entity<Data.MediaFolder>().Property(f => f.Name).HasMaxLength(128).IsRequired();

            mb.Entity<Data.MediaVersion>().ToTable("Piranha_MediaVersions");
            mb.Entity<Data.MediaVersion>().HasIndex(v => new { v.MediaId, v.Width, v.Height }).IsUnique();

            mb.Entity<Data.Page>().ToTable("Piranha_Pages");
            mb.Entity<Data.Page>().Property(p => p.PageTypeId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.Page>().Property(p => p.ContentType).HasMaxLength(255).IsRequired().HasDefaultValue("Page");
            mb.Entity<Data.Page>().Property(p => p.Title).HasMaxLength(128).IsRequired();
            mb.Entity<Data.Page>().Property(p => p.NavigationTitle).HasMaxLength(128);
            mb.Entity<Data.Page>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
            mb.Entity<Data.Page>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Data.Page>().Property(p => p.MetaDescription).HasMaxLength(256);
            mb.Entity<Data.Page>().Property(p => p.Route).HasMaxLength(256);
            mb.Entity<Data.Page>().Property(p => p.RedirectUrl).HasMaxLength(256);
            mb.Entity<Data.Page>().HasIndex(p => new { p.SiteId, p.Slug }).IsUnique();

            mb.Entity<Data.PageField>().ToTable("Piranha_PageFields");
            mb.Entity<Data.PageField>().Property(f => f.RegionId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.PageField>().Property(f => f.FieldId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.PageField>().Property(f => f.ClrType).HasMaxLength(256).IsRequired();
            mb.Entity<Data.PageField>().HasIndex(f => new { f.PageId, f.RegionId, f.FieldId, f.SortOrder });

            mb.Entity<Data.PageType>().ToTable("Piranha_PageTypes");
            mb.Entity<Data.PageType>().Property(p => p.Id).HasMaxLength(64).IsRequired();

            mb.Entity<Data.Param>().ToTable("Piranha_Params");
            mb.Entity<Data.Param>().Property(p => p.Key).HasMaxLength(64).IsRequired();
            mb.Entity<Data.Param>().Property(p => p.Description).HasMaxLength(256);
            mb.Entity<Data.Param>().HasIndex(p => p.Key).IsUnique();

            mb.Entity<Data.Post>().ToTable("Piranha_Posts");
            mb.Entity<Data.Post>().Property(p => p.PostTypeId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.Post>().Property(p => p.Title).HasMaxLength(128).IsRequired();
            mb.Entity<Data.Post>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
            mb.Entity<Data.Post>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Data.Post>().Property(p => p.MetaDescription).HasMaxLength(256);
            mb.Entity<Data.Post>().Property(p => p.Route).HasMaxLength(256);
            mb.Entity<Data.Post>().Property(p => p.RedirectUrl).HasMaxLength(256);
            mb.Entity<Data.Post>().HasOne(p => p.Category).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);
            mb.Entity<Data.Post>().HasIndex(p => new { p.BlogId, p.Slug }).IsUnique();

            mb.Entity<Data.PostField>().ToTable("Piranha_PostFields");
            mb.Entity<Data.PostField>().Property(f => f.RegionId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.PostField>().Property(f => f.FieldId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.PostField>().Property(f => f.ClrType).HasMaxLength(256).IsRequired();
            mb.Entity<Data.PostField>().HasIndex(f => new { f.PostId, f.RegionId, f.FieldId, f.SortOrder });

            mb.Entity<Data.PostTag>().ToTable("Piranha_PostTags");
            mb.Entity<Data.PostTag>().HasKey(t => new { t.PostId, t.TagId });
            mb.Entity<Data.PostTag>().HasOne(t => t.Tag).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);

            mb.Entity<Data.PostType>().ToTable("Piranha_PostTypes");
            mb.Entity<Data.PostType>().Property(p => p.Id).HasMaxLength(64).IsRequired();

            mb.Entity<Data.Site>().ToTable("Piranha_Sites");
            mb.Entity<Data.Site>().Property(s => s.InternalId).HasMaxLength(64).IsRequired();
            mb.Entity<Data.Site>().Property(s => s.Title).HasMaxLength(128);
            mb.Entity<Data.Site>().Property(s => s.Description).HasMaxLength(256);
            mb.Entity<Data.Site>().Property(s => s.Hostnames).HasMaxLength(256);
            mb.Entity<Data.Site>().HasIndex(s => s.InternalId).IsUnique();

            mb.Entity<Data.Tag>().ToTable("Piranha_Tags");
            mb.Entity<Data.Tag>().Property(t => t.Title).IsRequired().HasMaxLength(64);
            mb.Entity<Data.Tag>().Property(t => t.Slug).IsRequired().HasMaxLength(64);
            mb.Entity<Data.Tag>().HasIndex(t => new { t.BlogId, t.Slug }).IsUnique();
        }

        /// <summary>
        /// Seeds the default data.
        /// </summary>
        private void Seed() {
            SaveChanges();

            //
            // Params
            //
            var param = Params.FirstOrDefault(p => p.Key == Config.ARCHIVE_PAGE_SIZE);
            if (param == null)
                Params.Add(new Data.Param() {
                    Id = Guid.NewGuid(),
                    Key = Config.ARCHIVE_PAGE_SIZE,
                    Value = 5.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });

            param = Params.FirstOrDefault(p => p.Key == Config.CACHE_EXPIRES_PAGES);
            if (param == null)
                Params.Add(new Data.Param() {
                    Id = Guid.NewGuid(),
                    Key = Config.CACHE_EXPIRES_PAGES,
                    Value = 0.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });

            param = Params.FirstOrDefault(p => p.Key == Config.CACHE_EXPIRES_POSTS);
            if (param == null)
                Params.Add(new Data.Param() {
                    Id = Guid.NewGuid(),
                    Key = Config.CACHE_EXPIRES_POSTS,
                    Value = 0.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });

            param = Params.FirstOrDefault(p => p.Key == Config.PAGES_HIERARCHICAL_SLUGS);
            if (param == null)
                Params.Add(new Data.Param() {
                    Id = Guid.NewGuid(),
                    Key = Config.PAGES_HIERARCHICAL_SLUGS,
                    Value = true.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });

            param = Params.FirstOrDefault(p => p.Key == Config.MANAGER_EXPANDED_SITEMAP_LEVELS);
            if (param == null)
                Params.Add(new Data.Param() {
                    Id = Guid.NewGuid(),
                    Key = Config.MANAGER_EXPANDED_SITEMAP_LEVELS,
                    Value = 1.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });

            //
            // Default site
            //
            if (Sites.Count() == 0)
                Sites.Add(new Data.Site() {
                    Id = Guid.NewGuid(),
                    InternalId = "Default",
                    IsDefault = true,
                    Title = "Default Site",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });

            SaveChanges();
        }
    }
}