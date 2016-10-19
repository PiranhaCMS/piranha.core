/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.EF.Migrations
{
    public partial class AddBlocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Piranha_BlockTypes",
                columns: table => new {
                    Id = table.Column<string>(maxLength: 32, nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Piranha_BlockTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_Blocks",
                columns: table => new {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Published = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    TypeId = table.Column<string>(maxLength: 32, nullable: false),
                    View = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Piranha_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Blocks_Piranha_BlockTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Piranha_BlockTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_BlockFields",
                columns: table => new {
                    Id = table.Column<Guid>(nullable: false),
                    BlockId = table.Column<Guid>(nullable: false),
                    CLRType = table.Column<string>(maxLength: 255, nullable: false),
                    FieldId = table.Column<string>(maxLength: 32, nullable: false),
                    RegionId = table.Column<string>(maxLength: 32, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Piranha_BlockFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_BlockFields_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Blocks_TypeId",
                table: "Piranha_Blocks",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_BlockFields_BlockId",
                table: "Piranha_BlockFields",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_BlockFields_BlockId_RegionId_FieldId_SortOrder",
                table: "Piranha_BlockFields",
                columns: new[] { "BlockId", "RegionId", "FieldId", "SortOrder" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "Piranha_BlockFields");

            migrationBuilder.DropTable(
                name: "Piranha_Blocks");

            migrationBuilder.DropTable(
                name: "Piranha_BlockTypes");
        }
    }
}
