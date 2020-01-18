using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddCloseCommentsAfter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Pages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Pages");
        }
    }
}
