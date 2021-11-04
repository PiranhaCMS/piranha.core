using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLServer.Migrations
{
    public partial class AddContentBlocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Piranha_ContentGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Piranha_ContentBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CLRType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentBlocks_Piranha_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Piranha_Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentBlockFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CLRType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentBlockFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentBlockFields_Piranha_ContentBlocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_ContentBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentBlockFieldTranslations",
                columns: table => new
                {
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentBlockFieldTranslations", x => new { x.FieldId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_Piranha_ContentBlockFieldTranslations_Piranha_ContentBlockFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Piranha_ContentBlockFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_ContentBlockFieldTranslations_Piranha_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Piranha_Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentBlockFields_BlockId_FieldId_SortOrder",
                table: "Piranha_ContentBlockFields",
                columns: new[] { "BlockId", "FieldId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentBlockFieldTranslations_LanguageId",
                table: "Piranha_ContentBlockFieldTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentBlocks_ContentId",
                table: "Piranha_ContentBlocks",
                column: "ContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_ContentBlockFieldTranslations");

            migrationBuilder.DropTable(
                name: "Piranha_ContentBlockFields");

            migrationBuilder.DropTable(
                name: "Piranha_ContentBlocks");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Piranha_ContentGroups");
        }
    }
}
