using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.MySql.Migrations
{
    [NoCoverage]
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Posts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableComments",
                table: "Piranha_Posts",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Pages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableComments",
                table: "Piranha_Pages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Piranha_MediaFolders",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "Piranha_Media",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Piranha_Media",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Properties",
                table: "Piranha_Media",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Piranha_Media",
                maxLength: 128,
                nullable: true);

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
                name: "Piranha_PagePermissions");

            migrationBuilder.DropTable(
                name: "Piranha_PostComments");

            migrationBuilder.DropTable(
                name: "Piranha_PostPermissions");

            migrationBuilder.DropColumn(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "EnableComments",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "CloseCommentsAfterDays",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "EnableComments",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Piranha_MediaFolders");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Properties",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Piranha_Media");
        }
    }
}
