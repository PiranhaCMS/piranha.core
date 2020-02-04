using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddContentPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_PagePermissions",
                columns: table => new
                {
                    PageId = table.Column<Guid>(nullable: false),
                    Permission = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PagePermissions", x => new { x.PageId, x.Permission });
                    table.ForeignKey(
                        name: "FK_Piranha_PagePermissions_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostPermissions",
                columns: table => new
                {
                    PostId = table.Column<Guid>(nullable: false),
                    Permission = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostPermissions", x => new { x.PostId, x.Permission });
                    table.ForeignKey(
                        name: "FK_Piranha_PostPermissions_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_PagePermissions");

            migrationBuilder.DropTable(
                name: "Piranha_PostPermissions");
        }
    }
}
