using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddMediaFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Piranha_MediaFolders",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "Piranha_Media",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Piranha_Media",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Properties",
                table: "Piranha_Media",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Piranha_Media",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Piranha_MediaFolders");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Properties",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Piranha_Media");
        }
    }
}
