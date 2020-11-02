using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_Blocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true),
                    IsReusable = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Blocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentGroups",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    CLRType = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Icon = table.Column<string>(maxLength: 64, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    CLRType = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Group = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Culture = table.Column<string>(maxLength: 6, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_MediaFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_MediaFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Params",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Params", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_SiteTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_SiteTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Taxonomies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    GroupId = table.Column<string>(maxLength: 64, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Taxonomies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_BlockFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlockId = table.Column<Guid>(nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_BlockFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_BlockFields_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    SiteTypeId = table.Column<string>(maxLength: 64, nullable: true),
                    InternalId = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    LogoId = table.Column<Guid>(nullable: true),
                    Hostnames = table.Column<string>(maxLength: 256, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    Culture = table.Column<string>(maxLength: 6, nullable: true),
                    ContentLastModified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Sites_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FolderId = table.Column<Guid>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(maxLength: 128, nullable: false),
                    ContentType = table.Column<string>(maxLength: 256, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true),
                    AltText = table.Column<string>(maxLength: 128, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Size = table.Column<long>(nullable: false),
                    PublicUrl = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Properties = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Media_Piranha_MediaFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Piranha_MediaFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Content",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: true),
                    TypeId = table.Column<string>(maxLength: 64, nullable: false),
                    PrimaryImageId = table.Column<Guid>(nullable: true),
                    Excerpt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Content", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Content_Piranha_Taxonomies_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Piranha_Taxonomies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Piranha_Content_Piranha_ContentTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Piranha_ContentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Aliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SiteId = table.Column<Guid>(nullable: false),
                    AliasUrl = table.Column<string>(maxLength: 256, nullable: false),
                    RedirectUrl = table.Column<string>(maxLength: 256, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Aliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Aliases_Piranha_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Piranha_Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(maxLength: 128, nullable: false),
                    MetaTitle = table.Column<string>(maxLength: 128, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 256, nullable: true),
                    MetaIndex = table.Column<bool>(nullable: false, defaultValue: true),
                    MetaFollow = table.Column<bool>(nullable: false, defaultValue: true),
                    MetaPriority = table.Column<double>(nullable: false, defaultValue: 0.5),
                    OgTitle = table.Column<string>(maxLength: 128, nullable: true),
                    OgDescription = table.Column<string>(maxLength: 256, nullable: true),
                    OgImageId = table.Column<Guid>(nullable: false),
                    Route = table.Column<string>(maxLength: 256, nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    PageTypeId = table.Column<string>(maxLength: 64, nullable: false),
                    SiteId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    ContentType = table.Column<string>(maxLength: 255, nullable: false, defaultValue: "Page"),
                    PrimaryImageId = table.Column<Guid>(nullable: true),
                    Excerpt = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    NavigationTitle = table.Column<string>(maxLength: 128, nullable: true),
                    IsHidden = table.Column<bool>(nullable: false),
                    RedirectUrl = table.Column<string>(maxLength: 256, nullable: true),
                    RedirectType = table.Column<int>(nullable: false),
                    EnableComments = table.Column<bool>(nullable: false, defaultValue: false),
                    CloseCommentsAfterDays = table.Column<int>(nullable: false),
                    OriginalPageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Pages_Piranha_PageTypes_PageTypeId",
                        column: x => x.PageTypeId,
                        principalTable: "Piranha_PageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_Pages_Piranha_Pages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Piranha_Pages_Piranha_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Piranha_Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_SiteFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegionId = table.Column<string>(maxLength: 64, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    SiteId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_SiteFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_SiteFields_Piranha_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Piranha_Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_MediaVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: true),
                    FileExtension = table.Column<string>(maxLength: 8, nullable: true),
                    MediaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_MediaVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_MediaVersions_Piranha_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Piranha_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegionId = table.Column<string>(maxLength: 64, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    ContentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentFields_Piranha_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Piranha_Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentTaxonomies",
                columns: table => new
                {
                    ContentId = table.Column<Guid>(nullable: false),
                    TaxonomyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentTaxonomies", x => new { x.ContentId, x.TaxonomyId });
                    table.ForeignKey(
                        name: "FK_Piranha_ContentTaxonomies_Piranha_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Piranha_Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentTaxonomies_Piranha_Taxonomies_TaxonomyId",
                        column: x => x.TaxonomyId,
                        principalTable: "Piranha_Taxonomies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentTranslations",
                columns: table => new
                {
                    ContentId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Excerpt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentTranslations", x => new { x.ContentId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_ContentTranslations_Piranha_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Piranha_Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    BlogId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Categories_Piranha_Pages_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    PageId = table.Column<Guid>(nullable: false),
                    BlockId = table.Column<Guid>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PageBlocks_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PageBlocks_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Author = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PageComments_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegionId = table.Column<string>(maxLength: 64, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    PageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PageFields_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PagePermissions",
                columns: table => new
                {
                    PageId = table.Column<Guid>(nullable: false),
                    Permission = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PagePermissions", x => new { x.PageId, x.Permission });
                    table.ForeignKey(
                        name: "FK_Piranha_PagePermissions_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PageRevisions_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    BlogId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Tags_Piranha_Pages_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentFieldTranslations",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(nullable: false),
                    LanguageId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentFieldTranslations", x => new { x.FieldId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_ContentFieldTranslations_Piranha_ContentFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Piranha_ContentFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentFieldTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(maxLength: 128, nullable: false),
                    MetaTitle = table.Column<string>(maxLength: 128, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 256, nullable: true),
                    MetaIndex = table.Column<bool>(nullable: false, defaultValue: true),
                    MetaFollow = table.Column<bool>(nullable: false, defaultValue: true),
                    MetaPriority = table.Column<double>(nullable: false, defaultValue: 0.5),
                    OgTitle = table.Column<string>(maxLength: 128, nullable: true),
                    OgDescription = table.Column<string>(maxLength: 256, nullable: true),
                    OgImageId = table.Column<Guid>(nullable: false),
                    Route = table.Column<string>(maxLength: 256, nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    PostTypeId = table.Column<string>(maxLength: 64, nullable: false),
                    BlogId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    PrimaryImageId = table.Column<Guid>(nullable: true),
                    Excerpt = table.Column<string>(nullable: true),
                    RedirectUrl = table.Column<string>(maxLength: 256, nullable: true),
                    RedirectType = table.Column<int>(nullable: false),
                    EnableComments = table.Column<bool>(nullable: false, defaultValue: false),
                    CloseCommentsAfterDays = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Posts_Piranha_Pages_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_Posts_Piranha_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Piranha_Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Piranha_Posts_Piranha_PostTypes_PostTypeId",
                        column: x => x.PostTypeId,
                        principalTable: "Piranha_PostTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    PostId = table.Column<Guid>(nullable: false),
                    BlockId = table.Column<Guid>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostBlocks_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PostBlocks_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: true),
                    Author = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostComments_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegionId = table.Column<string>(maxLength: 64, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostFields_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostPermissions",
                columns: table => new
                {
                    PostId = table.Column<Guid>(nullable: false),
                    Permission = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostPermissions", x => new { x.PostId, x.Permission });
                    table.ForeignKey(
                        name: "FK_Piranha_PostPermissions_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostRevisions_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostTags",
                columns: table => new
                {
                    PostId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_Piranha_PostTags_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PostTags_Piranha_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Piranha_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Aliases_SiteId_AliasUrl",
                table: "Piranha_Aliases",
                columns: new[] { "SiteId", "AliasUrl" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_BlockFields_BlockId_FieldId_SortOrder",
                table: "Piranha_BlockFields",
                columns: new[] { "BlockId", "FieldId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Categories_BlogId_Slug",
                table: "Piranha_Categories",
                columns: new[] { "BlogId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Content_CategoryId",
                table: "Piranha_Content",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Content_TypeId",
                table: "Piranha_Content",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentFields_ContentId_RegionId_FieldId_SortOrder",
                table: "Piranha_ContentFields",
                columns: new[] { "ContentId", "RegionId", "FieldId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentFieldTranslations_LanguageId",
                table: "Piranha_ContentFieldTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentTaxonomies_TaxonomyId",
                table: "Piranha_ContentTaxonomies",
                column: "TaxonomyId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentTranslations_LanguageId",
                table: "Piranha_ContentTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Media_FolderId",
                table: "Piranha_Media",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_MediaVersions_MediaId_Width_Height",
                table: "Piranha_MediaVersions",
                columns: new[] { "MediaId", "Width", "Height" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageBlocks_BlockId",
                table: "Piranha_PageBlocks",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageBlocks_PageId_SortOrder",
                table: "Piranha_PageBlocks",
                columns: new[] { "PageId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageComments_PageId",
                table: "Piranha_PageComments",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageFields_PageId_RegionId_FieldId_SortOrder",
                table: "Piranha_PageFields",
                columns: new[] { "PageId", "RegionId", "FieldId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageRevisions_PageId",
                table: "Piranha_PageRevisions",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Pages_PageTypeId",
                table: "Piranha_Pages",
                column: "PageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Pages_ParentId",
                table: "Piranha_Pages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Pages_SiteId_Slug",
                table: "Piranha_Pages",
                columns: new[] { "SiteId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Params_Key",
                table: "Piranha_Params",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostBlocks_BlockId",
                table: "Piranha_PostBlocks",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostBlocks_PostId_SortOrder",
                table: "Piranha_PostBlocks",
                columns: new[] { "PostId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostComments_PostId",
                table: "Piranha_PostComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostFields_PostId_RegionId_FieldId_SortOrder",
                table: "Piranha_PostFields",
                columns: new[] { "PostId", "RegionId", "FieldId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostRevisions_PostId",
                table: "Piranha_PostRevisions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_CategoryId",
                table: "Piranha_Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_PostTypeId",
                table: "Piranha_Posts",
                column: "PostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_BlogId_Slug",
                table: "Piranha_Posts",
                columns: new[] { "BlogId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostTags_TagId",
                table: "Piranha_PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_SiteFields_SiteId_RegionId_FieldId_SortOrder",
                table: "Piranha_SiteFields",
                columns: new[] { "SiteId", "RegionId", "FieldId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Sites_InternalId",
                table: "Piranha_Sites",
                column: "InternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Sites_LanguageId",
                table: "Piranha_Sites",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Tags_BlogId_Slug",
                table: "Piranha_Tags",
                columns: new[] { "BlogId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Taxonomies_GroupId_Type_Slug",
                table: "Piranha_Taxonomies",
                columns: new[] { "GroupId", "Type", "Slug" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_Aliases");

            migrationBuilder.DropTable(
                name: "Piranha_BlockFields");

            migrationBuilder.DropTable(
                name: "Piranha_ContentFieldTranslations");

            migrationBuilder.DropTable(
                name: "Piranha_ContentGroups");

            migrationBuilder.DropTable(
                name: "Piranha_ContentTaxonomies");

            migrationBuilder.DropTable(
                name: "Piranha_ContentTranslations");

            migrationBuilder.DropTable(
                name: "Piranha_MediaVersions");

            migrationBuilder.DropTable(
                name: "Piranha_PageBlocks");

            migrationBuilder.DropTable(
                name: "Piranha_PageComments");

            migrationBuilder.DropTable(
                name: "Piranha_PageFields");

            migrationBuilder.DropTable(
                name: "Piranha_PagePermissions");

            migrationBuilder.DropTable(
                name: "Piranha_PageRevisions");

            migrationBuilder.DropTable(
                name: "Piranha_Params");

            migrationBuilder.DropTable(
                name: "Piranha_PostBlocks");

            migrationBuilder.DropTable(
                name: "Piranha_PostComments");

            migrationBuilder.DropTable(
                name: "Piranha_PostFields");

            migrationBuilder.DropTable(
                name: "Piranha_PostPermissions");

            migrationBuilder.DropTable(
                name: "Piranha_PostRevisions");

            migrationBuilder.DropTable(
                name: "Piranha_PostTags");

            migrationBuilder.DropTable(
                name: "Piranha_SiteFields");

            migrationBuilder.DropTable(
                name: "Piranha_SiteTypes");

            migrationBuilder.DropTable(
                name: "Piranha_ContentFields");

            migrationBuilder.DropTable(
                name: "Piranha_Media");

            migrationBuilder.DropTable(
                name: "Piranha_Blocks");

            migrationBuilder.DropTable(
                name: "Piranha_Posts");

            migrationBuilder.DropTable(
                name: "Piranha_Tags");

            migrationBuilder.DropTable(
                name: "Piranha_Content");

            migrationBuilder.DropTable(
                name: "Piranha_MediaFolders");

            migrationBuilder.DropTable(
                name: "Piranha_Categories");

            migrationBuilder.DropTable(
                name: "Piranha_PostTypes");

            migrationBuilder.DropTable(
                name: "Piranha_Taxonomies");

            migrationBuilder.DropTable(
                name: "Piranha_ContentTypes");

            migrationBuilder.DropTable(
                name: "Piranha_Pages");

            migrationBuilder.DropTable(
                name: "Piranha_PageTypes");

            migrationBuilder.DropTable(
                name: "Piranha_Sites");

            migrationBuilder.DropTable(
                name: "Piranha_Languages");
        }
    }
}
