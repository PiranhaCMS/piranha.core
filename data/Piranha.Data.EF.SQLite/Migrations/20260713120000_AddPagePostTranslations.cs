using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddPagePostTranslations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_PageFieldTranslations",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageFieldTranslations", x => new { x.FieldId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_PageFieldTranslations_Piranha_PageFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Piranha_PageFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PageFieldTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageTranslations",
                columns: table => new
                {
                    PageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    NavigationTitle = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Excerpt = table.Column<string>(type: "TEXT", nullable: true),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    OgTitle = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    OgDescription = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageTranslations", x => new { x.PageId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_PageTranslations_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PageTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageBlockFieldTranslations",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageBlockFieldTranslations", x => new { x.FieldId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_PageBlockFieldTranslations_Piranha_BlockFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Piranha_BlockFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PageBlockFieldTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostFieldTranslations",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostFieldTranslations", x => new { x.FieldId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_PostFieldTranslations_Piranha_PostFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Piranha_PostFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PostFieldTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostTranslations",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Excerpt = table.Column<string>(type: "TEXT", nullable: true),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    OgTitle = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    OgDescription = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostTranslations", x => new { x.PostId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_PostTranslations_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PostTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageTranslations_LanguageId_Slug",
                table: "Piranha_PageTranslations",
                columns: new[] { "LanguageId", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostTranslations_LanguageId_Slug",
                table: "Piranha_PostTranslations",
                columns: new[] { "LanguageId", "Slug" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Piranha_PageFieldTranslations");
            migrationBuilder.DropTable(name: "Piranha_PageTranslations");
            migrationBuilder.DropTable(name: "Piranha_PageBlockFieldTranslations");
            migrationBuilder.DropTable(name: "Piranha_PostFieldTranslations");
            migrationBuilder.DropTable(name: "Piranha_PostTranslations");
        }
    }
}
