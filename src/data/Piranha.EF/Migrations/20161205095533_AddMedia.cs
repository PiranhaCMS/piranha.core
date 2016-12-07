using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.EF.Migrations
{
    public partial class AddMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_MediaFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_MediaFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 255, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(maxLength: 128, nullable: true),
                    FileSize = table.Column<long>(nullable: false),
                    FolderId = table.Column<Guid>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: false),
                    PublicUrl = table.Column<string>(maxLength: 255, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Media_Piranha_MediaFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Piranha_MediaFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Media_FolderId",
                table: "Piranha_Media",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_Media");

            migrationBuilder.DropTable(
                name: "Piranha_MediaFolders");
        }
    }
}
