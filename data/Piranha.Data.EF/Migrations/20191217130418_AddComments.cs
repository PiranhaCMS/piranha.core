using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Migrations
{
    [NoCoverage]
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_PageComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Author = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PageComments_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PostComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: true),
                    Author = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostComments_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageComments_PageId",
                table: "Piranha_PageComments",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostComments_PostId",
                table: "Piranha_PostComments",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_PageComments");

            migrationBuilder.DropTable(
                name: "Piranha_PostComments");
        }
    }
}
