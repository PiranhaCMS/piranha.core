using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Piranha.EF;

namespace Piranha.EF.Migrations
{
    [DbContext(typeof(Db))]
    partial class DbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder) {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Piranha.EF.Data.Category", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("ArchiveDescription");

                b.Property<string>("ArchiveKeywords");

                b.Property<string>("ArchiveRoute");

                b.Property<string>("ArchiveTitle");

                b.Property<DateTime>("Created");

                b.Property<string>("Description")
                    .HasAnnotation("MaxLength", 512);

                b.Property<DateTime>("LastModified");

                b.Property<string>("Slug")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 64);

                b.Property<string>("Title")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 64);

                b.HasKey("Id");

                b.HasIndex("Slug")
                    .IsUnique();

                b.ToTable("Piranha_Categories");
            });

            modelBuilder.Entity("Piranha.EF.Data.Page", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<DateTime>("Created");

                b.Property<DateTime>("LastModified");

                b.Property<string>("MetaDescription")
                    .HasAnnotation("MaxLength", 255);

                b.Property<string>("MetaKeywords")
                    .HasAnnotation("MaxLength", 128);

                b.Property<string>("NavigationTitle")
                    .HasAnnotation("MaxLength", 128);

                b.Property<string>("PageTypeId")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 32);

                b.Property<Guid?>("ParentId");

                b.Property<DateTime?>("Published");

                b.Property<string>("Route")
                    .HasAnnotation("MaxLength", 255);

                b.Property<string>("Slug")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 128);

                b.Property<int>("SortOrder");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 128);

                b.HasKey("Id");

                b.HasIndex("PageTypeId");

                b.HasIndex("Slug")
                    .IsUnique();

                b.ToTable("Piranha_Pages");
            });

            modelBuilder.Entity("Piranha.EF.Data.PageField", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("CLRType")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 255);

                b.Property<string>("FieldId")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 32);

                b.Property<Guid>("PageId");

                b.Property<string>("RegionId")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 32);

                b.Property<int>("SortOrder");

                b.Property<string>("Value");

                b.HasKey("Id");

                b.HasIndex("PageId");

                b.HasIndex("PageId", "RegionId", "FieldId", "SortOrder")
                    .IsUnique();

                b.ToTable("Piranha_PageFields");
            });

            modelBuilder.Entity("Piranha.EF.Data.PageType", b => {
                b.Property<string>("Id")
                    .HasAnnotation("MaxLength", 32);

                b.Property<string>("Body");

                b.Property<DateTime>("Created");

                b.Property<DateTime>("LastModified");

                b.HasKey("Id");

                b.ToTable("Piranha_PageTypes");
            });

            modelBuilder.Entity("Piranha.EF.Data.Post", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Body");

                b.Property<Guid>("CategoryId");

                b.Property<DateTime>("Created");

                b.Property<string>("Excerpt")
                    .HasAnnotation("MaxLength", 512);

                b.Property<DateTime>("LastModified");

                b.Property<string>("MetaDescription")
                    .HasAnnotation("MaxLength", 255);

                b.Property<string>("MetaKeywords")
                    .HasAnnotation("MaxLength", 128);

                b.Property<DateTime?>("Published");

                b.Property<string>("Route")
                    .HasAnnotation("MaxLength", 255);

                b.Property<string>("Slug")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 128);

                b.Property<string>("Title")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 128);

                b.HasKey("Id");

                b.HasIndex("CategoryId");

                b.HasIndex("CategoryId", "Slug")
                    .IsUnique();

                b.ToTable("Piranha_Posts");
            });

            modelBuilder.Entity("Piranha.EF.Data.Tag", b => {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<DateTime>("Created");

                b.Property<DateTime>("LastModified");

                b.Property<string>("Slug")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 64);

                b.Property<string>("Title")
                    .IsRequired()
                    .HasAnnotation("MaxLength", 64);

                b.HasKey("Id");

                b.HasIndex("Slug")
                    .IsUnique();

                b.ToTable("Piranha_Tags");
            });

            modelBuilder.Entity("Piranha.EF.Data.Page", b => {
                b.HasOne("Piranha.EF.Data.PageType", "PageType")
                    .WithMany("Pages")
                    .HasForeignKey("PageTypeId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Piranha.EF.Data.PageField", b => {
                b.HasOne("Piranha.EF.Data.Page", "Page")
                    .WithMany("Fields")
                    .HasForeignKey("PageId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Piranha.EF.Data.Post", b => {
                b.HasOne("Piranha.EF.Data.Category", "Category")
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
