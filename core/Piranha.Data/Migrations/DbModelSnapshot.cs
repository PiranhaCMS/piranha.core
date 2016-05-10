using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Piranha;

namespace Piranha.Data.Migrations
{
    [DbContext(typeof(Db))]
    partial class DbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Piranha.Data.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 512);

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("UserId")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Piranha_Authors");
                });

            modelBuilder.Entity("Piranha.Data.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ArchiveRoute")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("ArchiveTitle")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 512);

                    b.Property<bool>("HasArchive");

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

                    b.HasAnnotation("Relational:TableName", "Piranha_Categories");
                });

            modelBuilder.Entity("Piranha.Data.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 255);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<Guid?>("FolderId");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("PublicUrl")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 255);

                    b.Property<long>("Size");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Piranha_Media");
                });

            modelBuilder.Entity("Piranha.Data.MediaFolder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<Guid?>("ParentId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Piranha_MediaFolders");
                });

            modelBuilder.Entity("Piranha.Data.Page", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AuthorId");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsHidden");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("MetaDescription")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("MetaKeywords")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("MetaTitle")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("NavigationTitle")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<Guid?>("ParentId");

                    b.Property<DateTime?>("Published");

                    b.Property<string>("Route")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<int>("SortOrder");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_Pages");
                });

            modelBuilder.Entity("Piranha.Data.PageField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ParentId");

                    b.Property<Guid>("TypeId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ParentId", "TypeId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_PageFields");
                });

            modelBuilder.Entity("Piranha.Data.PageType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("InternalId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Route")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasIndex("InternalId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_PageTypes");
                });

            modelBuilder.Entity("Piranha.Data.PageTypeField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CLRType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 512);

                    b.Property<int>("FieldType");

                    b.Property<string>("InternalId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<int>("SortOrder");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("TypeId", "FieldType", "InternalId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_PageTypeFields");
                });

            modelBuilder.Entity("Piranha.Data.Param", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("InternalId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("InternalId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_Params");
                });

            modelBuilder.Entity("Piranha.Data.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AuthorId");

                    b.Property<Guid>("CategoryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Excerpt")
                        .HasAnnotation("MaxLength", 512);

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("MetaDescription")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("MetaKeywords")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("MetaTitle")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<DateTime?>("Published");

                    b.Property<string>("Route")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId", "Slug");

                    b.HasAnnotation("Relational:TableName", "Piranha_Posts");
                });

            modelBuilder.Entity("Piranha.Data.PostField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ParentId");

                    b.Property<Guid>("TypeId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ParentId", "TypeId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_PostFields");
                });

            modelBuilder.Entity("Piranha.Data.PostType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("InternalId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Route")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasIndex("InternalId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_PostTypes");
                });

            modelBuilder.Entity("Piranha.Data.PostTypeField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CLRType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 512);

                    b.Property<int>("FieldType");

                    b.Property<string>("InternalId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<int>("SortOrder");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("TypeId", "InternalId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_PostTypeFields");
                });

            modelBuilder.Entity("Piranha.Data.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64);

                    b.HasKey("Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "Piranha_Tags");
                });

            modelBuilder.Entity("Piranha.Data.Media", b =>
                {
                    b.HasOne("Piranha.Data.MediaFolder")
                        .WithMany()
                        .HasForeignKey("FolderId");
                });

            modelBuilder.Entity("Piranha.Data.Page", b =>
                {
                    b.HasOne("Piranha.Data.Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Piranha.Data.PageType")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Piranha.Data.PageField", b =>
                {
                    b.HasOne("Piranha.Data.Page")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("Piranha.Data.PageTypeField")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Piranha.Data.PageTypeField", b =>
                {
                    b.HasOne("Piranha.Data.PageType")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Piranha.Data.Post", b =>
                {
                    b.HasOne("Piranha.Data.Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Piranha.Data.Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Piranha.Data.PostType")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Piranha.Data.PostField", b =>
                {
                    b.HasOne("Piranha.Data.Post")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("Piranha.Data.PostTypeField")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("Piranha.Data.PostTypeField", b =>
                {
                    b.HasOne("Piranha.Data.PostType")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });
        }
    }
}
