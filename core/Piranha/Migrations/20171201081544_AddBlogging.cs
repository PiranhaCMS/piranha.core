using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddBlogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArchiveDescription = table.Column<string>(nullable: true),
                    ArchiveKeywords = table.Column<string>(maxLength: 128, nullable: true),
                    ArchiveRoute = table.Column<string>(maxLength: 256, nullable: true),
                    ArchiveTitle = table.Column<string>(maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    EnableArchive = table.Column<bool>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostTypes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(maxLength: 64, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    MetaDescription = table.Column<string>(maxLength: 256, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
                    PostTypeId = table.Column<string>(maxLength: 64, nullable: false),
                    Published = table.Column<DateTime>(nullable: true),
                    RedirectType = table.Column<int>(nullable: false),
                    RedirectUrl = table.Column<string>(maxLength: 256, nullable: true),
                    Route = table.Column<string>(maxLength: 256, nullable: true),
                    Slug = table.Column<string>(maxLength: 128, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Posts_Piranha_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Piranha_Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_Posts_Piranha_PostTypes_PostTypeId",
                        column: x => x.PostTypeId,
                        principalTable: "Piranha_PostTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    PostId = table.Column<Guid>(nullable: false),
                    RegionId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Categories_Slug",
                table: "Piranha_Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostFields_PostId_RegionId_FieldId_SortOrder",
                table: "Piranha_PostFields",
                columns: new[] { "PostId", "RegionId", "FieldId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_PostTypeId",
                table: "Piranha_Posts",
                column: "PostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_CategoryId_Slug",
                table: "Piranha_Posts",
                columns: new[] { "CategoryId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostTags_TagId",
                table: "Piranha_PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Tags_Slug",
                table: "Piranha_Tags",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_PostFields");

            migrationBuilder.DropTable(
                name: "Piranha_PostTags");

            migrationBuilder.DropTable(
                name: "Piranha_Posts");

            migrationBuilder.DropTable(
                name: "Piranha_Tags");

            migrationBuilder.DropTable(
                name: "Piranha_Categories");

            migrationBuilder.DropTable(
                name: "Piranha_PostTypes");
        }
    }
}
