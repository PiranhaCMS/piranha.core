using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Piranha.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchiveRoute = table.Column<string>(nullable: true),
                    ArchiveTitle = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    HasArchive = table.Column<bool>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_MediaFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFolder", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_PageTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    InternalId = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Route = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageType", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_Params",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    InternalId = table.Column<string>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Param", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "PostType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    InternalId = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Route = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostType", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Filename = table.Column<string>(nullable: false),
                    FolderId = table.Column<Guid>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    PublicUrl = table.Column<string>(nullable: false),
                    Size = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_MediaFolder_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Piranha_MediaFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    IsHidden = table.Column<bool>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    MetaDescription = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    NavigationTitle = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    Route = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Page_Author_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Piranha_Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Page_PageType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Piranha_PageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_PageTypeFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(nullable: false),
                    InternalId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTypeField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTypeField_PageType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Piranha_PageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Excerpt = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    MetaDescription = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    Route = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Author_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Piranha_Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Post_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Piranha_Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_PostType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PostType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_PostTypeFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(nullable: false),
                    InternalId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTypeField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostTypeField_PostType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PostType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_PageFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageField_Page_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageField_PageTypeField_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Piranha_PageTypeFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Piranha_PostFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostField_Post_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostField_PostTypeField_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Piranha_PostTypeFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Category_Slug",
                table: "Piranha_Categories",
                column: "Slug",
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_Page_Slug",
                table: "Piranha_Pages",
                column: "Slug",
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_PageField_ParentId_TypeId",
                table: "Piranha_PageFields",
                columns: new[] { "ParentId", "TypeId" },
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_PageType_InternalId",
                table: "Piranha_PageTypes",
                column: "InternalId",
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_PageTypeField_TypeId_InternalId",
                table: "Piranha_PageTypeFields",
                columns: new[] { "TypeId", "InternalId" },
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_Param_InternalId",
                table: "Piranha_Params",
                column: "InternalId",
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_Post_CategoryId_Slug",
                table: "Piranha_Posts",
                columns: new[] { "CategoryId", "Slug" });
            migrationBuilder.CreateIndex(
                name: "IX_PostField_ParentId_TypeId",
                table: "Piranha_PostFields",
                columns: new[] { "ParentId", "TypeId" },
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_PostTypeField_TypeId_InternalId",
                table: "Piranha_PostTypeFields",
                columns: new[] { "TypeId", "InternalId" },
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_Tag_Slug",
                table: "Piranha_Tags",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Piranha_Media");
            migrationBuilder.DropTable("Piranha_PageFields");
            migrationBuilder.DropTable("Piranha_Params");
            migrationBuilder.DropTable("Piranha_PostFields");
            migrationBuilder.DropTable("Piranha_Tags");
            migrationBuilder.DropTable("Piranha_MediaFolders");
            migrationBuilder.DropTable("Piranha_Pages");
            migrationBuilder.DropTable("Piranha_PageTypeFields");
            migrationBuilder.DropTable("Piranha_Posts");
            migrationBuilder.DropTable("Piranha_PostTypeFields");
            migrationBuilder.DropTable("Piranha_PageTypes");
            migrationBuilder.DropTable("Piranha_Authors");
            migrationBuilder.DropTable("Piranha_Categories");
            migrationBuilder.DropTable("PostType");
        }
    }
}
