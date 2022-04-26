using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class AddSectionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Piranha_PostBlocks",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Piranha_PageBlocks",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "Piranha_ContentBlocks",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "Blocks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Piranha_PostBlocks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Piranha_PageBlocks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Piranha_ContentBlocks");
        }
    }
}
