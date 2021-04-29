using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class AddTranslationLastModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Piranha_ContentTranslations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Piranha_ContentTranslations");
        }
    }
}
