using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.EF.Migrations
{
	public partial class InitialMigration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "Piranha_Categories",
				columns: table => new {
					Id = table.Column<Guid>(nullable: false),
					ArchiveDescription = table.Column<string>(nullable: true),
					ArchiveKeywords = table.Column<string>(nullable: true),
					ArchiveRoute = table.Column<string>(nullable: true),
					ArchiveTitle = table.Column<string>(nullable: true),
					Created = table.Column<DateTime>(nullable: false),
					Description = table.Column<string>(maxLength: 512, nullable: true),
					LastModified = table.Column<DateTime>(nullable: false),
					Slug = table.Column<string>(maxLength: 64, nullable: false),
					Title = table.Column<string>(maxLength: 64, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Piranha_Categories", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Piranha_PageTypes",
				columns: table => new {
					Id = table.Column<string>(maxLength: 32, nullable: false),
					Body = table.Column<string>(nullable: true),
					Created = table.Column<DateTime>(nullable: false),
					LastModified = table.Column<DateTime>(nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Piranha_PageTypes", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Piranha_Tags",
				columns: table => new {
					Id = table.Column<Guid>(nullable: false),
					Created = table.Column<DateTime>(nullable: false),
					LastModified = table.Column<DateTime>(nullable: false),
					Slug = table.Column<string>(maxLength: 64, nullable: false),
					Title = table.Column<string>(maxLength: 64, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Piranha_Tags", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Piranha_Posts",
				columns: table => new {
					Id = table.Column<Guid>(nullable: false),
					Body = table.Column<string>(nullable: true),
					CategoryId = table.Column<Guid>(nullable: false),
					Created = table.Column<DateTime>(nullable: false),
					Excerpt = table.Column<string>(maxLength: 512, nullable: true),
					LastModified = table.Column<DateTime>(nullable: false),
					MetaDescription = table.Column<string>(maxLength: 255, nullable: true),
					MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
					Published = table.Column<DateTime>(nullable: true),
					Route = table.Column<string>(maxLength: 255, nullable: true),
					Slug = table.Column<string>(maxLength: 128, nullable: false),
					Title = table.Column<string>(maxLength: 128, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Piranha_Posts", x => x.Id);
					table.ForeignKey(
						name: "FK_Piranha_Posts_Piranha_Categories_CategoryId",
						column: x => x.CategoryId,
						principalTable: "Piranha_Categories",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Piranha_Pages",
				columns: table => new {
					Id = table.Column<Guid>(nullable: false),
					Created = table.Column<DateTime>(nullable: false),
					LastModified = table.Column<DateTime>(nullable: false),
					MetaDescription = table.Column<string>(maxLength: 255, nullable: true),
					MetaKeywords = table.Column<string>(maxLength: 128, nullable: true),
					NavigationTitle = table.Column<string>(maxLength: 128, nullable: true),
					PageTypeId = table.Column<string>(maxLength: 32, nullable: false),
					ParentId = table.Column<Guid>(nullable: true),
					Published = table.Column<DateTime>(nullable: true),
					Route = table.Column<string>(maxLength: 255, nullable: true),
					Slug = table.Column<string>(maxLength: 128, nullable: false),
					SortOrder = table.Column<int>(nullable: false),
					Title = table.Column<string>(maxLength: 128, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_Piranha_Pages", x => x.Id);
					table.ForeignKey(
						name: "FK_Piranha_Pages_Piranha_PageTypes_PageTypeId",
						column: x => x.PageTypeId,
						principalTable: "Piranha_PageTypes",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Piranha_PageFields",
				columns: table => new {
					Id = table.Column<Guid>(nullable: false),
					CLRType = table.Column<string>(maxLength: 255, nullable: false),
					FieldId = table.Column<string>(maxLength: 32, nullable: false),
					PageId = table.Column<Guid>(nullable: false),
					RegionId = table.Column<string>(maxLength: 32, nullable: false),
					SortOrder = table.Column<int>(nullable: false),
					Value = table.Column<string>(nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_Piranha_PageFields", x => x.Id);
					table.ForeignKey(
						name: "FK_Piranha_PageFields_Piranha_Pages_PageId",
						column: x => x.PageId,
						principalTable: "Piranha_Pages",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_Categories_Slug",
				table: "Piranha_Categories",
				column: "Slug",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_Pages_PageTypeId",
				table: "Piranha_Pages",
				column: "PageTypeId");

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_Pages_Slug",
				table: "Piranha_Pages",
				column: "Slug",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_PageFields_PageId",
				table: "Piranha_PageFields",
				column: "PageId");

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_PageFields_PageId_RegionId_FieldId_SortOrder",
				table: "Piranha_PageFields",
				columns: new[] { "PageId", "RegionId", "FieldId", "SortOrder" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_Posts_CategoryId",
				table: "Piranha_Posts",
				column: "CategoryId");

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_Posts_CategoryId_Slug",
				table: "Piranha_Posts",
				columns: new[] { "CategoryId", "Slug" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Piranha_Tags_Slug",
				table: "Piranha_Tags",
				column: "Slug",
				unique: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "Piranha_PageFields");

			migrationBuilder.DropTable(
				name: "Piranha_Posts");

			migrationBuilder.DropTable(
				name: "Piranha_Tags");

			migrationBuilder.DropTable(
				name: "Piranha_Pages");

			migrationBuilder.DropTable(
				name: "Piranha_Categories");

			migrationBuilder.DropTable(
				name: "Piranha_PageTypes");
		}
	}
}
