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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

namespace Piranha
{
    public sealed class Db : DbContext, IDb
    {
        /// <summary>
        /// Gets/sets whether the db context as been initialized. This
        /// is only performed once in the application lifecycle.
        /// </summary>
        static bool _isInitialized;

        /// <summary>
        /// The object mutext used for initializing the context.
        /// </summary>
        static readonly object Mutex = new object();

        /// <summary>
        /// Gets/sets the alias set.
        /// </summary>
        public DbSet<Alias> Aliases { get; set; }

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
        /// Gets/sets the media version set.
        /// </summary>
        public DbSet<MediaVersion> MediaVersions { get; set; }

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
        /// Gets/sets the post tag set.
        /// </summary>
        public DbSet<PostTag> PostTags { get; set; }

        /// <summary>
        /// Gets/sets the post type set.
        /// </summary>
        public DbSet<PostType> PostTypes { get; set; }

        /// <summary>
        /// Gets/sets the site set.
        /// </summary>
        public DbSet<Site> Sites { get; set; }

        /// <summary>
        /// Gets/sets the tag set.
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public Db(DbContextOptions<Db> options) : base(options)
        {
            if (!_isInitialized)
            {
                lock (Mutex)
                {
                    if (!_isInitialized)
                    {
                        // Migrate database
                        Database.Migrate();
                        // Seed
                        Seed();

                        _isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Creates and configures the data model.
        /// </summary>
        /// <param name="mb">The current model builder</param>
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Alias>().ToTable("Piranha_Aliases");
            mb.Entity<Alias>().Property(a => a.AliasUrl).IsRequired().HasMaxLength(256);
            mb.Entity<Alias>().Property(a => a.RedirectUrl).IsRequired().HasMaxLength(256);
            mb.Entity<Alias>().HasIndex(a => new { a.SiteId, a.AliasUrl }).IsUnique();

            mb.Entity<Category>().ToTable("Piranha_Categories");
            mb.Entity<Category>().Property(c => c.Title).IsRequired().HasMaxLength(64);
            mb.Entity<Category>().Property(c => c.Slug).IsRequired().HasMaxLength(64);
            mb.Entity<Category>().HasIndex(c => new { c.BlogId, c.Slug }).IsUnique();

            mb.Entity<Media>().ToTable("Piranha_Media");
            mb.Entity<Media>().Property(m => m.Filename).HasMaxLength(128).IsRequired();
            mb.Entity<Media>().Property(m => m.ContentType).HasMaxLength(256).IsRequired();

            mb.Entity<MediaFolder>().ToTable("Piranha_MediaFolders");
            mb.Entity<MediaFolder>().Property(f => f.Name).HasMaxLength(128).IsRequired();

            mb.Entity<MediaVersion>().ToTable("Piranha_MediaVersions");
            mb.Entity<MediaVersion>().HasIndex(v => new { v.MediaId, v.Width, v.Height }).IsUnique();

            mb.Entity<Page>().ToTable("Piranha_Pages");
            mb.Entity<Page>().Property(p => p.PageTypeId).HasMaxLength(64).IsRequired();
            mb.Entity<Page>().Property(p => p.ContentType).HasMaxLength(255).IsRequired().HasDefaultValue("Page");
            mb.Entity<Page>().Property(p => p.Title).HasMaxLength(128).IsRequired();
            mb.Entity<Page>().Property(p => p.NavigationTitle).HasMaxLength(128);
            mb.Entity<Page>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
            mb.Entity<Page>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Page>().Property(p => p.MetaDescription).HasMaxLength(256);
            mb.Entity<Page>().Property(p => p.Route).HasMaxLength(256);
            mb.Entity<Page>().Property(p => p.RedirectUrl).HasMaxLength(256);
            mb.Entity<Page>().HasIndex(p => new { p.SiteId, p.Slug }).IsUnique();

            mb.Entity<PageField>().ToTable("Piranha_PageFields");
            mb.Entity<PageField>().Property(f => f.RegionId).HasMaxLength(64).IsRequired();
            mb.Entity<PageField>().Property(f => f.FieldId).HasMaxLength(64).IsRequired();
            mb.Entity<PageField>().Property(f => f.ClrType).HasMaxLength(256).IsRequired();
            mb.Entity<PageField>().HasIndex(f => new { f.PageId, f.RegionId, f.FieldId, f.SortOrder });

            mb.Entity<PageType>().ToTable("Piranha_PageTypes");
            mb.Entity<PageType>().Property(p => p.Id).HasMaxLength(64).IsRequired();

            mb.Entity<Param>().ToTable("Piranha_Params");
            mb.Entity<Param>().Property(p => p.Key).HasMaxLength(64).IsRequired();
            mb.Entity<Param>().Property(p => p.Description).HasMaxLength(256);
            mb.Entity<Param>().HasIndex(p => p.Key).IsUnique();

            mb.Entity<Post>().ToTable("Piranha_Posts");
            mb.Entity<Post>().Property(p => p.PostTypeId).HasMaxLength(64).IsRequired();
            mb.Entity<Post>().Property(p => p.Title).HasMaxLength(128).IsRequired();
            mb.Entity<Post>().Property(p => p.Slug).HasMaxLength(128).IsRequired();
            mb.Entity<Post>().Property(p => p.MetaKeywords).HasMaxLength(128);
            mb.Entity<Post>().Property(p => p.MetaDescription).HasMaxLength(256);
            mb.Entity<Post>().Property(p => p.Route).HasMaxLength(256);
            mb.Entity<Post>().Property(p => p.RedirectUrl).HasMaxLength(256);
            mb.Entity<Post>().HasOne(p => p.Category).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);
            mb.Entity<Post>().HasIndex(p => new { p.BlogId, p.Slug }).IsUnique();

            mb.Entity<PostField>().ToTable("Piranha_PostFields");
            mb.Entity<PostField>().Property(f => f.RegionId).HasMaxLength(64).IsRequired();
            mb.Entity<PostField>().Property(f => f.FieldId).HasMaxLength(64).IsRequired();
            mb.Entity<PostField>().Property(f => f.ClrType).HasMaxLength(256).IsRequired();
            mb.Entity<PostField>().HasIndex(f => new { f.PostId, f.RegionId, f.FieldId, f.SortOrder });

            mb.Entity<PostTag>().ToTable("Piranha_PostTags");
            mb.Entity<PostTag>().HasKey(t => new { t.PostId, t.TagId });
            mb.Entity<PostTag>().HasOne(t => t.Tag).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);

            mb.Entity<PostType>().ToTable("Piranha_PostTypes");
            mb.Entity<PostType>().Property(p => p.Id).HasMaxLength(64).IsRequired();

            mb.Entity<Site>().ToTable("Piranha_Sites");
            mb.Entity<Site>().Property(s => s.InternalId).HasMaxLength(64).IsRequired();
            mb.Entity<Site>().Property(s => s.Title).HasMaxLength(128);
            mb.Entity<Site>().Property(s => s.Description).HasMaxLength(256);
            mb.Entity<Site>().Property(s => s.Hostnames).HasMaxLength(256);
            mb.Entity<Site>().HasIndex(s => s.InternalId).IsUnique();

            mb.Entity<Tag>().ToTable("Piranha_Tags");
            mb.Entity<Tag>().Property(t => t.Title).IsRequired().HasMaxLength(64);
            mb.Entity<Tag>().Property(t => t.Slug).IsRequired().HasMaxLength(64);
            mb.Entity<Tag>().HasIndex(t => new { t.BlogId, t.Slug }).IsUnique();
        }

        /// <summary>
        /// Seeds the default data.
        /// </summary>
        private void Seed()
        {
            SaveChanges();

            //
            // Params
            //
            var param = Params.FirstOrDefault(p => p.Key == Config.ARCHIVE_PAGE_SIZE);
            if (param == null)
            {
                Params.Add(new Param
                {
                    Id = Guid.NewGuid(),
                    Key = Config.ARCHIVE_PAGE_SIZE,
                    Value = 5.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });
            }

            param = Params.FirstOrDefault(p => p.Key == Config.CACHE_EXPIRES_PAGES);
            if (param == null)
            {
                Params.Add(new Param
                {
                    Id = Guid.NewGuid(),
                    Key = Config.CACHE_EXPIRES_PAGES,
                    Value = 0.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });
            }

            param = Params.FirstOrDefault(p => p.Key == Config.CACHE_EXPIRES_POSTS);
            if (param == null)
            {
                Params.Add(new Param
                {
                    Id = Guid.NewGuid(),
                    Key = Config.CACHE_EXPIRES_POSTS,
                    Value = 0.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });
            }

            param = Params.FirstOrDefault(p => p.Key == Config.PAGES_HIERARCHICAL_SLUGS);
            if (param == null)
            {
                Params.Add(new Param
                {
                    Id = Guid.NewGuid(),
                    Key = Config.PAGES_HIERARCHICAL_SLUGS,
                    Value = true.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });
            }

            param = Params.FirstOrDefault(p => p.Key == Config.MANAGER_EXPANDED_SITEMAP_LEVELS);
            if (param == null)
            {
                Params.Add(new Param
                {
                    Id = Guid.NewGuid(),
                    Key = Config.MANAGER_EXPANDED_SITEMAP_LEVELS,
                    Value = 1.ToString(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });
            }

            //
            // Default site
            //
            if (!Sites.Any())
            {
                Sites.Add(new Site
                {
                    Id = Guid.NewGuid(),
                    InternalId = "Default",
                    IsDefault = true,
                    Title = "Default Site",
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                });
            }

            SaveChanges();
        }
    }
}