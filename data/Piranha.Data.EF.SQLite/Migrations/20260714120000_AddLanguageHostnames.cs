using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Piranha.Data.EF.SQLite;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations;

[DbContext(typeof(SQLiteDb))]
[Migration("20260714120000_AddLanguageHostnames")]
public partial class AddLanguageHostnames : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Hostnames",
            table: "Piranha_Languages",
            type: "TEXT",
            maxLength: 256,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Hostnames",
            table: "Piranha_Languages");
    }
}
