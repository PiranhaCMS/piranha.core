using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.PostgreSql.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddPageInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "EnableComments",
                table: "Piranha_Posts",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Excerpt",
                table: "Piranha_Pages",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryImageId",
                table: "Piranha_Pages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Excerpt",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "PrimaryImageId",
                table: "Piranha_Pages");

            migrationBuilder.AlterColumn<bool>(
                name: "EnableComments",
                table: "Piranha_Posts",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
