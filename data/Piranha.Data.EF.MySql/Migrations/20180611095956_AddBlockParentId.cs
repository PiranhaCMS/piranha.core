using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.MySql.Migrations
{
    [NoCoverage]
    public partial class AddBlockParentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Piranha_PostBlocks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Piranha_PageBlocks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Piranha_PostBlocks");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Piranha_PageBlocks");
        }
    }
}
