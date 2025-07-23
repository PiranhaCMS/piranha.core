using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Piranha.Data.EF.PostgreSql.Migrations
{
    [ExcludeFromCodeCoverage]
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
