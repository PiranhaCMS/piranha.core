using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Piranha.Data.EF.SQLServer.Migrations
{
    [NoCoverage]
    public partial class AddBlocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_Blocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    IsReusable = table.Column<bool>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Blocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_BlockFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlockId = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 256, nullable: false),
                    FieldId = table.Column<string>(maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_BlockFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_BlockFields_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_PageBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BlockId = table.Column<Guid>(nullable: false),
                    PageId = table.Column<Guid>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PageBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PageBlocks_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PageBlocks_Piranha_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Piranha_Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_BlockFields_BlockId_FieldId_SortOrder",
                table: "Piranha_BlockFields",
                columns: new[] { "BlockId", "FieldId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageBlocks_BlockId",
                table: "Piranha_PageBlocks",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageBlocks_PageId_SortOrder",
                table: "Piranha_PageBlocks",
                columns: new[] { "PageId", "SortOrder" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_BlockFields");

            migrationBuilder.DropTable(
                name: "Piranha_PageBlocks");

            migrationBuilder.DropTable(
                name: "Piranha_Blocks");
        }
    }
}
