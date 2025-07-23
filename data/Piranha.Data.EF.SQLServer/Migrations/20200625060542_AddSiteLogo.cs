using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLServer.Migrations
{
    public partial class AddSiteLogo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LogoId",
                table: "Piranha_Sites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Piranha_Sites");
        }
    }
}
