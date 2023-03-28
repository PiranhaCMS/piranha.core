using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.PostgreSql.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddOgFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Piranha_Posts",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgDescription",
                table: "Piranha_Posts",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OgImageId",
                table: "Piranha_Posts",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OgTitle",
                table: "Piranha_Posts",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Piranha_Pages",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgDescription",
                table: "Piranha_Pages",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OgImageId",
                table: "Piranha_Pages",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OgTitle",
                table: "Piranha_Pages",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "OgDescription",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "OgImageId",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "OgTitle",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "OgDescription",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "OgImageId",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "OgTitle",
                table: "Piranha_Pages");
        }
    }
}
