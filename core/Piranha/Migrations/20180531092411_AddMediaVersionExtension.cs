using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddMediaVersionExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Piranha_MediaVersions",
                maxLength: 8,
                nullable: true);

            // Set the fileextension for all already generation versions.
            migrationBuilder.Sql("UPDATE \"Piranha_MediaVersions\" SET \"FileExtension\"='.jpg' WHERE \"FileExtension\" IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Piranha_MediaVersions");
        }
    }
}
