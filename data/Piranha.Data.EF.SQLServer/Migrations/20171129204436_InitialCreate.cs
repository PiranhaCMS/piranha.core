using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Piranha.Data.EF.SQLServer.Migrations
{
    [NoCoverage]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_MediaFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    ParentId = table.Column<Guid>(nullable: true)
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
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Params", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Hostnames = table.Column<string>(maxLength: 256, nullable: true),
                    InternalId = table.Column<string>(maxLength: 64, nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Filename = table.Column<string>(maxLength: 128, nullable: false),
                    FolderId = table.Column<Guid>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    PublicUrl = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false)
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
                name: "Piranha_Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    IsHidden = table.Column<bool>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    MetaDescription = table.Column<string>(maxLength: 256, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
                    NavigationTitle = table.Column<string>(maxLength: 128, nullable: true),
                    PageTypeId = table.Column<string>(maxLength: 64, nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Published = table.Column<DateTime>(nullable: true),
                    RedirectType = table.Column<int>(nullable: false),
                    RedirectUrl = table.Column<string>(maxLength: 256, nullable: true),
                    Route = table.Column<string>(maxLength: 256, nullable: true),
                    SiteId = table.Column<Guid>(nullable: false),
                    Slug = table.Column<string>(maxLength: 128, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false)
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
                name: "Piranha_PageFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    PageId = table.Column<Guid>(nullable: false),
                    RegionId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Media_FolderId",
                table: "Piranha_Media",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageFields_PageId_RegionId_FieldId_SortOrder",
                table: "Piranha_PageFields",
                columns: new[] { "PageId", "RegionId", "FieldId", "SortOrder" });

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
                name: "IX_Piranha_Sites_InternalId",
                table: "Piranha_Sites",
                column: "InternalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_Media");

            migrationBuilder.DropTable(
                name: "Piranha_PageFields");

            migrationBuilder.DropTable(
                name: "Piranha_Params");

            migrationBuilder.DropTable(
                name: "Piranha_MediaFolders");

            migrationBuilder.DropTable(
                name: "Piranha_Pages");

            migrationBuilder.DropTable(
                name: "Piranha_PageTypes");

            migrationBuilder.DropTable(
                name: "Piranha_Sites");
        }
    }
}
