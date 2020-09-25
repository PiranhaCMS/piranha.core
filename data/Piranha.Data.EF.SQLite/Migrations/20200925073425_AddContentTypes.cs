using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class AddContentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_ContentGroups",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    CLRType = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_ContentTypes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CLRType = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Group = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentTypes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_ContentGroups");

            migrationBuilder.DropTable(
                name: "Piranha_ContentTypes");
        }
    }
}
