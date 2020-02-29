using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.PostgreSql.Migrations
{
    [NoCoverage]
    public partial class AddPostRevisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_PostRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostRevisions_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostRevisions_PostId",
                table: "Piranha_PostRevisions",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_PostRevisions");
        }
    }
}
