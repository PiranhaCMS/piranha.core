using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddBrowserTitleColumnPages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                                             name: "BrowserTitle",
                                             table: "Piranha_Pages",
                                             nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        name: "BrowserTitle",
                                        table: "Piranha_Pages");
        }
    }
}
