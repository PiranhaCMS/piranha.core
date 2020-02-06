using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [NoCoverage]
    public partial class AddCLRContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CLRType",
                table: "Piranha_PostTypes",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CLRType",
                table: "Piranha_PageTypes",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CLRType",
                table: "Piranha_PostTypes");

            migrationBuilder.DropColumn(
                name: "CLRType",
                table: "Piranha_PageTypes");
        }
    }
}
