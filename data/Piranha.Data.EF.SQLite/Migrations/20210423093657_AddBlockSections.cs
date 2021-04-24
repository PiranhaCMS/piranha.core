using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class AddBlockSections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Piranha_PostBlocks_PostId_SortOrder",
                table: "Piranha_PostBlocks");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_PageBlocks_PageId_SortOrder",
                table: "Piranha_PageBlocks");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Piranha_PostBlocks",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Piranha_PageBlocks",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Piranha_ContentBlocks",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "Blocks");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostBlocks_PostId_SectionId_SortOrder",
                table: "Piranha_PostBlocks",
                columns: new[] { "PostId", "SectionId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageBlocks_PageId_SectionId_SortOrder",
                table: "Piranha_PageBlocks",
                columns: new[] { "PageId", "SectionId", "SortOrder" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Piranha_PostBlocks_PostId_SectionId_SortOrder",
                table: "Piranha_PostBlocks");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_PageBlocks_PageId_SectionId_SortOrder",
                table: "Piranha_PageBlocks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Piranha_PostBlocks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Piranha_PageBlocks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Piranha_ContentBlocks");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostBlocks_PostId_SortOrder",
                table: "Piranha_PostBlocks",
                columns: new[] { "PostId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageBlocks_PageId_SortOrder",
                table: "Piranha_PageBlocks",
                columns: new[] { "PageId", "SortOrder" },
                unique: true);
        }
    }
}
