using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class AddContentGroupListImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ListImage",
                table: "Piranha_ContentGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListImage",
                table: "Piranha_ContentGroups");
        }
    }
}
