using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    public partial class AddMetaRobots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaRobots",
                table: "Piranha_Posts",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaRobots",
                table: "Piranha_Pages",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaRobots",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "MetaRobots",
                table: "Piranha_Pages");
        }
    }
}
